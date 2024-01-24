using Discord;
using Discord.WebSocket;
using NgrokSharp;
using NgrokSharp.DTO;
using PnPBot.Sevices;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PnPBot.Commands;

public sealed class TextCommands : IDisposable
{
    private static ProcessStartInfo mcServerProcessInfo;
    private static Process mcServerProcess;
    private static INgrokManager ngrokManager;
    private const string ngrokTunnelName = "MC Tunnel";

    public TextCommands(Logger logger)
    {
        mcServerProcessInfo = new ProcessStartInfo()
        {
            FileName = "java",
            Arguments = ("-Xmx2048M -Xms2048M -jar server.jar nogui"),
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = "/App/MinecraftServer",
        };

        ngrokManager = new NgrokManager(logger);
        ngrokManager.DownloadAndUnzipNgrokAsync().GetAwaiter().GetResult();
        ngrokManager.RegisterAuthTokenAsync(Environment.GetEnvironmentVariable("NGROK_AUTH_TOKEN")).GetAwaiter().GetResult();
        ngrokManager.StartNgrokWithLogging(NgrokManager.Region.Europe);
    }

    public async Task StartCommand(SocketSlashCommand command)
    {
        var embedBuiler = new EmbedBuilder()
            .WithTitle("Start MC Server")
            .WithDescription("starting")
            .WithColor(Color.Green)
            .WithCurrentTimestamp();
        await command.RespondAsync(embed: embedBuiler.Build());

        mcServerProcess = Process.Start(mcServerProcessInfo);

        var tunnel = new StartTunnelDTO
        {
            name = ngrokTunnelName,
            proto = "tcp",
            addr = "25565"
        };
        var httpResponseMessage = await ngrokManager.StartTunnelAsync(tunnel);

        if ((int)httpResponseMessage.StatusCode == 201)
        {
            var tunnelDetail = JsonSerializer.Deserialize<TunnelDetailDTO>(await httpResponseMessage.Content.ReadAsStringAsync());

            embedBuiler = new EmbedBuilder()
                .WithTitle("Start MC Server")
                .WithDescription(tunnelDetail.PublicUrl.ToString())
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            var channel = await command.GetChannelAsync().ConfigureAwait(false);
            await channel.SendMessageAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
        }
        else
        {
            embedBuiler = new EmbedBuilder()
                .WithTitle("Start MC Server")
                .WithDescription($"something is wrong :( {httpResponseMessage.ReasonPhrase} {httpResponseMessage.StatusCode} {httpResponseMessage.RequestMessage}")
                .WithColor(Color.Red)
                .WithCurrentTimestamp();

            var channel = await command.GetChannelAsync().ConfigureAwait(false);
            await channel.SendMessageAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
        }
    }

    public async Task StatusCommand(SocketSlashCommand command)
    {
        var status = false;
        try
        {
            status = !mcServerProcess.HasExited;
        }
        catch { }
        var embedBuiler = new EmbedBuilder()
            .WithTitle("Status MC Server")
            .WithDescription(status ? "Server running" : "Server not running")
            .WithColor(status ? Color.Green : Color.Red)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuiler.Build());
    }

    public async Task StopCommand(SocketSlashCommand command)
    {
        await ngrokManager.StopTunnelAsync(ngrokTunnelName).ConfigureAwait(false);
        ngrokManager.StopNgrok();
        mcServerProcess.StandardInput.WriteLine("stop");

        var embedBuiler = new EmbedBuilder()
            .WithTitle("Stop MC Server")
            .WithDescription("stoped")
            .WithColor(Color.Red)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuiler.Build());
    }

    public void Dispose()
    {
        ngrokManager.StopTunnelAsync(ngrokTunnelName).GetAwaiter().GetResult();
        ngrokManager.StopNgrok();
        ngrokManager.Dispose();
        mcServerProcess.StandardInput.WriteLine("stop");
        mcServerProcess.WaitForExit();
    }
}
