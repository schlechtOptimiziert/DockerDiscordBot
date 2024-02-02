using RconSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PnPBot.Sevices;

public class MinecraftServerService
{
    private const string serverAddress = "your-server-ip";
    private const int serverPort = 25575;

    static async Task SendCommandUsingRcon(string containerName, string rconPassword, string command)
    {
        var client = RconClient.Create(serverAddress, serverPort);

        // Open the connection
        await client.ConnectAsync();

        // Send a RCON packet with type AUTH and the RCON password for the target server
        var authenticated = await client.AuthenticateAsync("RCONPASSWORD");
        if (authenticated)
        {
            // If the response is positive, the connection is authenticated and further commands can be sent
            var status = await client.ExecuteCommandAsync("status");
            // Some responses will be split into multiple RCON pakcets when body length exceeds the maximum allowed
            // For this reason these commands needs to be issued with isMultiPacketResponse parameter set to true
            // An example is CS:GO cvarlist
            var cvarlist = await client.ExecuteCommandAsync("cvarlist", true);
        }

        client.Disconnect();
    }
}
