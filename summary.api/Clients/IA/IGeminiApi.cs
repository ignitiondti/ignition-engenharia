namespace summary.api.Clients.API
{
    public interface IGeminiApi
    {
        Task<string> GetAnswer(string question);
    }
}
