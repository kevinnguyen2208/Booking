using InfoTrackBooking.Models;

namespace InfoTrackBooking.Repositories
{
    public interface IBookingRepository
    {
        Task<List<BookingDetails>> GetExistingBookingsByStartTime(string startTime);
        Task<Guid> SaveBooking(string startTime, string endTime, string name);
    }
}
