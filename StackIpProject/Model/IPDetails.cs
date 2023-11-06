using StackIpProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackIpProject.Model
{
    public class IPDetails : IIPDetails
    {
     
        public string? Ip { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
