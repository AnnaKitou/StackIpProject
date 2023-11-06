using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackIpProject.Interfaces;
using StackIpProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StackIpProject
{
    public class IPInfoProvider : IIPInfoProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public IPInfoProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IIPDetails> GetIPDetailsAsync(string ip)
        {
            string fullUrl = $"{_configuration.GetSection("Url")}{ip}?access_key={_configuration.GetSection("Key")}";

            Console.WriteLine(fullUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            var response =await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseMessage = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<IPDetails>(responseMessage);


                return result;
            }
            throw new NotImplementedException();
        }

   
    }
}
