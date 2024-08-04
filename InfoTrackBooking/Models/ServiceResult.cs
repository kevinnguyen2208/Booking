namespace InfoTrackBooking.Models
{
    public class ServiceResult<T>
    {
        public ServiceResult(T value)
        {
            Value = value;
            Validation = ValidationTypes.None;
            Message = null;
        }

        public T Value { get; set; }
        public ValidationTypes Validation { get; set; }
        public string Message { get; set; }

        public static ServiceResult<T> CreateErrorMessage(string errorMessage, ValidationTypes validation = ValidationTypes.NotFound)
        {
            var result = new ServiceResult<T>(default(T))
            {
                Validation = validation,
                Message = errorMessage
            };

            return result;
        }
    }
}
