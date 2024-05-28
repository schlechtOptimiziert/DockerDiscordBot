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
        return await dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true, });
    }

    public async Task PauseContainerAsync(string Id)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        await dockerClient.Containers.PauseContainerAsync(Id).ConfigureAwait(false);
    }

    public async Task UnpauseContainerAsync(string Id)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        await dockerClient.Containers.UnpauseContainerAsync(Id).ConfigureAwait(false);
    }
}
