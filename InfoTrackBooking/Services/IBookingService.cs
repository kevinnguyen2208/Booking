using InfoTrackBooking.Models;

namespace InfoTrackBooking.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<Guid>> ExecuteBooking(string bookingTime, string name);
    }
}
