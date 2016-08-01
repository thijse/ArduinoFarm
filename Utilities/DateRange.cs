using System;

namespace Utilities
{
    public class DateRange
    {
        public DateRange()
        {
            Start = DateTime.MinValue;
            End = DateTime.MaxValue;
        }

        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool Includes(DateTime value)
        {
            return (Start <= value) && (value <= End);
        }

        public bool Includes(DateRange range)
        {
            return (Start <= range.Start) && (range.End <= End);
        }

        public static DateRange After(DateTime start)
        {
            return new DateRange(start, DateTime.MaxValue);
        }

        public static DateRange Before(DateTime end)
        {
            return new DateRange(DateTime.MinValue, end);
        }

        public static DateRange All()
        {
            return new DateRange(DateTime.MinValue, DateTime.MaxValue);
        }

    }
}
