using InfoTrackBooking.Helpers;
using InfoTrackBooking.Models;
using InfoTrackBooking.Repositories;

namespace InfoTrackBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        /// <summary>
        /// validate booking parameters before saving
        /// </summary>
        public async Task<ServiceResult<Guid>> ExecuteBooking(string bookingTime, string name)
        {
            //validate booking time or name is not null or empty
            if (string.IsNullOrEmpty(bookingTime) || string.IsNullOrEmpty(name))
            {
                return ServiceResult<Guid>.CreateErrorMessage("Booking time and/or name must not be empty or null.");
            }

            //validate booking time to be valid hh:mm format
            bool isValidTimeFormat = TimeHelper.CheckTimeFormat(bookingTime);
            bool isValidTime = TimeSpan.TryParse(bookingTime, out var startTime);
            if (!isValidTimeFormat || !isValidTime) 
            {
                return ServiceResult<Guid>.CreateErrorMessage("Booking time must be in hh:mm format.");
            }

            //validate booking hours
            if (!ValidateBookingTime(startTime))
            {
                return ServiceResult<Guid>.CreateErrorMessage("Booking time must be between 09:00 and 16:00, or the minutes must be either 00 or 30.", ValidationTypes.InvalidTime);
            }

            //validate booking time reservation
            if (!await ValidateReservation(bookingTime))
            {
                return ServiceResult<Guid>.CreateErrorMessage("Booking time is fully booked.", ValidationTypes.ReservedTime);
            }

            //save booking when all validations have been verified
            Guid id = await _bookingRepository.SaveBooking(bookingTime, TimeHelper.CreateEndTime(startTime), name);
            return new ServiceResult<Guid>(id);
        }

        /// <summary>
        /// Validate booking time to be in range of business hours and of minutes 0 or 30
        /// </summary>
        private static bool ValidateBookingTime(TimeSpan startTime)
        {
            TimeSpan startOfBusinessHours = new(9, 0, 0);
            TimeSpan endOfBusinessHours = new(16, 0, 0);
            if (startTime < startOfBusinessHours || startTime > endOfBusinessHours //business hours i.e.from 9 AM to cut-off time 4 PM
            || !(startTime.Minutes == 0 || startTime.Minutes == 30)) //of minutes 0 or 30
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate booking time to be available i.e. only at maximum 3 bookings of that start time exist
        /// </summary>
        private async Task<bool> ValidateReservation(string startTime)
        {
            List<BookingDetails> exsitingBookings = await _bookingRepository.GetExistingBookingsByStartTime(startTime);
            if (exsitingBookings == null || exsitingBookings.Count < 4)
            {
                return true;
            }
            return false;
        }
    }
}
