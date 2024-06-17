using summary.api.Services.Model;

namespace summary.api.Repositorys
{
    public interface ISummaryRepository
    {
        SummaryModel SaveSummary(string fileName, string summary);

        SummaryModel GetLastSummary();
    }
}
