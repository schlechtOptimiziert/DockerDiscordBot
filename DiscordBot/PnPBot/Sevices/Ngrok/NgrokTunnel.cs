using System.Collections.Generic;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace PnPBot.Sevices;


public class NgrokTunnelResponeBody
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Public_url { get; set; }
    public string Proto { get; set; }

    public static NgrokTunnelResponeBody GetNgrokTunnel(string objectAsString)
        => JsonConvert.DeserializeObject<NgrokTunnelResponeBody>(objectAsString);

    public static IEnumerable<NgrokTunnelResponeBody> GetNgrokTunnels(string objectAsString)
        => JsonConvert.DeserializeObject<NgrokTunnels>(objectAsString).Tunnels;
    class NgrokTunnels
    {
        public IEnumerable<NgrokTunnelResponeBody> Tunnels { get; set; }
    }
}

public class NgrokTunnelCreateRequestBody : NgrokTunnelResponeBody
{
    public string Addr { get; set; }

    public NgrokTunnelCreateRequestBody(string name, string protocoll, string port)
    {
        Name = name;
        Proto = protocoll;
        Addr = port;
    }
}