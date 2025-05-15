using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

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
    }
}
