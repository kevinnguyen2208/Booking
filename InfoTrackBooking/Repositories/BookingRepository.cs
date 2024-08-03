using InfoTrackBooking.Data;
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

        public async Task<List<BookingDetails>> GetExistingBookingsByStartTime(string startTime)
        {
            List<BookingDetails> bookings = await _context.Bookings.ToListAsync();
            return bookings.Where(f => f.StartTime == startTime).ToList();
        }

        public async Task<Guid> SaveBooking(string startTime, string endTime, string name)
        {
            BookingDetails booking = new BookingDetails()
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
