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

                var mail = new MailMessage
                {
                    From = new MailAddress("noreply@crm.com"),
                    Subject = "New customer assigned to you",
                    Body = $"You have been assigned as the responsible salesperson for {customer.Name}",
                    IsBodyHtml = false
                };

                mail.To.Add(customer.Salesperson.Email);
                await smtpClient.SendMailAsync(mail);
            }
        }
    }
}