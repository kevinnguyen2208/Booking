using InfoTrackBooking.Mappers;
using InfoTrackBooking.Models;
using InfoTrackBooking.Repositories;
using InfoTrackBooking.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.UnitTests
{
    public class BookingServiceTest
    {
        private readonly ServiceCollection SC;
        private readonly ServiceProvider SP;
        private readonly IBookingService _service;

        public BookingServiceTest()
        {
            SC = new ServiceCollection();
            SC.AddScoped<IBookingService, BookingService>();
            SetupBookingRepository();
            SP = SC.BuildServiceProvider();
            _service = SP.GetRequiredService<IBookingService>();
        }

        private List<BookingDetailsDto> BookingStorage = new List<BookingDetailsDto>();
        Guid BookingId = Guid.NewGuid();
        private void SetupBookingRepository()
        {
            Mock<IBookingRepository> mock = new Mock<IBookingRepository>(MockBehavior.Strict);

            _ = mock.Setup(s => s.GetAllBookings())
                .ReturnsAsync(BookingStorage.ToViewModel);

            _ = mock.Setup(s => s.GetAllBookingsByStartTime(It.IsAny<string>()))
                .ReturnsAsync((string bookingTime) =>
                {
                    return BookingStorage.Where(f => f.StartTime == bookingTime).ToList();
                });

            _ = mock.Setup(s => s.SaveBooking(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string bookingTime, string endTime, string name) =>
                {
                    BookingDetailsDto booking = new BookingDetailsDto()
                    {
                        BookingId = BookingId,
                        Name = name,
                        StartTime = bookingTime,
                        EndTime = endTime
                    };

                    BookingStorage.Add(booking);
                    return BookingId;
                });

            SC.AddScoped(_ => mock.Object);
        }

        private void GenerateDummyReservationByTime(string time, string name, int reservationCount)
        {
            for (int i = 0; i < reservationCount; i++)
            {
                BookingStorage.Add(new BookingDetailsDto()
                {
                    BookingId = Guid.NewGuid(),
                    Name = name,
                    StartTime = time
                });
            }
        }

        /// <summary>
        /// Validate booking time and name
        /// Requirements:
        /// - Booking Time must be in correct hh:mm format
        /// - Booking time and name must not be null or empty 
        /// </summary>
        [Theory]
        [InlineData("", "John", false)] //empty time - invalid
        [InlineData(null, "John", false)] //null time - invalid
        [InlineData("39:30", "John", false)] //invalid time - invalid
        [InlineData("09:30", "", false)] //empty name - invalid
        [InlineData("09:30", null, false)] //null name - invalid
        [InlineData("9:30", "John", false)] //incorrect time format - invalid
        [InlineData("09:30", "John", true)] //correct time format and name - valid
        [InlineData("14:30", "John", true)] //correct time format and name - valid
        public async void ValidateParameters(string time, string name, bool isValid)
        {
            ServiceResult<Guid> serviceResult = await _service.ExecuteBooking(time, name);
            if (isValid)
            {
                Assert.Equal(ValidationTypes.None, serviceResult.Validation);
            }
            else
            {
                Assert.Equal(ValidationTypes.InvalidParameters, serviceResult.Validation);
            }
        }

        /// <summary>
        /// Validate booking time
        /// Requirement: between 9:00 and 16:00 and of minute 0 or 30
        /// </summary>
        [Theory]
        [InlineData("08:59", "John", false)] //time is before 9:00 - invalid
        [InlineData("16:01", "John", false)] //time is after 16:00 - invalid
        [InlineData("09:01", "John", false)] //time is not of minute 0 or 30 - invalid
        [InlineData("15:31", "John", false)] //time is not of minute 0 or 30 - invalid
        [InlineData("09:00", "John", true)] //time is 9:00 onwards - valid
        [InlineData("16:00", "John", true)] //time is 16:00 backwards - valid
        [InlineData("09:30", "John", true)] //time is 9:00 onwards - valid
        [InlineData("15:30", "John", true)] //time is 16:00 backwards - valid
        public async void ValidateValidBookingTime(string time, string name, bool isValid)
        {
            ServiceResult<Guid> serviceResult = await _service.ExecuteBooking(time, name);
            if (isValid)
            {
                Assert.Equal(ValidationTypes.None, serviceResult.Validation);
            }
            else
            {
                Assert.Equal(ValidationTypes.InvalidTime, serviceResult.Validation);
            }
        }

        /// <summary>
        /// Validate booking time available i.e. must hold less than 4 simultaneous bookings at the same time
        /// </summary>
        [Theory]
        [InlineData("10:00", "John", 0, true)] //0 reservation exists for 10:00 - valid
        [InlineData("10:00", "John", 1, true)] //1 reservation exists for 10:00 - valid
        [InlineData("10:00", "John", 2, true)] //2 reservations exist for 10:00 - valid
        [InlineData("10:00", "John", 3, true)] //3 reservation exist for 10:00 - valid
        [InlineData("10:00", "John", 4, false)] //4 reservations exist for 10:00 - invalid i.e. only allow 4 simultaneously
        [InlineData("10:00", "John", 5, false)] //5 reservations exist for 10:00 - invalid i.e. only allow 4 simultaneously
        public async void ValidateReservedBookingTime(string time, string name, int reservationCount, bool isValid)
        {
            GenerateDummyReservationByTime(time, name, reservationCount);

            ServiceResult<Guid> serviceResult = await _service.ExecuteBooking(time, name);
            if (isValid)
            {
                Assert.Equal(ValidationTypes.None, serviceResult.Validation);
            }
            else
            {
                Assert.Equal(ValidationTypes.ReservedTime, serviceResult.Validation);
            }
        }

        /// <summary>
        /// Save booking returns bookingId
        /// </summary>
        [Theory]
        [InlineData("09:00", "09:59")]
        [InlineData("16:00", "16:59")]
        [InlineData("12:30", "13:29")]
        public async void SaveBooking(string startTime, string endTime)
        {
            string name = "John";
            ServiceResult<Guid> serviceResult = await _service.ExecuteBooking(startTime, name);

            BookingDetailsDto savedDetail = BookingStorage.First();
            Assert.Equal(serviceResult.Value, savedDetail.BookingId);
            Assert.Equal(name, savedDetail.Name);
            Assert.Equal(startTime, savedDetail.StartTime);
            Assert.Equal(endTime, savedDetail.EndTime);
        }

        /// <summary>
        /// Save booking for the same timeslot stops after 4 bookings 
        /// </summary>
        [Fact]
        public async void SaveBookingUntilLimit()
        {
            string startTime = "12:00";
            string name = "John";
            ServiceResult<Guid> booking1 = await _service.ExecuteBooking(startTime, name);
            ServiceResult<Guid> booking2 = await _service.ExecuteBooking(startTime, name);
            ServiceResult<Guid> booking3 = await _service.ExecuteBooking(startTime, name);
            ServiceResult<Guid> booking4 = await _service.ExecuteBooking(startTime, name);
            ServiceResult<Guid> booking5 = await _service.ExecuteBooking(startTime, name);

            Assert.Equal(ValidationTypes.None, booking1.Validation);
            Assert.Equal(ValidationTypes.None, booking2.Validation);
            Assert.Equal(ValidationTypes.None, booking3.Validation);
            Assert.Equal(ValidationTypes.None, booking4.Validation);
            Assert.Equal(ValidationTypes.ReservedTime, booking5.Validation);
        }

        /// <summary>
        /// Get all existing bookings when there is none available
        /// Expected: Error message returned says there's non available
        /// </summary>
        [Fact]
        public async void GetExistingBookingWhenNoneAvailable()
        {
            ServiceResult<IEnumerable<BookingDetailsViewModel>> res = await _service.GetAllBookings();
            Assert.Equal(ValidationTypes.NotFound, res.Validation);
            Assert.Null(res.Value);
        }

        /// <summary>
        /// Get all existing bookings when there are some available
        /// Expected: Get the bookings with correct view model
        /// </summary>
        [Fact]
        public async void GetExistingBookingWhenSomeAvailable()
        {
            string startTime = "12:00";
            string startTime2 = "09:30";
            string startTime3 = "16:00";
            string startTime4 = "14:30";
            string name = "John";
            await _service.ExecuteBooking(startTime, name);
            await _service.ExecuteBooking(startTime2, name);
            await _service.ExecuteBooking(startTime3, name);
            await _service.ExecuteBooking(startTime4, name);

            ServiceResult<IEnumerable<BookingDetailsViewModel>> res = await _service.GetAllBookings();
            Assert.Equal(ValidationTypes.None, res.Validation);
            Assert.Equal(4, res.Value.Count());
            Assert.Collection(res.Value, 
                e =>
                {
                    Assert.Equal(startTime, e.StartTime);
                },
                e =>
                {
                    Assert.Equal(startTime2, e.StartTime);
                },
                e =>
                {
                    Assert.Equal(startTime3, e.StartTime);
                },
                e =>
                {
                    Assert.Equal(startTime4, e.StartTime);
                }
            );
        }
    }
}