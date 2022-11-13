using System;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Nodes;
using RestSharp.Serializers.Json;

namespace CleanBudget.Services
{
    public class CurrencyConverter
    {
        static string api = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/";

        public static double Convert(string fromCode, string toCode, double value)
        {
            var client = new RestClient($"{api}{fromCode}/{toCode}.min.json");
            client.UseSystemTextJson(new JsonSerializerOptions { });
            var request = new RestRequest();
            request.AddHeader("accept", "application/json");
            RestResponse response = client.Execute(request);
            var result = (double)JsonNode.Parse(response.Content)[toCode] * value;
            return result;
        }
    }
}
