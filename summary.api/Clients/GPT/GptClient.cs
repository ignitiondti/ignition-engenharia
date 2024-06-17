using summary.api.Clients.GPT.Model;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace summary.api.Clients.GPT
{
    public class GptClient : IGptClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GptClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GetAnswer(string question)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");

            var payload = new RequestGpt
            {
                Model = "gpt-4",
                Messages = new List<MessageRequest>
                {
                    new MessageRequest
                    {
                        Role = "user",
                        Content = $"Summary: {question}"
                    }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }

            var responseApi = JsonSerializer.Deserialize<ResponseGpt>(jsonResponse);
            return responseApi.Choices.FirstOrDefault()?.Message.Content;
        }

    }
}
