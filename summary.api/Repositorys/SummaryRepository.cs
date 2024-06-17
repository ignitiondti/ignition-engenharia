using Dapper;
using summary.api.Repositorys.Querys;
using summary.api.Services.Model;
using System.Data.SqlClient;

namespace summary.api.Repositorys
{
    public class SummaryRepository : ISummaryRepository
    {
        private readonly IConfiguration _configuration;

        public SummaryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SummaryModel GetLastSummary()
        {
            return new SummaryModel { Summary = "summary", CreatedDate = DateTime.Now, FileName = "fileName", Id = new Random().Next(1000) };
        }

        public SummaryModel SaveSummary(string fileName, string summary)
        {
            return new SummaryModel { Summary= summary, CreatedDate= DateTime.Now, FileName = fileName, Id = new Random().Next(1000) };
        }

        private SummaryModel GetSummary(int id)
        {
            return new SummaryModel { Summary = "summary", CreatedDate = DateTime.Now, FileName = "fileName", Id = id };

        }
    }
}
