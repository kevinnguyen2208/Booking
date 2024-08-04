using InfoTrackBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackBooking.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<BookingDetailsDto> Bookings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingDetailsDto>()
                .HasKey(b => b.BookingId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
