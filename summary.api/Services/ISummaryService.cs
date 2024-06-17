using summary.api.Services.Model;

namespace summary.api.Services
{
    public interface ISummaryService
    {
        Task<SummaryModel> CreateSummary(IFormFile file);

        SummaryModel GetLastSummary();
    }
}
