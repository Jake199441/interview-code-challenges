namespace OneBeyondApi.Model
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BorrowerId { get; set; }
        public Borrower Borrower { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public DateTime DateReserved { get; set; } = DateTime.Now;
    }
}
