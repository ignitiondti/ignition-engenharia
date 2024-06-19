namespace summary.api.Clients.GPT
{
    public interface IGeminiApi
    {
        Task<string> GetAnswer(string question);
    }
}
