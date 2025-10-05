using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Shared
{
    public static class BookingIdGenerator
    {
        private static int _counter = 0;

        public static string GenerateNextBookingId()
        {
            return $"GIC{(++_counter).ToString("D4", CultureInfo.InvariantCulture)}";
        }
    }
}
