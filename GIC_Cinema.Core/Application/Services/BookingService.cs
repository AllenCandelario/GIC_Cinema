using GIC_Cinema.Core.Domain.Entities;
using GIC_Cinema.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Application.Services
{
    public static class BookingService
    {
        public static bool CanBook(CinemaMovie movie, int seatCount) => seatCount >= 1 && seatCount <= movie.AvailableSeats;

        public static List<Seat> DefaultSeatingPreview(CinemaMovie movie, int seatCount) => SeatAllocator.DefaultAllocation(movie, seatCount);

        public static List<Seat> SelectedSeatingPreview(CinemaMovie movie, int seatCount, Seat start) => SeatAllocator.AllocateFromPosition(movie, seatCount, start);

        public static void Confirm(CinemaMovie movie, string bookingId, List<Seat> allocatedSeats)
        {
            var booking = new Booking(bookingId, allocatedSeats);
            movie.ConfirmBooking(booking);
        }

        public static IReadOnlyList<Seat> GetBookingSeats(CinemaMovie movie, string bookingId) => movie.GetBookingSeats(bookingId);
    }
}
