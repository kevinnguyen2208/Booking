using InfoTrackBooking.Models;

namespace InfoTrackBooking.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<IEnumerable<BookingDetailsViewModel>>> GetAllBookings();
        Task<ServiceResult<Guid>> ExecuteBooking(string bookingTime, string name);
    }
}
