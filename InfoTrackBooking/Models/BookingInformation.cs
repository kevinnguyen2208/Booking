namespace InfoTrackBooking.Models
{
    public class BookingRequest
    {
        public string BookingTime { get; set; }
        public string Name { get; set; }
    }

    public class BookingDetailsDto
    {
        public Guid BookingId { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class BookingDetailsViewModel
    {
        public Guid BookingId { get; set; }
        public string StartTime { get; set; }
    }
}
