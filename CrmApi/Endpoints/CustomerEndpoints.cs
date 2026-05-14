using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrmApi.Models;
using CrmApi.Services;

namespace CrmApi.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void MapCustomerEndpoints(this WebApplication app)
        {
            app.MapPost("/api/customers", async (Customer customer, ICustomerService service) =>
            {
                var createdCustomer = await service.AddCustomerAsync(customer);
                return Results.Created($"/api/customers/{createdCustomer.Id}", createdCustomer);

            });

            app.MapGet("/api/customers", async (ICustomerService service) =>
            {
                var customers = await service.GetAllCustomersAsync();
                return Results.Ok(customers);
            });

            app.MapGet("/api/customers/{id}", async (string id, ICustomerService service) =>
            {
                var customer = await service.GetCustomerByIdAsync(id);
                return customer != null ? Results.Ok(customer) : Results.NotFound();
            });

            app.MapGet("/api/customers/search", async (string? name, string? salesperson, ICustomerService service) =>
           {
               if (name != null)
               {
                   var customers = await service.GetCustomersByNameAsync(name);
                   return Results.Ok(customers);

               }
               else if (salesperson != null)
               {
                   var customers = await service.GetCustomersBySalespersonAsync(salesperson);
                   return Results.Ok(customers);
               }
               else
               {
                   return Results.BadRequest("Please provide either a name or salesperson query parameter.");
               }

           });

            app.MapDelete("/api/customers/{id}", async (string id, ICustomerService service) =>
            {
                var deleted = await service.DeleteCustomerAsync(id);
                if (deleted)
                {
                    return Results.NoContent();
                }
                else
                {
                    return Results.NotFound();
                }

            });

            app.MapPut("/api/customers/{id}", async (string id, Customer customer, ICustomerService service) =>
            {
                var updatedCustomer = await service.UpdateCustomerAsync(id, customer);
                if (updatedCustomer != null)
                {
                    return Results.Ok(updatedCustomer);
                }
                return Results.NotFound();
            });
        }
    }
}