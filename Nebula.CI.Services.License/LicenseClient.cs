using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Nebula.CI.Services.License
{
    public class LicenseClient : ISingletonDependency
    {
        private HttpClient client = new HttpClient();
        private string id = "nebula.ci";
        private string baseUrl = "http://nebula-ci-license:443/nebula.license/License";
        //private string baseUrl = "http://172.18.67.106:5600/nebula.license/License";

        public async Task<bool> IsValid()
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/Permission/{id}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> IsOnline()
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/Update/{id}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
