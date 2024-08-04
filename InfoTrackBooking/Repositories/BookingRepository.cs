using InfoTrackBooking.Data;
using InfoTrackBooking.Mappers;
using InfoTrackBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingDetailsViewModel>> GetAllBookings()
        {
            IEnumerable<BookingDetailsDto> bookings = await _context.Bookings.ToArrayAsync();
            return bookings.ToViewModel();
        }

        public async Task<IEnumerable<BookingDetailsDto>> GetAllBookingsByStartTime(string startTime)
        {
            IEnumerable<BookingDetailsDto> bookings = await _context.Bookings.Where(f => f.StartTime == startTime).ToArrayAsync();
            return bookings;
        }

        public async Task<Guid> SaveBooking(string startTime, string endTime, string name)
        {
            BookingDetailsDto booking = new BookingDetailsDto()
            {
                BookingId = Guid.NewGuid(),
                Name = name,
                StartTime = startTime,
                EndTime = endTime
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking.BookingId;
        }
    }
}
