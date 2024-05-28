using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PnPBot.Exceptions;

namespace PnPBot.Sevices;

public class NgrokService
{
    private readonly HttpClient client = new();
    private readonly string ngrokIp;

    public NgrokService()
    {
#if DEBUG
        ngrokIp = Environment.GetEnvironmentVariable("NGROK_IP");
#else
        ngrokIp = Environment.GetEnvironmentVariable("NGROK_IP") ?? throw new ConfigurationException("Environment variable 'NGROK_IP' has not been set.");
#endif
    }

    public async Task<IEnumerable<NgrokTunnel>> GetTunnelsAsync()
    {
        using HttpResponseMessage response = await client.GetAsync($"http://{ngrokIp}:4040/api/tunnels").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return NgrokTunnels.GetNgrokTunnels(responseBody).tunnels;
    }
}
