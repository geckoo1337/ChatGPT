using Models; // models for ChatGPT
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
// json
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models
{
    class ChatGptChoice
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("logprobs")]
        public object Logprobs { get; set; }

        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }
    }

    class ChatGptUsage
    {
        [JsonProperty("promptTokens")]
        public int PromptTokens { get; set; }

        [JsonProperty("completionTokens")]
        public int CompletionTokens { get; set; }

        [JsonProperty("totalTokens")]
        public int TotalTokens { get; set; }
    }

    class ChatGptError
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("Param")]
        public object LogParamprobs { get; set; }

        [JsonProperty("code")]
        public object code { get; set; }
    }

    class ChatGptResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("choices")]
        public List<ChatGptChoice> Choices { get; set; }

        [JsonProperty("usage")]
        public ChatGptUsage Usage { get; set; }

        [JsonProperty("error")]
#pragma warning disable CS8632
        public ChatGptError? Error { get; set; }
#pragma warning restore CS8632
    }

    class ChatGptRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("max_tokens")]
        public int Max_tokens { get; set; }

        [JsonPropertyName("top_p")]
        public int Top_p { get; set; }

        [JsonPropertyName("frequency_penalty")]
        public int Frequency_penalty { get; set; }

        [JsonPropertyName("presence_penalty")]
        public int Presence_penalty { get; set; }

        public ChatGptRequest(string prompt)
        {
            this.Model = "text-davinci-003";
            this.Prompt = prompt;
            this.Temperature = 0f;
            this.Max_tokens = 150;
            this.Top_p = 1;
            this.Frequency_penalty = 0;
            this.Presence_penalty = 0;
        }
    }
}

namespace gptBot
{
    class ChatBot
    {
        static async Task Main()
        {
            var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            DateTime date = DateTime.Now;
            Console.WriteLine("{0}", date.ToString("yyyy-MM-dd" + "\t" + "HH:mm"));

            Console.WriteLine("##################################################################");
            Console.WriteLine("### Hello, I'm ChatGPT in console version written in C# .NET 5 ###");
            Console.WriteLine("### This is just a little test playing with ChatGPT for fun... ###");
            Console.WriteLine("### You need to put your own private API key to the line 171   ###");
            Console.WriteLine("##################################################################");
            Console.WriteLine();
            Console.WriteLine("What's your first question?");
            Console.WriteLine();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                ChatGptRequest chatGptRequest = YourRequest();
                StringContent jsonContent = CreateJsonContent(chatGptRequest, jsonSerializerOptions);
                string yourResponse = await YourResponseAsync(jsonContent, jsonSerializerOptions);
                // display answer
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.CursorTop--;
                Console.WriteLine(yourResponse + "\n");
            }
        }

        private static ChatGptRequest YourRequest()
        {
            var prompt = Console.ReadLine();
            ChatGptRequest chatGptRequest = new(prompt);
            // command to close the conversation
            if (prompt == "exit")
                System.Environment.Exit(0);

            return chatGptRequest;
        }

        private static StringContent CreateJsonContent(ChatGptRequest chatGptRequest, JsonSerializerOptions jsonSerializerOptions)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(chatGptRequest, jsonSerializerOptions);
            StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            return jsonContent;
        }

        private static async Task<string> YourResponseAsync(StringContent jsonContent, JsonSerializerOptions jsonSerializerOptions)
        {
            var openaiUrl = "https://api.openai.com/v1/completions";
            var openaiApiKey = "PUT_YOUR_PRIVATE_KEY_HERE";
            var httpClient = new HttpClient();
            // new http request
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openaiApiKey);
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(openaiUrl, jsonContent);
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            if (responseContent != null)
            {
                ChatGptResponse chatGptResponse = System.Text.Json.JsonSerializer.Deserialize<ChatGptResponse>(responseContent, jsonSerializerOptions);
                return chatGptResponse.Choices != null ? chatGptResponse.Choices[0].Text : "No reply!";
            }
            else 
                return "No reply!";
        }
    }
}
