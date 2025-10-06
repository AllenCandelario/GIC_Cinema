using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GIC_Cinema.Core.Shared;
using GIC_Cinema.Core.Domain.Entities;

namespace GIC_Cinema.Tests
{
    public class ValidationTests
    {
        // -------------------- CLI PARSE: Cinema creation --------------------

        [Theory]
        [InlineData("Inception 8 10", "Inception", 8, 10)]
        [InlineData("Space between titles 1 1", "Space between titles", 1, 1)]
        [InlineData(" ExtraSpaces 1  1 ", "ExtraSpaces", 1, 1)]
        public void ParseCinemaCreation_ValidInput_CLI(string input, string expTitle, int expRows, int expSeats)
        {
            Assert.True(Validation.TryParseCinemaCreationInput_CLI(input, out var title, out var rows, out var seatsPerRow, out var err));
            Assert.Equal(expTitle, title);
            Assert.Equal(expRows, rows);
            Assert.Equal(expSeats, seatsPerRow);
            Assert.True(string.IsNullOrEmpty(err));
        }

        [Theory]
        [InlineData("")]
        [InlineData("FewerThan3Inputs 5")]
        [InlineData("NoNumbers here there")]
        [InlineData("Title NotANumber 10")]
        [InlineData("Title 8 NotANumber")]
        public void ParseCinemaCreation_InvalidInput_CLI(string input)
        {
            Assert.False(Validation.TryParseCinemaCreationInput_CLI(input, out _, out _, out _, out var err));
            Assert.False(string.IsNullOrEmpty(err));
        }

        // -------------------- DOMAIN: Cinema creation --------------------

        [Theory]
        [InlineData("Inception", 1, 1)]
        [InlineData("Any", 26, 50)]
        public void Domain_CinemaCreation_Valid(string title, int rows, int seatsPerRow)
        {
            Assert.True(Validation.TryParseCinemaCreationInput_Domain(title, rows, seatsPerRow, out var err));
            Assert.True(string.IsNullOrEmpty(err));
        }

        [Theory]
        [InlineData("", 8, 10)]
        [InlineData("   ", 8, 10)]
        [InlineData("Title", 0, 10)]
        [InlineData("Title", 27, 10)]
        [InlineData("Title", 8, 0)]
        [InlineData("Title", 8, 51)]
        public void Domain_CinemaCreation_Invalid(string title, int rows, int seatsPerRow)
        {
            Assert.False(Validation.TryParseCinemaCreationInput_Domain(title, rows, seatsPerRow, out var err));
            Assert.False(string.IsNullOrEmpty(err));
        }

        // -------------------- CLI PARSE: Seat number --------------------

        [Theory]
        [InlineData("1")]
        [InlineData("5")]
        public void ParseSeatNumber_CLI_Valid(string input)
        {
            Assert.True(Validation.TryParseSeatNumberInput_CLI(input, out var n, out var err));
            Assert.True(string.IsNullOrEmpty(err));
            Assert.Equal(int.Parse(input), n);
        }

        [Theory]
        [InlineData("0.1")]
        [InlineData("abc")]
        public void ParseSeatNumber_CLI_Invalid(string input)
        {
            Assert.False(Validation.TryParseSeatNumberInput_CLI(input, out _, out var err));
            Assert.False(string.IsNullOrEmpty(err));
        }

        // -------------------- DOMAIN: Seat number --------------------

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public void Domain_SeatNumber_Valid(int n)
        {
            Assert.True(Validation.TryParseSeatNumberInput_Domain(n, out var err));
            Assert.True(string.IsNullOrEmpty(err));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Domain_SeatNumber_Invalid(int n)
        {
            Assert.False(Validation.TryParseSeatNumberInput_Domain(n, out var err));
            Assert.False(string.IsNullOrEmpty(err));
        }

        // -------------------- CLI PARSE: Seat position --------------------

        [Theory]
        [InlineData("A01", 0, 0)]
        [InlineData("B03", 1, 2)]
        [InlineData("b03", 1, 2)]
        public void ParseSeatPosition_CLI_Valid(string input, int expRow, int expCol)
        {
            // rows/seatsPerRow are not used in CLI parse; domain will enforce later
            Assert.True(Validation.TryParseSeatPositionInput_CLI(input, rows: 26, seatsPerRow: 50, out Seat seat, out var err));
            Assert.True(string.IsNullOrEmpty(err));
            Assert.Equal(expRow, seat.Row);
            Assert.Equal(expCol, seat.Col);
        }

        [Theory]
        [InlineData("B3")]   // too short, requires at least 3 chars
        [InlineData("A0x")]  // non-digit columns
        [InlineData("A-1")]  // non-digit/dash
        public void ParseSeatPosition_CLI_Invalid(string input)
        {
            Assert.False(Validation.TryParseSeatPositionInput_CLI(input, rows: 26, seatsPerRow: 50, out _, out var err));
            Assert.False(string.IsNullOrEmpty(err));
        }

        // -------------------- DOMAIN: Seat position --------------------

        [Theory]
        [InlineData(2, 10, 0, 0)]  // A01 within 2x10
        [InlineData(2, 10, 1, 2)]  // B03 within 2x10
        public void Domain_SeatPosition_Valid(int rows, int seatsPerRow, int seatRow, int seatCol)
        {
            var seat = new Seat(seatRow, seatCol);
            Assert.True(Validation.TryParseSeatPositionInput_Domain(rows, seatsPerRow, seat, out var err));
            Assert.True(string.IsNullOrEmpty(err));
        }

        [Theory]
        [InlineData(2, 10, 25, 0)] // Z01 out of range for rows=2
        [InlineData(2, 10, 0, -1)] // col 0
        [InlineData(2, 10, 0, 10)] // col > seatsPerRow-1
        public void Domain_SeatPosition_Invalid(int rows, int seatsPerRow, int seatRow, int seatCol)
        {
            var seat = new Seat(seatRow, seatCol);
            Assert.False(Validation.TryParseSeatPositionInput_Domain(rows, seatsPerRow, seat, out var err));
            Assert.False(string.IsNullOrEmpty(err));
        }
    }
}
