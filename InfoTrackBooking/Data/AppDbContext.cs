using InfoTrackBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackBooking.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BookingDetails> Bookings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
