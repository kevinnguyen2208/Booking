using System.Text.RegularExpressions;

namespace InfoTrackBooking.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Validate booking time to be valid hh:mm format
        /// </summary>
        public static bool CheckTimeFormat(string time)
        {
            string pattern = @"^([0-2][0-3]|[0-1][0-9]):[0-5][0-9]+$";
            bool isValidTimeFormat = Regex.IsMatch(time, pattern);
            return isValidTimeFormat;
        }

        /// <summary>
        /// Add 59 minutes to start of booking time
        /// </summary>
        public static string CreateEndTime(TimeSpan start)
        {
            TimeSpan end = start.Add(new TimeSpan(0, 59, 0));
            return end.ToString(@"hh\:mm");
        }
    }
}
