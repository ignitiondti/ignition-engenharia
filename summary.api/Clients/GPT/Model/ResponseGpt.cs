using System.Text.Json.Serialization;

namespace summary.api.Clients.GPT.Model
{
    public class ResponseGpt
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public MessageResponse Message { get; set; }
    }

    public class MessageResponse
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
