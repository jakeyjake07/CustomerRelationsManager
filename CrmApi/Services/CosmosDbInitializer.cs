using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CrmApi.Services
{
    public class CosmosDbInitializer : IHostedService
    {
        private readonly CosmosClient _client;
        private readonly IConfiguration _configuration;

        public CosmosDbInitializer(CosmosClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            var databaseName = _configuration["CosmosDb:DatabaseName"];
            var containerName = _configuration["CosmosDb:ContainerName"];

            return InitializeCosmosDbAsync(databaseName, containerName);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        private async Task InitializeCosmosDbAsync(string databaseName, string containerName)
        {
            var database = await _client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
        }

    }


}