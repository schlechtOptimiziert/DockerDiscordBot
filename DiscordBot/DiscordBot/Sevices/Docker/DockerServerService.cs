using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Sevices.Docker;

public class DockerServerService
{
    private readonly string PathToBlueprints = "/Servers";

    public ServerConfig GetServerContainer(string name)
    {
        var path = GetPath(name);
        if (!File.Exists(path))
            return null;

        using var reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<ServerConfig>(json);
    }

    public IEnumerable<string> GetServerNames()
        => Directory.GetDirectories(PathToBlueprints).Select(x => Path.GetFileName(x));

    private string GetPath(string name)
        => Path.Combine(PathToBlueprints, name, "dockerProperties.json");
}
