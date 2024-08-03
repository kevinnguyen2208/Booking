using InfoTrackBooking.Models;
using InfoTrackBooking.Repositories;
using InfoTrackBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfoTrackBooking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteBooking([FromBody] BookingRequest request)
        {
            ServiceResult<Guid> bookingValidation = await _bookingService.ExecuteBooking(request.BookingTime, request.Name);
            switch (bookingValidation.Validation)
            {
                case ValidationTypes.InvalidParameters:
                case ValidationTypes.InvalidTime:
                    return BadRequest(bookingValidation.Message);
                case ValidationTypes.ReservedTime:
                    return Conflict(bookingValidation.Message);
                case ValidationTypes.None:
                default:
                    return Ok(bookingValidation.Value);
            }
        }
    }
}
