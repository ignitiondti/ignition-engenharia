using summary.api.Clients.API;
using summary.api.Clients.Files;
using summary.api.Exceptions;
using summary.api.Repositorys;
using summary.api.Services.Model;


namespace summary.api.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly IGeminiApi _geminiApi;

        private readonly ISummaryRepository _summaryRepository;

        private readonly IFileReader _fileReader;

        private readonly int MAX_FILE_SIZE = 1000000; // 1MB

        public SummaryService(
            ISummaryRepository summaryRepository,
            IGeminiApi geminiApi,
            IFileReader fileReader)
        {
            _summaryRepository = summaryRepository;
            _geminiApi = geminiApi;
            _fileReader = fileReader;
        }

        public async Task<SummaryModel> CreateSummary(IFormFile file)
        {
            ValidateFile(file);

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            using var stream = file.OpenReadStream();

            string fileContent = fileExtension switch
            {
                ".txt" => ReadTxtFile(stream),
                ".doc" => ReadDocFile(stream),
                ".docx" => ReadDocxFile(stream),
                _ => throw new ServiceException(ErrorConstants.INVALID_FILE_FORMAT),
            };

            //Fazer chamada do chatgpt usando o _gptClient e armazenar o response na variavel summary
            string summary = string.Empty;
        
            try
            {
                return _summaryRepository.SaveSummary(file.FileName, summary);
            }
            catch (Exception)
            {
                throw new ServiceException(ErrorConstants.FAILURE_SAVE_SUMMARY);
            }
        }

        private void ValidateFile(IFormFile file)
        {
            // Fazer Validações da entrada descritas na história
            throw new NotImplementedException();
        }

        private string ReadTxtFile(Stream stream)
        {
            try
            {
                return _fileReader.ReadTxtFile(stream);
            }
            catch (Exception)
            {
                throw new ServiceException(ErrorConstants.FAILURE_READ_TXT_FILE);
            }

        }

        private string ReadDocFile(Stream stream)
        {
            try
            {
                return _fileReader.ReadDocFile(stream);
            }
            catch (Exception)
            {
                throw new ServiceException(ErrorConstants.FAILURE_READ_DOC_FILE);
            }

        }

        private string ReadDocxFile(Stream stream)
        {
            try
            {
                return _fileReader.ReadDocxFile(stream);

            }
            catch (Exception)
            {
                throw new ServiceException(ErrorConstants.FAILURE_READ_DOCX_FILE);
            }

        }

        public SummaryModel GetLastSummary()
        {
            try
            {
                return _summaryRepository.GetLastSummary();
            }
            catch (Exception)
            {
                throw new ServiceException(ErrorConstants.FAILURE_FETCH_LAST_SUMMARY);
            }
        }
    }
}
