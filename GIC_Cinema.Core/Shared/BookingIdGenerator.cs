using System.Globalization;

namespace GIC_Cinema.Core.Shared
{
    public static class BookingIdGenerator
    {
        private static int _counter = 0;

        public static string GenerateNextBookingId()
        {
            return $"GIC{(++_counter).ToString("D4", CultureInfo.InvariantCulture)}";
        }

        public static string PeekNextBookingId()
        {
            return $"GIC{(_counter + 1).ToString("D4", CultureInfo.InvariantCulture)}";
        }

        public static void ResetBookingId()
        {
            _counter = 0;
        }
    }
}
