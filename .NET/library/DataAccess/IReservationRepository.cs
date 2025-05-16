using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface IReservationRepository
    {
        string AddReservation(Reservation reservation);
        DateTime? GetNextAvailability(Guid bookId, Guid borrowerId);
    }
}
