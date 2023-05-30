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

        // Only supporting OpenAI API
        public Uri Endpoint = new Uri("https://api.openai.com/v1/chat/completions");

        public GptApi(IMonitor monitor)
        {
            this.Client = new HttpClient();
            this.Monitor = monitor;

            this.Setup();
        }

        private void Setup()
        {
            // Read Key from environment and attach to default authorization header
            string openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User);
            this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openaiKey);
        }

        public async Task<string> GetCompletionAsync(List<GptMessage> messages)
        {
            string completion;
            var request = CreateRequestMessage(messages);
            this.Monitor.Log($"Making request to {this.Endpoint}", LogLevel.Debug);
            HttpResponseMessage response = await Client.SendAsync(request);
            this.Monitor.Log($"Received response with status code {response.StatusCode}", LogLevel.Debug);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                GptCompletionResponse responseObject = JsonSerializer.Deserialize<GptCompletionResponse>(responseContent);
                completion = responseObject.choices[0].message.content;
                this.Monitor.Log($"Received message {responseObject.choices[0].message}", LogLevel.Debug);
                if (!String.IsNullOrEmpty(completion))
                    return completion;
            }
            return "Sorry, I can't talk right now.";
        }

        private HttpRequestMessage CreateRequestMessage(List<GptMessage> messages)
        {
            var requestData = new GptCompletionRequestData
            {
                model = GptApi.Model,
                messages = messages, 
                temperature = GptApi.Temperature,
                max_tokens = GptApi.MaxTokens
            };
            
            string requestDataJson = JsonSerializer.Serialize(requestData);
            this.Monitor.Log($"Creating request with content {requestDataJson}", LogLevel.Debug);

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
            public GptMessage message { get; set; }
            public int index { get; set; }
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
            public List<GptMessage> messages { get; set; }
            public float temperature { get; set; }
            public int max_tokens { get; set; }
            public string stop { get; set; }
        }
    }
}