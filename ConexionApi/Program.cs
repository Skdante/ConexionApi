using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConexionApi
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        public static async Task Main()
        {
            List<List<int>> amountlist = new List<List<int>>();
            // Obtención de información acerca del API
            client.BaseAddress = new Uri("https://jsonmock.hackerrank.com/api/transactions/");
            var result = await client.GetAsync("search?txnType=debit&page=1");
            var data = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            // Manejo de Json
            var datas = JsonConvert.DeserializeObject<JObject>(data)["data"];
            var categories = datas
                    .GroupBy(group => group["userId"])
                    .Select(g => new {
                        userId = (int)g.Key,
                        amount = g.Sum(x => Convert.ToDecimal(x["amount"].ToString().TrimStart('$')))
                    })
                    .ToList();

            foreach (var category in categories)
            {
                amountlist.Add(new List<int>
                {
                    category.userId,
                    Convert.ToInt16(category.amount)
                });
            }

            if (amountlist.Count < 1)
                amountlist.Add(new List<int> { -1, -1 });
        }
    }
}
