using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CrmApi.Models
{
    public class Customer
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Salesperson Salesperson { get; set; }

    }
}