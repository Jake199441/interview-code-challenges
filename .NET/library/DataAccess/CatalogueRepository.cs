using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;
using System.Net;

namespace OneBeyondApi.DataAccess
{
    public class CatalogueRepository : ICatalogueRepository
    {
        public CatalogueRepository()
        {
        }
        public List<BookStock> GetCatalogue()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .ToList();
                return list;
            }
        }

        public List<BookStock> GetOnLoanBooks(string? borrowerName = null, string? bookTitle = null) 
        {
            using (var context = new LibraryContext())
            {
                //Allow the user to get all books on loan or filter by the book title/borrowers name
                var query = context.Catalogue
                    .Include(x => x.Book)
                        .ThenInclude(x => x.Author)
                        .Include(x => x.OnLoanTo)
                        .Where(x => x.OnLoanTo != null)
                        .AsQueryable();

                if (!string.IsNullOrEmpty(borrowerName)) 
                {
                    query = query.Where(b => b.OnLoanTo.Name.Contains(borrowerName));
                }

                if (!string.IsNullOrEmpty(bookTitle))
                {
                    query = query.Where(b => b.Book.Name.Contains(bookTitle));
                }

                return query.ToList();
            }
        }

        public BookStock? ReturnBook(Guid bookId)
        {
            using (var context = new LibraryContext())
            {
                var book = context.Catalogue
                    .Include(x => x.Book)
                     .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .FirstOrDefault(b => b.Book.Id == bookId && b.OnLoanTo != null);

                if (book != null)
                {
                    // Check if the book is overdue and apply a fine if necessary
                    if (book.LoanEndDate.HasValue && book.LoanEndDate < DateTime.Now) 
                    {
                        //find out the fine amount
                        decimal fineAmount = CalculateFine(book.LoanEndDate.Value, DateTime.Now);

                        // if the fine is greater than 0 we apply the fine
                        if(fineAmount > 0) 
                        {
                            var fine = new Fine
                            {
                                BorrowerId = book.OnLoanTo.Id,
                                BookStockID = book.Id,
                                DateIssued = DateTime.Now,
                                Amount = CalculateFine(book.LoanEndDate.Value, DateTime.Now),
                            };
                            context.Fines.Add(fine);
                        }
                    }
                    book.OnLoanTo = null;
                    book.LoanEndDate = null;
                    context.SaveChanges();
                }

                return book;
            }
        }

        public List<BookStock> SearchCatalogue(CatalogueSearch search)
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .AsQueryable();

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Author)) {
                        list = list.Where(x => x.Book.Author.Name.Contains(search.Author));
                    }
                    if (!string.IsNullOrEmpty(search.BookName)) {
                        list = list.Where(x => x.Book.Name.Contains(search.BookName));
                    }
                }
                    
                return list.ToList();
            }
        }
        /* After looking around on how most libarys operate i have created a method to calculate the fine based on the number of days it is overdue
         * based on what i could find on library's policies they charge a fixed ammount per day then cap it at a maximum amount
         */
        private decimal CalculateFine(DateTime loanEndDate, DateTime returnDate)
        {
            // Find out how many days late the book is
            int daysLate = (returnDate - loanEndDate).Days;

            if (daysLate <= 0)
            {
                return 0; 
            }

            decimal fine = daysLate * 0.50m; // £0.50 per day
            return fine > 5.00m ? 5.00m : fine; // Cap the fine at £5.00
        }

    }
}
