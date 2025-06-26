using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FunctionApp1
{
    public record class Titles(
[property: JsonPropertyName("ja")] string Ja,
[property: JsonPropertyName("en")] string En

);
    public record class TrainInformation(
    [property: JsonPropertyName("@context")] string Context,
    [property: JsonPropertyName("@id")] string Id,
    [property: JsonPropertyName("@type")] string Type,
    [property: JsonPropertyName("dc:date")] string Date,
    [property: JsonPropertyName("owl:sameAs")] string SameAs,
    [property: JsonPropertyName("dct:valid")] string Valid,
    [property: JsonPropertyName("odpt:timeOfOrigin")] string TimeOfOrigin,
    [property: JsonPropertyName("odpt:operator")] string Operator,
    [property: JsonPropertyName("odpt:railway")] string Railway,
    [property: JsonPropertyName("odpt:trainInformationText")] Titles TrainInformationText

    );



    public class HttpExample
    {


        [Function("TrainInfo")]
        public static async Task<OkObjectResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req) { 
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://cloudserverc3631597.azurewebsites.net/api/Train/traininformation");

            HttpResponseMessage apiresponse = await client.SendAsync(request);

            

            var content = await apiresponse.Content.ReadAsStreamAsync();
            var traininfo = await JsonSerializer.DeserializeAsync<List<TrainInformation>>(content);


            var delays = traininfo.Where(t => t.TrainInformationText.En.Contains("delay", StringComparison.OrdinalIgnoreCase)).Select(t => new { Railway = t.Railway, Status = t.TrainInformationText.En }).ToList();


            
            return new OkObjectResult(delays);

        }

    }
}
