using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ValidareDate.Models;

namespace ValidareDate.Services
{
    public class HttpService
    {
        private static string url = "https://webservicesp.anaf.ro/PlatitorTvaRest/api/v3/ws/tva";

        public static async Task<AnafResponse> PostCui(List<CuiRequest> cuiList)
        {

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(cuiList);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var jsonRes = await response.Content.ReadAsStringAsync();

                var anafResponse = JsonConvert.DeserializeObject<AnafResponse>(jsonRes);
                return anafResponse;
            }
        }
    }
}