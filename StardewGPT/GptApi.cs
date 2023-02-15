using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using StardewModdingAPI;

namespace StardewGPT
{
    public class GptApi
    {
        static Uri Endpoint = new Uri("https://api.openai.com/v1/completions");

        static string Model = "text-davinci-003";

        static float Temperature = 0.7f;

        static int MaxTokens = 128;

        public HttpClient Client;

        public IMonitor Monitor;

        public GptApi(IMonitor monitor)
        {
            this.Client = new HttpClient();
            this.Monitor = monitor;

            // setup authentication, read key from env
            string openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);
            this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openaiKey);
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var request = CreateRequestMessage(prompt);
            HttpResponseMessage response = await Client.SendAsync(request);
            this.Monitor.Log($"Received response with status code {response.StatusCode}", LogLevel.Debug);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                GptCompletionResponse responseObject = JsonSerializer.Deserialize<GptCompletionResponse>(responseContent);
                return responseObject.choices[0].text;
            }
            else
                return response.ReasonPhrase;
        }

        private HttpRequestMessage CreateRequestMessage(string prompt)
        {
            var requestData = new GptCompletionRequestData
            {
                model = GptApi.Model,
                prompt = prompt, 
                temperature = GptApi.Temperature,
                max_tokens = GptApi.MaxTokens,
                stop = "\n"
            };
            
            string requestDataJson = JsonSerializer.Serialize(requestData);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = GptApi.Endpoint,
                Content = new StringContent(requestDataJson, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            return request;
        }
    }
}