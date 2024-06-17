namespace summary.api.Clients.GPT
{
    public interface IGptClient
    {
        Task<string> GetAnswer(string question);
    }
}
