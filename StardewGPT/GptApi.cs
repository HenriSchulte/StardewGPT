using System;
using System.Collections.Generic;
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
        // If using Azure OpenAI, model is determined by deployment
        static string Model = "gpt-3.5-turbo";

        static float Temperature = 0.7f;

        static int MaxTokens = 300;

        public HttpClient Client;

        public IMonitor Monitor;

        public Uri Endpoint;

        public GptApi(IMonitor monitor)
        {
            this.Client = new HttpClient();
            this.Monitor = monitor;

            this.Setup();
        }

        private void Setup()
        {
            bool isAzure = false;

            // Read endpoint from environment, default to OpenAI API if not found
            string endpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT", EnvironmentVariableTarget.User);
            if (String.IsNullOrEmpty(endpoint))
            {
                this.Endpoint = new Uri("https://api.openai.com/v1/completions");
            }
            else
            {
                if (endpoint.Contains(".openai.azure.com"))
                {
                    isAzure = true;
                    string deployment = Environment.GetEnvironmentVariable("OPENAI_DEPLOYMENT", EnvironmentVariableTarget.User);
                    string version = Environment.GetEnvironmentVariable("OPENAI_API_VERSION", EnvironmentVariableTarget.User);
                    this.Endpoint = new Uri($"{endpoint}/openai/deployments/{deployment}/completions?api-version={version}");
                }
                else
                {
                    this.Endpoint = new Uri(endpoint);
                }
            }

            // Read Key from environment and attach to default authorization header
            string openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);
            if (isAzure)
            {
                this.Client.DefaultRequestHeaders.Add("api-key", openaiKey);
            }
            else
            {
                this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openaiKey);
            }
        }

        public async Task<string> GetCompletionAsync(string prompt)
        {
            const int retries = 3;
            string completion;
            for (var i = 0; i < retries; i++)
            {
                var request = CreateRequestMessage(prompt);
                this.Monitor.Log($"Making request to {this.Endpoint}", LogLevel.Debug);
                HttpResponseMessage response = await Client.SendAsync(request);
                this.Monitor.Log($"Received response with status code {response.StatusCode}", LogLevel.Debug);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    GptCompletionResponse responseObject = JsonSerializer.Deserialize<GptCompletionResponse>(responseContent);
                    completion = responseObject.choices[0].text;
                    if (!String.IsNullOrEmpty(completion))
                        return completion;
                }
            }
            return "Sorry, I can't talk right now.";

        }

        private HttpRequestMessage CreateRequestMessage(string prompt)
        {
            var requestData = new GptCompletionRequestData
            {
                model = GptApi.Model,
                prompt = prompt, 
                temperature = GptApi.Temperature,
                max_tokens = GptApi.MaxTokens,
                stop = ModEntry.EndMsgToken
            };
            
            string requestDataJson = JsonSerializer.Serialize(requestData);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = this.Endpoint,
                Content = new StringContent(requestDataJson, Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            return request;
        }

        // API object classes
        public class GptCompletion
        {
            public string text { get; set; }
            public int index { get; set; }
            public object logprobs { get; set; }
            public string finish_reason { get; set; }
        }

        public class GptUsage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }

        public class GptCompletionResponse
        {
            public string id { get; set; }
            public string @object { get; set; }
            public int created { get; set; }
            public string model { get; set; }
            public List<GptCompletion> choices { get; set; }
            public GptUsage usage { get; set; }
        }

        public class GptCompletionRequestData
        {
            public string model { get; set; }
            public string prompt { get; set; }
            public float temperature { get; set; }
            public int max_tokens { get; set; }
            public string stop { get; set; }
        }
    }
}