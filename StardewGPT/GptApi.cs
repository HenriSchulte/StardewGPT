using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StardewGPT
{
    public class GptApi
    {
        static Uri endpoint = new Uri("https://api.openai.com/v1/completions");

        static string model = "text-davinci-003";

        static double temperature = 0.7;

        static int max_tokens = 64;

        public HttpClient client;

        public GptApi()
        {
            this.client = new HttpClient();

            // setup authentication, read key from env
            string openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openaiKey);
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            var request = CreateRequestMessage(prompt);

            HttpResponseMessage response = await client.SendAsync(request);
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
                model = GptApi.model,
                prompt = prompt, 
                temperature = GptApi.temperature,
                max_tokens = GptApi.max_tokens,
                stop = "\n"
            };
            
            string requestDataJson = JsonSerializer.Serialize(requestData);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = GptApi.endpoint,
                Content = new StringContent(requestDataJson, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            return request;
        }
    }
}