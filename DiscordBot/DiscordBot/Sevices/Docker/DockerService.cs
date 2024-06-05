﻿using DiscordBot.Sevices.Docker;
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

    public async Task<bool> CreateContainerAsync(DockerContainer container)
    {
        using var dockerClient = dockerClientConfiguration.CreateClient();
        var response = await dockerClient.Containers.CreateContainerAsync(container.ToCreateParameters()).ConfigureAwait(false);
        if(response.Warnings.Any())
            return false;

        await dockerClient.Containers.StartContainerAsync(response.ID, new()).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> RemoveContainerAsync(string Id)
    {
        var container = await GetContainerAsync(Id).ConfigureAwait(false);
        if (container is null)
            return false;

        using var dockerClient = dockerClientConfiguration.CreateClient();
        if (string.Equals(container.State, "running", StringComparison.OrdinalIgnoreCase))
            await dockerClient.Containers.StopContainerAsync(Id, new()).ConfigureAwait(false);

        await dockerClient.Containers.RemoveContainerAsync(Id, new()).ConfigureAwait(false);
        return true;
    }

    private async Task<ContainerListResponse> GetContainerAsync(string Id)
    {
        var response = await GetConatinersAsync().ConfigureAwait(false);
        return response.FirstOrDefault(x => x.ID == Id);
    }
}
