using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Sevices.Docker;

public class DockerBlueprintService
{
    private readonly string PathToBlueprints = "/Blueprints";

    public DockerContainer GetBlueprint(string name)
    {
        var path = GetPath(name);
        if (!File.Exists(path))
            return null;

        using var reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<DockerContainer>(json);
    }

    public IEnumerable<string> GetBlueprintNames()
        => Directory.GetFiles(PathToBlueprints).Select(x => Path.GetFileNameWithoutExtension(x));

    //private bool AddBlueprint(DockerContainer container)
    //{
    //    var path = GetPath(container.Name);
    //    if (!File.Exists(path))
    //        return false;

    //    File.WriteAllText(path, JsonConvert.SerializeObject(container));
    //    return true;
    //}

    private string GetPath(string name)
        => Path.Combine(PathToBlueprints, $"{name}.json");
}
