using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ora.API.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IBusClient BusClient;
        public BaseController(IBusClient busClient)
        {
            BusClient = busClient;
        }

        private string _endpoint;
        public enum EndpointMethod
        {
            Config
        }
        public enum HTTPMethod
        {
            Get,
            Post,
            Put,
            Delete
        }
        public string ApplicationName => System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
        protected void SetEndpoint(string endpoint) => _endpoint = endpoint;
        protected async Task<T> MakeRequestAsync<T, TIn>(HTTPMethod httpMethod, TIn model = null, EndpointMethod? route = null) where TIn : class where T : class, new()
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri($"{_endpoint}/{route?.ToString().ToLower()}");
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Content = content,
                    Method = new HttpMethod(httpMethod.ToString())
                };
                HttpResponseMessage response = await client.SendAsync(request);

                string apiResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    if (apiResponse != "")
                        return JsonConvert.DeserializeObject<T>(apiResponse);
                    else if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                        throw new Exception();
                    return new T();
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
        }
        protected async Task<T> MakeRequestAsync<T>(HTTPMethod httpMethod, EndpointMethod? route = null, Dictionary<string, string> parameters = null) where T : class, new()
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri($"{_endpoint}/{route?.ToString().ToLower()}");
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod.ToString()), uri);

                if (parameters != null)
                    requestMessage.Content = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = await client.SendAsync(requestMessage);

                string apiResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    if (apiResponse != "" && !apiResponse.ToLower().Contains("message"))
                        return JsonConvert.DeserializeObject<T>(apiResponse);
                    else if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                        throw new Exception();
                    return new T();
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
        }

    }
}
