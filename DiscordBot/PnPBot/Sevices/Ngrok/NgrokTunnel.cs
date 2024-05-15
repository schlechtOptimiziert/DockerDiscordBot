using System.Collections.Generic;
using Newtonsoft.Json;

namespace PnPBot.Sevices.Ngrok;

public class NgrokTunnels
{
    public IEnumerable<NgrokTunnel> tunnels { get; set; }

    public static NgrokTunnels GetNgrokTunnels(string objectAsString)
        => JsonConvert.DeserializeObject<NgrokTunnels>(objectAsString);
}


public class NgrokTunnel
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Public_url { get; set; }
}
