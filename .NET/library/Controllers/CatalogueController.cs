using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogueController : ControllerBase
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly ICatalogueRepository _catalogueRepository;

        public CatalogueController(ILogger<CatalogueController> logger, ICatalogueRepository catalogueRepository)
        {
            _logger = logger;
            _catalogueRepository = catalogueRepository;   
        }

        [HttpGet]
        [Route("GetCatalogue")]
        public IList<BookStock> Get()
        {
            return _catalogueRepository.GetCatalogue();
        }

        [HttpPost]
        [Route("SearchCatalogue")]
        public IList<BookStock> Post(CatalogueSearch search)
        {
            return _catalogueRepository.SearchCatalogue(search);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get/query all books that are currently on loan",Description = "Returns all of the books with active loan's or filter by book title and borrower with active loan's")]
        [Route("OnLoan")]
        [ProducesResponseType(typeof(IList<BookStock>), 200)]
        public IList<BookStock> GetLoanedBooks([FromQuery] string? borrowerName, [FromQuery] string? bookTitle) 
        {
            return _catalogueRepository.GetOnLoanBooks(borrowerName, bookTitle);
        }
    }
}