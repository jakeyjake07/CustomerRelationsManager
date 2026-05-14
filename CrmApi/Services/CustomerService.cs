using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrmApi.Models;
using Microsoft.Azure.Cosmos;

namespace CrmApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly Container _container;


        public CustomerService(CosmosClient client, IConfiguration configuration)
        {

            var databaseName = configuration["CosmosDb:DatabaseName"];
            var containerName = configuration["CosmosDb:ContainerName"];
            _container = client.GetContainer(databaseName, containerName);
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            customer.Id = Guid.NewGuid().ToString();
            var now = DateTime.UtcNow;
            customer.CreatedAt = now;
            customer.UpdatedAt = now;
            await _container.CreateItemAsync(customer);
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>("SELECT * FROM c");
            var results = new List<Customer>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<Customer> GetCustomerByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Customer>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByNameAsync(string name)
        {
            var queryDef = new QueryDefinition("SELECT * FROM c WHERE CONTAINS(c.Name, @name)")
                .WithParameter("@name", name);
            var query = _container.GetItemQueryIterator<Customer>(queryDef);
            var results = new List<Customer>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<IEnumerable<Customer>> GetCustomersBySalespersonAsync(string salespersonName)
        {
            var queryDef = new QueryDefinition("SELECT * FROM c WHERE CONTAINS(c.Salesperson.Name, @salespersonName)")
                .WithParameter("@salespersonName", salespersonName);
            var query = _container.GetItemQueryIterator<Customer>(queryDef);
            var results = new List<Customer>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<Customer> UpdateCustomerAsync(string id, Customer customer)
        {
            try
            {
                customer.Id = id;
                customer.UpdatedAt = DateTime.UtcNow;
                var existing = await GetCustomerByIdAsync(id);
                customer.CreatedAt = existing.CreatedAt;
                var response = await _container.ReplaceItemAsync(customer, id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}