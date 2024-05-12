using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PnPBot.Exceptions;
using PnPBot.Sevices.Ngrok;

namespace PnPBot.Sevices;

public class NgrokService
{
    private readonly HttpClient client = new();
    private readonly string ngrokIp;

    public NgrokService()
    {
        ngrokIp = Environment.GetEnvironmentVariable("NGROK_IP") ?? throw new ConfigurationException("Environment variable 'NGROK_IP' has not been set.");
    }

    public async Task<IEnumerable<NgrokTunnel>> GetTunnelsAsync()
    {
        using HttpResponseMessage response = await client.GetAsync($"http://{ngrokIp}:4040/api/tunnels").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return NgrokTunnels.GetNgrokTunnels(responseBody).tunnels;
    }
}
