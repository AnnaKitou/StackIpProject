using IpStackAPI.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace IpStackAPI.Entities
{
    public class DetailsOfIp: IHasIpProperty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Ip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
