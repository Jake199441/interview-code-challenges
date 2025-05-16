using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class ReservationRepository : IReservationRepository
    {
        public ReservationRepository()
        {
                
        }

        public string AddReservation(Reservation reservation)
        {
            using (var context = new LibraryContext())
            {
                var bookStock = context.Catalogue
                .Include(x => x.OnLoanTo)
                .FirstOrDefault(x => x.Book.Id == reservation.BookId);

                // is the book on loan
                if (bookStock?.OnLoanTo == null)
                    return "Book is currently available.";

                // get reservations list
                var reservations = context.Reservations
                    .Where(x => x.BookId == reservation.BookId)
                    .OrderBy(x => x.DateReserved)
                    .ToList();

                // do we have a reservation for this book
                var existing = reservations.FirstOrDefault(x => x.BorrowerId == reservation.BorrowerId);
                if (existing != null)
                    return $"You already have a reservation made on {existing.DateReserved:d}.";

                // we dont have a reservation get count and add user to the queue
                int queuePosition = reservations.Count + 1;

                // Add the new reservation
                reservation.DateReserved = DateTime.Now;
                context.Reservations.Add(reservation);
                context.SaveChanges();

                //tell the user if where they are in the queue
                if (queuePosition == 1)
                {
                    return "Reservation created. You are first in the queue.";
                }
                else
                {
                    return $"Book is currently reserved by others. You have been added to the queue at position {queuePosition}.";
                }
            }
        }

        public DateTime? GetNextAvailability(Guid bookId, Guid borrowerId)
        {
            using (var context = new LibraryContext())
            {
                var catalogueEntry = context.Catalogue
                .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                .Include(x => x.OnLoanTo)
                .FirstOrDefault(x => x.Book.Id == bookId);

                //check if book is on loan currently
                if (catalogueEntry?.OnLoanTo == null || catalogueEntry.LoanEndDate == null)
                    return DateTime.Now;

                var currentLoanEnd = catalogueEntry.LoanEndDate.Value;

                var reservations = context.Reservations
                    .Where(x => x.BookId == bookId)
                    .OrderBy(x => x.DateReserved)
                    .ToList();

                // Find the borrowers position in the queue if any
                var position = reservations.FindIndex(x => x.BorrowerId == borrowerId);

                // if pos = -1 no reservations then tell the user date when book is free
                if (position == -1)
                    return currentLoanEnd;

                // calculate next avaible date based on position in queue mutipled by 20 days
                return currentLoanEnd.AddDays(20 * position);
            }

            
        }



    }
}
