using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FineController : ControllerBase
    {
        private readonly ILogger<FineController> _logger;
        private readonly IFineRepository _fineRepository;

        public FineController(ILogger<FineController> logger, IFineRepository fineRepository)
        {
            _logger = logger;
            _fineRepository = fineRepository;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Returns all fines issued to overdue books", Description = "Returns all records of fines for books that were returned as overdue")]
        [Route("GetFines")]
        public List<Fine> Get()
        {
            return _fineRepository.GetFines();
        }

    }
}
