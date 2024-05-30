using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Sevices;

public class DockerService
{
    private readonly DockerClientConfiguration dockerClientConfiguration;

    public DockerService()
    {
        dockerClientConfiguration = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"));
    }

    public async Task<IEnumerable<ContainerListResponse>> GetConatinersAsync(params string[] imageNames)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        var filter = new ContainersListParameters() { All= true };
        
        if (imageNames.Any())
        {
            var acceptedImages = new Dictionary<string, bool>();
            foreach (var imageName in imageNames)
                acceptedImages[imageName] = true;

            filter.Filters = new Dictionary<string, IDictionary<string, bool>> { ["ancestor"] = acceptedImages };
        }
        
        return await dockerClient.Containers.ListContainersAsync(filter).ConfigureAwait(false);
    }

    public async Task<bool> PauseContainerAsync(string Id)
    {
        var container = await GetContainerAsync(Id).ConfigureAwait(false);
        if (container is null)
            return false;

        if (!string.Equals(container.State, "running", StringComparison.OrdinalIgnoreCase))
            return true;

        using var dockerClient = dockerClientConfiguration.CreateClient();
        await dockerClient.Containers.PauseContainerAsync(Id).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> UnpauseContainerAsync(string Id)
    {
        var container = await GetContainerAsync(Id).ConfigureAwait(false);
        if (container is null)
            return false;

        if (string.Equals(container.State, "running", StringComparison.OrdinalIgnoreCase))
            return true;

        using var dockerClient = dockerClientConfiguration.CreateClient();
        await dockerClient.Containers.UnpauseContainerAsync(Id).ConfigureAwait(false);
        return true;
    }

    private async Task<ContainerListResponse> GetContainerAsync(string Id)
    {
        var response = await GetConatinersAsync().ConfigureAwait(false);
        return response.FirstOrDefault(x => x.ID == Id);
    }
}
