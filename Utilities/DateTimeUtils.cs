using System;
using System.Globalization;

namespace Utilities
{
    class DateTimeUtils
    {
        /// <summary>
        /// Return current time time in Milliseconds
        /// </summary>
        public static class CurrentMillis
        {
            private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            /// <summary>Get extra long current timestamp</summary>
            public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }
        }

        /// <summary>
        /// Return week of year, based on date and culture 
        /// </summary>
        /// <param name="dateTime">Time on which to base week of year</param>
        /// <param name="dateTimeFormatInfo">Culture specific Date information</param>
        /// <returns></returns>
        public static int WeekOfYear(DateTime dateTime, DateTimeFormatInfo dateTimeFormatInfo = null)
        {   
            if (dateTimeFormatInfo == null) dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
            if (dateTimeFormatInfo == null) return -1;
            var cal = dateTimeFormatInfo.Calendar;
            var weekOfYear = cal.GetWeekOfYear(dateTime, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);
            return weekOfYear-1;
        }
    }

    



}
