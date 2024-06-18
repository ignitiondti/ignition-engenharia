using summary.api.Services.Model;

namespace summary.api.Repositorys
{
    public class SummaryRepository : ISummaryRepository
    {
        public SummaryRepository()
        {
        }

        public SummaryModel GetLastSummary()
        {
            return new SummaryModel { Summary = "summary", CreatedDate = DateTime.Now, FileName = "fileName", Id = new Random().Next(1000) };
        }

        public SummaryModel SaveSummary(string fileName, string summary)
        {
            return new SummaryModel { Summary= summary, CreatedDate= DateTime.Now, FileName = fileName, Id = new Random().Next(1000) };
        }
    }
}
