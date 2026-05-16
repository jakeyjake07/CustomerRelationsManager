using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using CrmFunction.Models;

namespace CrmFunction
{
    public class CustomerChangedFunction
    {

        private readonly IConfiguration _configuration;

        public CustomerChangedFunction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Function("CustomerChangedFunction")]
        public async Task Run(
        [CosmosDBTrigger(
            databaseName: "CrmDb",
            containerName: "Customers",
            Connection = "CosmosDbConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]



        IReadOnlyList<Customer> customers)
        {
            foreach (var customer in customers)
            {
                var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
                {
                    Port = int.Parse(_configuration["MailtrapPort"]),
                    Credentials = new NetworkCredential(
                    _configuration["MailtrapUsername"],
                    _configuration["MailtrapPassword"]),
                    EnableSsl = true
                };

                MailMessage mail;

                if (customer.CreatedAt == customer.UpdatedAt)
                {
                    mail = new MailMessage
                    {
                        From = new MailAddress("noreply@crm.com"),
                        Subject = "New customer assigned to you",
                        Body = $"You have been assigned as the responsible salesperson for the following customer:\n\n" +
                                $"Name: {customer.Name}\n" +
                                $"Title: {customer.Title}\n" +
                                $"Phone: {customer.Phone}\n" +
                                $"Email: {customer.Email}\n" +
                                $"Address: {customer.Address}",
                    };

                }

                else
                {
                    mail = new MailMessage
                    {
                        From = new MailAddress("noreply@crm.com"),
                        Subject = "Updated information on your assigned customer",
                        Body = $"Your existing customer has been updated, check the new details:\n\n" +
                               $"Name: {customer.Name}\n" +
                               $"Title: {customer.Title}\n" +
                               $"Phone: {customer.Phone}\n" +
                               $"Email: {customer.Email}\n" +
                               $"Address: {customer.Address}",
                    };
                }

                mail.To.Add(customer.Salesperson.Email);
                await smtpClient.SendMailAsync(mail);
            }
        }
    }
}