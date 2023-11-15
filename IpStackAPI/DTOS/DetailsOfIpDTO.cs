using IpStackAPI.Interfaces;

namespace IpStackAPI.DTOS
{
    public class DetailsOfIpDTO
    {
        public string? Ip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
