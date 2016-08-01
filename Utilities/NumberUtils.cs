using System;
using System.Collections.Generic;

namespace Utilities
{
    class NumberUtils
    {
        public static int ClosestTo(IEnumerable<int> collection, int target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            var closest = int.MaxValue;
            var minDifference = int.MaxValue;
            foreach (var element in collection)
            {
                var difference = Math.Abs((long)element - target);
                if (minDifference <= difference) continue;
                minDifference = (int)difference;
                closest = element;
            }
            return closest;
        }

        public static long ClosestTo(IEnumerable<long> collection, long target)
        {
            // NB Method will return int.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary.
            var closest = long.MaxValue;
            var minDifference = long.MaxValue;
            foreach (var element in collection)
            {
                var difference = Math.Abs((long)element - target);
                if (minDifference <= difference) continue;
                minDifference = (int)difference;
                closest = element;
            }
            return closest;
        }
    }
}
