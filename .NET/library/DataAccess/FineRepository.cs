using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class FineRepository : IFineRepository
    {
        public FineRepository() { }

        public List<Fine> GetFines()
        {
            using (var context = new LibraryContext())
            {
                return context.Fines
                    .Include(x => x.Borrower)
                    .Include(x => x.BookStock)
                        .ThenInclude(x => x.Book)
                    .ToList();
            }
        }

    }
}
