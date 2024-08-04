using InfoTrackBooking.Models;

namespace InfoTrackBooking.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingDetailsViewModel>> GetAllBookings();
        Task<IEnumerable<BookingDetailsDto>> GetAllBookingsByStartTime(string startTime);
        Task<Guid> SaveBooking(string startTime, string endTime, string name);
    }
}
