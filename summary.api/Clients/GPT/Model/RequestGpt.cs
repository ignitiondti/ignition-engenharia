using System.Text.Json.Serialization;

namespace summary.api.Clients.GPT.Model
{
    public class MessageRequest
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class RequestGpt
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("messages")]
        public List<MessageRequest> Messages { get; set; }
    }
}
