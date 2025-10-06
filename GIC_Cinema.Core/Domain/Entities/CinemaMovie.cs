using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Domain.Entities
{
    // public for access for other layers
    // sealed to prevent inheritance
    public sealed class CinemaMovie
    {
        public string Title { get; }
        public int Rows { get; }
        public int SeatsPerRow { get; }

        private readonly bool[,] _seatMatrix;

        private readonly Dictionary<string, Booking> _bookings;

        public CinemaMovie(string title, int rows, int seatsPerRow)
        {
            // Title input is not null check 
            if (string.IsNullOrEmpty(title)) throw new Exception("Input for title cannot be empty");

            // row input is between 1 and 26 check
            if (rows < 1 || rows > 26) throw new Exception("Input for rows should be within 1 and 26");

            // seatsPerRow input is between 1 and 50 check
            if (seatsPerRow < 1 || seatsPerRow > 50) throw new Exception("Input for seats per row should be within 1 and 50");

            Title = title;
            Rows = rows;
            SeatsPerRow = seatsPerRow;
            _seatMatrix = new bool[rows, seatsPerRow];
            _bookings = new Dictionary<string, Booking>();
        }

        public int AvailableSeats
        {
            get
            {
                int totalSeats = Rows * SeatsPerRow;
                int occupiedSeats = 0;
                for (int row = 0; row < Rows; row++)
                {
                    for (int seats = 0; seats < SeatsPerRow; seats++)
                    {
                        if (_seatMatrix[row, seats])
                        {
                            occupiedSeats++;
                        }
                    }
                }
                return totalSeats - occupiedSeats;
            }
        }

        public bool IsSeatOccupied(int row, int col)
        {
            return _seatMatrix[row, col];
        }

        public void ConfirmBooking(Booking booking)
        {
            foreach (Seat seat in booking.AllocatedSeats)
                _seatMatrix[seat.Row, seat.Col] = true;
            _bookings[booking.Id] = booking;
        }

        public IReadOnlyList<Seat>? GetBookingSeats(string bookingId)
            => _bookings.TryGetValue(bookingId, out var b) ? b.AllocatedSeats : null;

        public IEnumerable<Booking> GetAllBookings() => _bookings.Values;
    }
}
