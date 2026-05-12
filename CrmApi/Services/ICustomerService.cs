using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrmApi.Models;

namespace CrmApi.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(string id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(string id, Customer customer);
        Task<bool> DeleteCustomerAsync(string id);
        Task<IEnumerable<Customer>> GetCustomersBySalespersonAsync(string salespersonName);
        Task<IEnumerable<Customer>> GetCustomersByNameAsync(string name);

    }
}