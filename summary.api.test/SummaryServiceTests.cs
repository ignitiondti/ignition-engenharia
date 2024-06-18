using summary.api.Clients.Files;
using summary.api.Clients.GPT;
using summary.api.Exceptions;
using summary.api.Repositorys;
using summary.api.Services;
using summary.api.Services.Model;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using Xunit;

namespace summary.api.test
{
    public class SummaryServiceTests
    {
        private readonly Mock<ISummaryRepository> _summaryRepositoryMock;
        private readonly Mock<IGptClient> _gptClientMock;
        private readonly Mock<IFileReader> _fileReaderMock;
        private readonly SummaryService _summaryService;


        public SummaryServiceTests()
        {
            _summaryRepositoryMock = new Mock<ISummaryRepository>();
            _gptClientMock = new Mock<IGptClient>();
            _fileReaderMock = new Mock<IFileReader>();
            _summaryService = new SummaryService(_summaryRepositoryMock.Object, _gptClientMock.Object, _fileReaderMock.Object);

        }

        #region CreateSummary
        [Fact]
        public async Task CreateSummary_Valid_Txt_File_SummaryGeneratedAndSaved()
        {
            // Arrange
            var content = "This is the content of the file.";
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            fileMock.Setup(f => f.FileName).Returns("example.txt");
            fileMock.Setup(f => f.OpenReadStream()).Returns(contentStream);

            var summaryText = "Summary of the file content";
            var summaryMock = new SummaryModel
            {
                Summary = summaryText,
                FileName = fileMock.Object.FileName,
                CreatedDate = DateTime.Now,
                Id = 1
            };
            _fileReaderMock.Setup(f => f.ReadTxtFile(It.IsAny<Stream>())).Returns(content);
            _gptClientMock.Setup(g => g.GetAnswer(It.IsAny<string>())).ReturnsAsync(summaryText);
            _summaryRepositoryMock.Setup(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>())).Returns(summaryMock);

            // Act
            var summaryResponse = await _summaryService.CreateSummary(fileMock.Object);

            // Assert
            Assert.Equal(summaryMock, summaryResponse);
            _fileReaderMock.Verify(f => f.ReadTxtFile(It.IsAny<Stream>()), Times.Once);
            _gptClientMock.Verify(g => g.GetAnswer(It.Is<string>(s => s.Equals($"Summary: {content}"))), Times.Once);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.Is<string>(s => s.Equals(fileMock.Object.FileName)), It.Is<string>(s => s.Equals(summaryText))), Times.Once);

        }

        // CreateSummary_Valid_Doc_File_SummaryGeneratedAndSaved

        [Fact]
        public async Task CreateSummary_Valid_Docx_File_SummaryGeneratedAndSaved()
        {
            // Arrange
            var content = "This is the content of the file.";
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1000000);
            fileMock.Setup(f => f.FileName).Returns("example.docx");
            fileMock.Setup(f => f.OpenReadStream()).Returns(contentStream);

            var summaryText = "Summary of the file content";
            var summaryMock = new SummaryModel
            {
                Summary = summaryText,
                FileName = fileMock.Object.FileName,
                CreatedDate = DateTime.Now,
                Id = 1
            };
            _fileReaderMock.Setup(f => f.ReadDocxFile(It.IsAny<Stream>())).Returns(content);
            _gptClientMock.Setup(g => g.GetAnswer(It.IsAny<string>())).ReturnsAsync(summaryText);
            _summaryRepositoryMock.Setup(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>())).Returns(summaryMock);

            // Act
            var summaryResponse = await _summaryService.CreateSummary(fileMock.Object);

            // Assert
            Assert.Equal(summaryMock, summaryResponse);
            _fileReaderMock.Verify(f => f.ReadDocxFile(It.IsAny<Stream>()), Times.Once);
            _gptClientMock.Verify(g => g.GetAnswer(It.Is<string>(s => s.Equals($"Summary: {content}"))), Times.Once);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.Is<string>(s => s.Equals(fileMock.Object.FileName)), It.Is<string>(s => s.Equals(summaryText))), Times.Once);

        }

        [Fact]
        public async Task CreateSummary_InvalidFile_ThrowsServiceException()
        {
            // Arrange
            IFormFile? invalidFile = null;

            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(invalidFile));
            Assert.Equal(ErrorConstants.INVALID_FILE, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadTxtFile(It.IsAny<Stream>()), Times.Never);
            _fileReaderMock.Verify(f => f.ReadDocFile(It.IsAny<Stream>()), Times.Never);
            _fileReaderMock.Verify(f => f.ReadDocxFile(It.IsAny<Stream>()), Times.Never);
            _gptClientMock.Verify(g => g.GetAnswer(It.IsAny<string>()), Times.Never);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateSummary_InvalidFileFormat_ThrowsServiceException()
        {
            // Arrange
            var invalidFile = new Mock<IFormFile>();
            invalidFile.Setup(f => f.Length).Returns(100);  // Assume valid length
            invalidFile.Setup(f => f.FileName).Returns("invalid_file.invalid");


            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(invalidFile.Object));
            Assert.Equal(ErrorConstants.INVALID_FILE_FORMAT, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadTxtFile(It.IsAny<Stream>()), Times.Never);
            _fileReaderMock.Verify(f => f.ReadDocFile(It.IsAny<Stream>()), Times.Never);
            _fileReaderMock.Verify(f => f.ReadDocxFile(It.IsAny<Stream>()), Times.Never);
            _gptClientMock.Verify(g => g.GetAnswer(It.IsAny<string>()), Times.Never);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
        
        //CreateSummary_InvalidSizeFile_ThrowsServicesException

        [Theory]
        [InlineData("invalid_file")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateSummary_InvalidFileNameEmpity_ThrowsServicesException(string fileName)
        {
            // Arrange
            var invalidFile = new Mock<IFormFile>();
            invalidFile.Setup(f => f.Length).Returns(100);
            invalidFile.Setup(f => f.FileName).Returns(fileName);

            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(invalidFile.Object));
            Assert.Equal(ErrorConstants.INVALID_FILE_NAME, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadTxtFile(It.IsAny<Stream>()), Times.Never);
            _fileReaderMock.Verify(f => f.ReadDocFile(It.IsAny<Stream>()), Times.Never);
            _fileReaderMock.Verify(f => f.ReadDocxFile(It.IsAny<Stream>()), Times.Never);
            _gptClientMock.Verify(g => g.GetAnswer(It.IsAny<string>()), Times.Never);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateSummary_ReadTxtFileFailure_ThrowsServiceException()
        {
            // Arrange
            var invalidTxtFile = new Mock<IFormFile>();
            invalidTxtFile.Setup(f => f.Length).Returns(100);
            invalidTxtFile.Setup(f => f.FileName).Returns("invalid_file.txt");
            _fileReaderMock.Setup(f => f.ReadTxtFile(It.IsAny<Stream>())).Throws(new Exception());


            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(invalidTxtFile.Object));
            Assert.Equal(ErrorConstants.FAILURE_READ_TXT_FILE, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadTxtFile(It.IsAny<Stream>()), Times.Once);
            _gptClientMock.Verify(g => g.GetAnswer(It.IsAny<string>()), Times.Never);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateSummary_ReadDocFileFailure_ThrowsServiceException()
        {
            // Arrange
            var invalidTxtFile = new Mock<IFormFile>();
            invalidTxtFile.Setup(f => f.Length).Returns(100);
            invalidTxtFile.Setup(f => f.FileName).Returns("invalid_file.doc");
            _fileReaderMock.Setup(f => f.ReadDocFile(It.IsAny<Stream>())).Throws(new Exception());


            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(invalidTxtFile.Object));
            Assert.Equal(ErrorConstants.FAILURE_READ_DOC_FILE, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadDocFile(It.IsAny<Stream>()), Times.Once);
            _gptClientMock.Verify(g => g.GetAnswer(It.IsAny<string>()), Times.Never);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateSummary_ReadDocxFileFailure_ThrowsServiceException()
        {
            // Arrange
            var invalidTxtFile = new Mock<IFormFile>();
            invalidTxtFile.Setup(f => f.Length).Returns(100);
            invalidTxtFile.Setup(f => f.FileName).Returns("invalid_file.docx");
            _fileReaderMock.Setup(f => f.ReadDocxFile(It.IsAny<Stream>())).Throws(new Exception());

            //Act 
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(invalidTxtFile.Object));

            //Assert
            Assert.Equal(ErrorConstants.FAILURE_READ_DOCX_FILE, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadDocxFile(It.IsAny<Stream>()), Times.Once);
            _gptClientMock.Verify(g => g.GetAnswer(It.IsAny<string>()), Times.Never);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task CreateSummary_GptClientFailure_ThrowsServiceException()
        {
            // Arrange
            var content = "This is the content of the file.";
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var validTxtFile = new Mock<IFormFile>();
            validTxtFile.Setup(f => f.Length).Returns(100);
            validTxtFile.Setup(f => f.FileName).Returns("valid_file.txt");
            validTxtFile.Setup(f => f.OpenReadStream()).Returns(contentStream);

            _fileReaderMock.Setup(f => f.ReadTxtFile(It.IsAny<Stream>())).Returns(content);

            _gptClientMock.Setup(g => g.GetAnswer(It.IsAny<string>()))
                .Throws(new Exception());

            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(validTxtFile.Object));
            Assert.Equal(ErrorConstants.FAILURE_GPT_API, exception.Error.Code);
            _fileReaderMock.Verify(f => f.ReadTxtFile(It.IsAny<Stream>()), Times.Once);
            _gptClientMock.Verify(g => g.GetAnswer(It.Is<string>(s => s.Equals($"Summary: {content}"))), Times.Once);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateSummary_SaveSummaryFailure_ThrowsServiceException()
        {
            // Arrange
            var content = "This is the content of the file.";
            var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var validTxtFile = new Mock<IFormFile>();
            validTxtFile.Setup(f => f.Length).Returns(100);  // Assume valid length
            validTxtFile.Setup(f => f.FileName).Returns("valid_file.txt");
            validTxtFile.Setup(f => f.OpenReadStream()).Returns(contentStream);

            var summaryText = "Summary of the file content";
            _gptClientMock.Setup(g => g.GetAnswer(It.IsAny<string>())).ReturnsAsync(summaryText);
            _fileReaderMock.Setup(f => f.ReadTxtFile(It.IsAny<Stream>())).Returns(content);

            _summaryRepositoryMock.Setup(r => r.SaveSummary(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());  // Simulate repository failure

            // Act and Assert
            var exception = await Assert.ThrowsAsync<ServiceException>(() => _summaryService.CreateSummary(validTxtFile.Object));

            Assert.Equal(ErrorConstants.FAILURE_SAVE_SUMMARY, exception.Error.Code);
            _gptClientMock.Verify(g => g.GetAnswer(It.Is<string>(s => s.Equals($"Summary: {content}"))), Times.Once);
            _summaryRepositoryMock.Verify(s => s.SaveSummary(It.Is<string>(s => s.Equals(validTxtFile.Object.FileName)), It.Is<string>(s => s.Equals(summaryText))), Times.Once);

        }
        #endregion

        #region GetLatestSummary

        [Fact]
        public void GetLastSummary_Success_ReturnsSummary()
        {
            // Arrange
            var expectedSummary = new SummaryModel
            {
                Id = 1,
                Summary = "Summary",
                FileName = "File.txt",
                CreatedDate = DateTime.Now
            };
            _summaryRepositoryMock.Setup(r => r.GetLastSummary())
                .Returns(expectedSummary);

            // Act
            var result = _summaryService.GetLastSummary();

            // Assert
            Assert.Equal(expectedSummary, result);
            _summaryRepositoryMock.Verify(r => r.GetLastSummary(), Times.Once);
        }

        [Fact]
        public void GetLastSummary_RepositoryFailure_ThrowsServiceException()
        {
            // Arrange
            var exceptionMessage = "Simulated repository failure.";
            _summaryRepositoryMock.Setup(r => r.GetLastSummary())
                .Throws(new Exception(exceptionMessage));

            // Act and Assert
            var exception = Assert.Throws<ServiceException>(() => _summaryService.GetLastSummary());
            Assert.Equal(ErrorConstants.FAILURE_FETCH_LAST_SUMMARY, exception.Code);
            _summaryRepositoryMock.Verify(r => r.GetLastSummary(), Times.Once);
        }
        #endregion


    }
}
