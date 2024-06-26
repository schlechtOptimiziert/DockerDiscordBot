﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DiscordBot.Exceptions;
using Discord.Net;

namespace DiscordBot.Sevices;

public class NgrokService
{
    private readonly HttpClient client = new();
    private readonly string ngrokIp;

    public NgrokService()
    {
        ngrokIp = Environment.GetEnvironmentVariable("NGROK_IP") ?? throw new ConfigurationException("Environment variable 'NGROK_IP' has not been set.");
    }

    public async Task<IEnumerable<NgrokTunnelResponeBody>> GetTunnelsAsync()
    {
        using var response = await client.GetAsync($"http://{ngrokIp}:4040/api/tunnels").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return NgrokTunnelResponeBody.GetNgrokTunnels(responseBody);
    }

    public async Task<NgrokTunnelResponeBody> GetTunnelAsync(string name)
    {
        using var response = await client.GetAsync($"http://{ngrokIp}:4040/api/tunnels/{name}").ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        var responseBody = await response.Content.ReadAsStringAsync();
        return NgrokTunnelResponeBody.GetNgrokTunnel(responseBody);
    }

    public async Task<bool> StartTunnelAsync(NgrokTunnelCreateRequestBody request)
    {
        if(await GetTunnelAsync(request.name).ConfigureAwait(false) is not null)
            return true;

        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        using var response = await client.PostAsync($"http://{ngrokIp}:4040/api/tunnels", content).ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> StopTunnelAsync(string name)
    {
        if (await GetTunnelAsync(name).ConfigureAwait(false) is null)
            return true;

        using var response = await client.DeleteAsync($"http://{ngrokIp}:4040/api/tunnels/{name}").ConfigureAwait(false);
        return response.IsSuccessStatusCode;
    }
}
