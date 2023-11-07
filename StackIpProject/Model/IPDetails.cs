using Newtonsoft.Json;
using StackIpProject.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackIpProject.Model
{
    public class IPDetails : IIPDetails
    {

        [Required]
        [MaxLength(20)]
        public string? Ip { get; set; }

        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public string? City { get; set; }

        [JsonProperty("country_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Country { get; set; }

        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Latitude { get; set; }

        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Longitude { get; set; }
    }
}
