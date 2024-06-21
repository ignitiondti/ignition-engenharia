using System.Text;
using System.Text.Json;

namespace summary.api.Clients.GPT
{
    public class GeminiApi : IGeminiApi
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GeminiApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GetAnswer(string question)
        {
            var client = _httpClientFactory.CreateClient();
            var requestBody = CreateRequest(question);
            var apiKey = "AIzaSyCAKGgizRISf_-0-vKai-y0wkSdMivnuWQ";
            var serializeOptions = new JsonSerializerOptions
            {
                // This can be changed to other naming policies like SnakeCaseLower, KebabCaseLower
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, serializeOptions), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}", content);


            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var responseApi = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse, serializeOptions);
            var geminiResponseText = responseApi?.Candidates[0].Content.Parts[0].Text;

            return geminiResponseText;
        }

        public static GeminiRequest CreateRequest(string prompt)
        {
            return new GeminiRequest
            {
                Contents = new GeminiContent[]
                {
                new GeminiContent
                {
                    Role = "user",
                    Parts = new GeminiPart[]
                    {
                        new GeminiPart
                        {
                            Text = prompt 
                        }
                    }
                }
                },
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 0,
                    TopK = 1,
                    TopP = 1,
                    MaxOutputTokens = 2048,
                    StopSequences = new List<object>()
                },
                SafetySettings = new SafetySettings[]
                {
                new SafetySettings
                {
                    Category = "HARM_CATEGORY_HARASSMENT",
                    Threshold = "BLOCK_ONLY_HIGH"
                },
                new SafetySettings
                {
                    Category = "HARM_CATEGORY_HATE_SPEECH",
                    Threshold = "BLOCK_ONLY_HIGH"
                },
                new SafetySettings
                {
                    Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                    Threshold = "BLOCK_ONLY_HIGH"
                },
                new SafetySettings
                {
                    Category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                    Threshold = "BLOCK_ONLY_HIGH"
                }
                }
            };
        }

    }


    public class GeminiRequest
    {
        public GeminiContent[] Contents { get; set; }
        public GenerationConfig GenerationConfig { get; set; }
        public SafetySettings[] SafetySettings { get; set; }
    }

    public class GeminiContent
    {
        public string Role { get; set; }
        public GeminiPart[] Parts { get; set; }
    }

    public class GeminiPart
    {
        // This one interests us the most
        public string Text { get; set; }
    }

    // Two models used for configuration
    public class GenerationConfig
    {
        public int Temperature { get; set; }
        public int TopK { get; set; }
        public int TopP { get; set; }
        public int MaxOutputTokens { get; set; }
        public List<object> StopSequences { get; set; }
    }

    public class SafetySettings
    {
        public string Category { get; set; }
        public string Threshold { get; set; }
    }

    public class GeminiResponse
    {
        public Candidate[] Candidates { get; set; }
        public PromptFeedback PromptFeedback { get; set; }
    }

    public class PromptFeedback
    {
        public SafetyRating[] SafetyRatings { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
        public string FinishReason { get; set; }
        public int Index { get; set; }
        public SafetyRating[] SafetyRatings { get; set; }
    }

    public class Content
    {
        public Part[] Parts { get; set; }
        public string Role { get; set; }
    }

    public class Part
    {
        // This one interests us the most
        public string Text { get; set; }
    }

    public class SafetyRating
    {
        public string Category { get; set; }
        public string Probability { get; set; }
    }
}
