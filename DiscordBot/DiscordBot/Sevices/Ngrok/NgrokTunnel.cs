using System.Collections.Generic;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace DiscordBot.Sevices;

public class NgrokTunnelResponeBody
{
    public string name { get; set; }
    public string id { get; set; }
    public string public_url { get; set; }
    public string proto { get; set; }

    public static NgrokTunnelResponeBody GetNgrokTunnel(string objectAsString)
        => JsonConvert.DeserializeObject<NgrokTunnelResponeBody>(objectAsString);

    public static IEnumerable<NgrokTunnelResponeBody> GetNgrokTunnels(string objectAsString)
        => JsonConvert.DeserializeObject<NgrokTunnels>(objectAsString).Tunnels;
    class NgrokTunnels
    {
        public IEnumerable<NgrokTunnelResponeBody> Tunnels { get; set; }
    }
}
