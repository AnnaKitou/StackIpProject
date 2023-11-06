using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackIpProject.Configuration;
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
        private readonly EndPointSetting _endPointSetting;
      
        public IPInfoProvider(HttpClient httpClient, IOptions<EndPointSetting> options)
        {
            _httpClient = httpClient;
            _endPointSetting = options.Value;
        }

        public async Task<IIPDetails> GetIPDetailsAsync(string ip)
        {
            string fullUrl = $"{_endPointSetting.Url}{ip}?access_key={_endPointSetting.Key}";

            Console.WriteLine(fullUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            var response = await _httpClient.SendAsync(request);

            try
            {
                response.EnsureSuccessStatusCode();
                var responseMessage = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<IPDetails>(responseMessage); 
                return result;
            }
            catch (HttpRequestException ex) when 
            (ex.StatusCode == HttpStatusCode.NotFound 
            || ex.StatusCode == HttpStatusCode.Unauthorized 
            || ex.StatusCode == HttpStatusCode.Forbidden 
            || ex.StatusCode == HttpStatusCode.BadGateway 
            || ex.StatusCode == HttpStatusCode.BadRequest 
            || ex.StatusCode == HttpStatusCode.GatewayTimeout 
            || ex.StatusCode == HttpStatusCode.RequestTimeout)
            {
                throw new HttpRequestException("IPServiceNotAvailableException");
            }
        }
    }
}
