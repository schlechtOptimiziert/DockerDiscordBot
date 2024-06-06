using DiscordBot.Sevices.Docker;
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

    public async Task<bool> CreateContainerAsync(DockerContainer container)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        var response = await dockerClient.Containers.CreateContainerAsync(container.ToCreateParameters()).ConfigureAwait(false);
        if(response.Warnings.Any())
            return false;

        await dockerClient.Containers.StartContainerAsync(response.ID, new()).ConfigureAwait(false);
        return true;
    }

    public async Task RemoveContainerAsync(DockerContainer container)
    {
        var dockerContainers = await GetConatinersAsync(new string[] { container.Name }).ConfigureAwait(false);
        using var dockerClient = dockerClientConfiguration.CreateClient();
        
        foreach(var dockerContainer in dockerContainers)
        {
            if (string.Equals(dockerContainer.State, "running", StringComparison.OrdinalIgnoreCase))
                await dockerClient.Containers.StopContainerAsync(dockerContainer.ID, new()).ConfigureAwait(false);

            await dockerClient.Containers.RemoveContainerAsync(dockerContainer.ID, new()).ConfigureAwait(false);
        }
    }
}
