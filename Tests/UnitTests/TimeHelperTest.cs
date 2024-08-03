using InfoTrackBooking.Helpers;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests
{
    public class TimeHelperTest
    {
        [Theory]
        [InlineData("9:00", false)]
        [InlineData("90:00", false)]
        [InlineData("90:000", false)]
        [InlineData("24:00", false)]
        [InlineData("24:11", false)]
        [InlineData("01:60", false)]
        [InlineData("01:00", true)]
        [InlineData("09:11", true)]
        [InlineData("23:59", true)]
        public void ValidateTimeFormat(string input, bool isValid)
        {
            bool result = TimeHelper.CheckTimeFormat(input);
            Assert.Equal(result, isValid);
        }

        [Theory]
        [InlineData("09:00", "09:59")]
        [InlineData("09:30", "10:29")]
        [InlineData("09:11", "10:10")]
        [InlineData("16:00", "16:59")]
        public void CreateEndTime(string input, string expectedOutput)
        {
            TimeSpan time = TimeSpan.Parse(input);
            string output = TimeHelper.CreateEndTime(time);
            Assert.Equal(output, expectedOutput);
        }
    }
}
