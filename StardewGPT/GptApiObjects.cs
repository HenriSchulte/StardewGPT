using System;
using System.Collections.Generic;

namespace StardewGPT
{
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