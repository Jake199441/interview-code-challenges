namespace OneBeyondApi.Model
{
    public class Fine
    {
        //Generate a unique ID for each fine
        public Guid Id { get; set; } = Guid.NewGuid();

        // Add BorrowerId to the fine model to link it to the borrower
        public Guid BorrowerId { get; set; }
        public Borrower Borrower { get; set; }

        // Add BookId to the fine model to link it to the book
        public Guid BookStockID { get; set; }
        public BookStock BookStock { get; set; }

        // Add properties for the fine amount and date issued
        public decimal Amount { get; set; }
        public DateTime DateIssued { get; set; }
        public bool IsPaid { get; set; } = false;




    }
}
