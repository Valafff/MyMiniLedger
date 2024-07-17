using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using System.Web;

namespace MyMiniLedger.BLL.ServicesAPI
{
    public class Requsts
    {
        public async Task<string> GetCoinCourseToFiatAsync(string _coinId, string _fiatName)
        {
            try
            {
                //Запрос для одной монеты
                var options = new RestClientOptions($"https://api.coingecko.com/api/v3/simple/price?ids={_coinId}&vs_currencies={_fiatName}");
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("x-cg-pro-api-key", "CG-FBZRGPf3MZs9MAjbHkVr4acz");
                var response = await client.GetAsync(request);

                string result = response.Content;
                var res = result.Split(':', StringSplitOptions.RemoveEmptyEntries);
                result = res[result.Length-1];

                return response.Content;
            }
            catch (Exception)
            {
                return "Курс не определен";
                throw;
            }



        }
    }
}
