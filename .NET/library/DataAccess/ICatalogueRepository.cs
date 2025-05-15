using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface ICatalogueRepository
    {
        public List<BookStock> GetCatalogue();

        public List<BookStock> GetOnLoanBooks(string? borrowerName = null, string? bookTitle = null);

        public List<BookStock> SearchCatalogue(CatalogueSearch search);
    }
}
