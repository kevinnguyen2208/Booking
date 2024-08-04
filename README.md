# InfoTrack Tech Challenge

Implement a booking API that will accept a booking time and respond indicating whether the reservation was successful or not.


## Requirements

- Assume that all bookings are for the same day (do not worry about handling dates).

- InfoTrack's hours of business are 9am-5pm, all bookings must complete by 5pm (latest booking is 4:00pm).

- Bookings are for 1 hour (booking at 9:00am means the spot is held from 9:00am to 9:59am).

- InfoTrack accepts up to 4 simultaneous settlements.

- API needs to accept POST requests of the following format:

  ```json
  {
    "bookingTime": "09:30",
    "name": "John Smith"
  }
  ```

- Successful bookings should respond with an OK status and a booking Id in GUID form
  ```json
  {
    "bookingId": "d90f8c55-90a5-4537-a99d-c68242a6012b"
  }
  ```

- Requests for out of hours times should return a Bad Request status.
- Requests with invalid data should return a Bad Request status.
- Requests when all settlements at a booking time are reserved should return a Conflict status
- The name property should be a non-empty string.
- The bookingTime property should be a 24-hour time (00:00 - 23:59)
- Bookings can be stored in-memory, it is fine for them to be forgotten when the application is restarted.
- **Further assumption**: Booking time must be of minute 00 or 30. Doesn’t make sense when booking is made at 13:21.
- **Added feature**: GET request to get all existing bookings, to showcase ViewModel Mapper and adherent to MVC pattern.


## Features
1. Data validations for the request parameters before saving.
2. Data storage is EntityFramework In-Memory.
3. Layers (Controller -> Service -> Repository) based on MVC pattern.
4. Controller: ```BookingController``` - handles the incoming request and outputs IActionResult accordingly.
5. Service: ```BookingService``` and interface ```IBookingService``` - handles business logic, data validation.
6. Repository: ```BookingRepository``` and interface ```IBookingRepository``` handles extracting data, filtering, sorting and accessing database to save.
7. Unit tests: ```BookingServiceTest``` and ```TimeHelperTest```. Implements ```[Theory]``` and ```[Fact]``` by XUnit.
8. CI/CD workflows for build and unit tests. Can be seen in GitHub Actions.
9. Data models: ```BookingRequest``` to handle JSON payload and ```BookingDetailsDto``` to store data.
10. Mapper: Maps ```BookingDetailsDto``` to ```BookingDetailsViewModel``` in order to display to the request recipeient only necessary information.
11. Delegate class: ```ServiceResult``` - Accepts generic type for future expansion. For this project type GUID and ```BookingDetailsViewModel``` are used.
12. Helper class: ```TimeHelper``` - Handles time data types.

## How to test
1. Clone this repo and open in Visual Studio.
2. Start the solution
3. Use JSON payload in SwaggerUI. SwaggerUI is automatically opened when Start the solution at the link below.
```
https://localhost:7260/Booking
```
4. Alternatively, cUrl can be used.
   
GET request to get existing bookings
```
curl https://localhost:7260/Booking
```
POST request to post JSON payload and return validation message or GUID as ```bookingId``` if successful.
```
curl -X POST -H "Content-Type: application/json" -d "{ \"bookingTime\": \"09:00\", \"name\": \"John Smith\" }" https://localhost:7260/Booking
```


