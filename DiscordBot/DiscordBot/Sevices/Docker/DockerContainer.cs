using Docker.DotNet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Sevices.Docker;

public class DockerContainer
{
    public string Name { get; set; }
    public string Image { get; set; }
    public Dictionary<string, string> PortBindings { get; set; }
    public Dictionary<string, string> EnvironmentVariables { get; set; }
    public Dictionary<string, string> MountedVolumes { get; set; }

    public CreateContainerParameters ToCreateParameters()
    {
        var env = new List<string>();
        foreach (var environmentVariable in EnvironmentVariables)
            env.Add($"{environmentVariable.Key}={environmentVariable.Value}");

        var binds = new List<string>();
        foreach (var mountedVolume in MountedVolumes)
            binds.Add($"{mountedVolume.Key}:{mountedVolume.Value}");

        var exposedPorts = new Dictionary<string, EmptyStruct>();
        var portBindings = new Dictionary<string, IList<PortBinding>>();
        foreach (var portBinding in PortBindings)
        {
            exposedPorts.Add(portBinding.Value, new());
            portBindings.Add(portBinding.Value, new List<PortBinding> { new() { HostPort = portBinding.Key } });
        }

        return new()
        {
            Image = Image,
            Name = Name,
            Env = env,
            ExposedPorts = exposedPorts,
            HostConfig = new HostConfig
            {
                PortBindings = portBindings,
                Binds = binds,
            },
        };
    }
}