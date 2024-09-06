using DiscordBot.Sevices.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Sevices;

public class DockerService
{
    private readonly DockerClientConfiguration dockerClientConfiguration;
    private readonly string PathToBlueprints = "/Servers";
    private readonly NgrokService ngrokService;

    public DockerService(NgrokService ngrokService)
    {
        dockerClientConfiguration = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"));
        this.ngrokService = ngrokService ?? throw new ArgumentNullException(nameof(ngrokService));
    }

    public async Task<IEnumerable<ContainerListResponse>> GetConatinersAsync(IEnumerable<string> names)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        var filter = new ContainersListParameters() { All = true };
        
        if (names.Any())
        {
            var acceptedNames = new Dictionary<string, bool>();
            foreach (var name in names)
                acceptedNames[name] = true;

            filter.Filters = new Dictionary<string, IDictionary<string, bool>> { ["name"] = acceptedNames };
        }
        
        return await dockerClient.Containers.ListContainersAsync(filter).ConfigureAwait(false);
    }

    public async Task<bool> CreateContainerAsync(ServerConfig serverConfig)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        var response = await dockerClient.Containers.CreateContainerAsync(serverConfig.ToContainerCreateParameters()).ConfigureAwait(false);
        if(response.Warnings.Any())
            return false;

        await dockerClient.Containers.StartContainerAsync(response.ID, new()).ConfigureAwait(false);
        await ngrokService.StartTunnelAsync(serverConfig.NgrokConfig).ConfigureAwait(false);
        return true;
    }

    public async Task RemoveContainerAsync(ServerConfig serverConfig)
    {
        var dockerContainers = await GetConatinersAsync(new string[] { serverConfig.Name }).ConfigureAwait(false);
        var dockerContainer = dockerContainers.Single();
        using var dockerClient = dockerClientConfiguration.CreateClient();

        if (string.Equals(dockerContainer.State, "running", StringComparison.OrdinalIgnoreCase))
            await dockerClient.Containers.StopContainerAsync(dockerContainer.ID, new()).ConfigureAwait(false);

        await dockerClient.Containers.RemoveContainerAsync(dockerContainer.ID, new()).ConfigureAwait(false);
        await ngrokService.StopTunnelAsync(serverConfig.NgrokConfig.name).ConfigureAwait(false);
    }

    public ServerConfig GetServerConfig(string name)
    {
        var path = Path.Combine(PathToBlueprints, name, "dockerProperties.json");
        if (!File.Exists(path))
            return null;

        using var reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<ServerConfig>(json);
    }

    public IEnumerable<string> GetServerNames()
        => Directory.GetDirectories(PathToBlueprints).Select(x => Path.GetFileName(x));
}
