using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IReservationRepository _reservationRepo;

        public ReservationController(ILogger<ReservationController> logger, IReservationRepository reservationRepo)
        {
            _logger = logger;
            _reservationRepo = reservationRepo;
        }

        [HttpPost("ReserveBook")]
        [SwaggerOperation(Summary = "Allows the user to reserve a book", Description = "Checks to see if the book is currently on loan and if there is already reservation for the borrower/other users")]
        [ProducesResponseType(typeof(string), 200)]
        public ActionResult<string> ReserveBook([FromQuery] Guid bookId, [FromQuery] Guid borrowerId)
        {
            var reservation = new Reservation
            {
                BookId = bookId,
                BorrowerId = borrowerId
            };

            var resultMessage = _reservationRepo.AddReservation(reservation);
            return Ok(resultMessage);
        }

        [HttpGet("NextAvailability")]
        [SwaggerOperation(Summary = "Informs the user of the next available date for book to be reserved", Description = "Checks to see when the book is next available and returns a date")]
        [ProducesResponseType(typeof(DateTime?), 200)]
        public ActionResult<DateTime?> GetNextAvailability([FromQuery] Guid bookId, [FromQuery] Guid borrowerId)
        {
            var date = _reservationRepo.GetNextAvailability(bookId, borrowerId);
            return Ok(date);
        }
    }
}