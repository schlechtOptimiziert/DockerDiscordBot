using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPBot.Sevices;

public class DockerService
{
    private readonly DockerClientConfiguration dockerClientConfiguration;

    public DockerService()
    {
#if DEBUG
        dockerClientConfiguration = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"));
#else
        dockerClientConfiguration = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"));
#endif
    }

    public async Task<IEnumerable<ContainerListResponse>> GetConatinersAsync()
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        return await dockerClient.Containers.ListContainersAsync(
            new()
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["ancestor"] = new Dictionary<string, bool>
                    {
                        ["itzg/minecraft-server"] = true
                    }
                },
                All = true,
            }
        ).ConfigureAwait(false);
    }

    public async Task<string> PauseContainerAsync(string Id)
    {
        var container = await GetContainerAsync(Id).ConfigureAwait(false);
        if (container is null)
            return "Id not found";

        using var dockerClient = dockerClientConfiguration.CreateClient();
        await dockerClient.Containers.PauseContainerAsync(Id).ConfigureAwait(false);
        return "Done";
    }

    public async Task<string> UnpauseContainerAsync(string Id)
    {
        var container = await GetContainerAsync(Id).ConfigureAwait(false);
        if (container is null)
            return "Id not found";

        using var dockerClient = dockerClientConfiguration.CreateClient();
        await dockerClient.Containers.UnpauseContainerAsync(Id).ConfigureAwait(false);
        return "Done";
    }

    private async Task<ContainerListResponse> GetContainerAsync(string Id)
    {
        var response = await GetConatinersAsync().ConfigureAwait(false);
        return response.FirstOrDefault(x => x.ID == Id);
    }
}
