
using summary.api.Exceptions;
using summary.api.Models;
using summary.api.Services;
using summary.api.Services.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace summary.api.Controllers
{
    [Route("api/summary")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _summaryService;

        public SummaryController(ISummaryService summaryService) => _summaryService = summaryService;
        /// <summary>
        /// Method responsible for creating file summary (txt,doc, docx)
        /// </summary>
        [HttpPost]
        [SwaggerRequestExample(typeof(IFormFile), typeof(IFormFile))]
        [ProducesResponseType(typeof(MessageError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SummaryModel), StatusCodes.Status201Created)]

        public async Task<IActionResult> Post(IFormFile file)
        {
            try
            {
                var response = await _summaryService.CreateSummary(file);
                return CreatedAtAction(nameof(GetSummatyById), new { id = response.Id }, response);
            }
            catch(ServiceException ex)
            {
                return BadRequest(ex.Error);
            }
        }
        /// <summary>
        ///     Method responsible for bringing the last summary created
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(MessageError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SummaryModel), StatusCodes.Status200OK)]
        [HttpGet("latest")]
        public IActionResult Get()
        {
            try
            {
                var summary = _summaryService.GetLastSummary();
                return Ok(summary);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        ///     Method responsible for bringing the summary filtering by its id
        /// </summary>
        [HttpGet("{id}", Name = "GetSummatyById")]
        public IActionResult GetSummatyById(int id)
        {
           throw new NotImplementedException();
        }
    }
}
