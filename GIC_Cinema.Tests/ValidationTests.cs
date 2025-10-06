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
        [Theory]
        [InlineData("Inception 8 10", "Inception", 8, 10)]
        [InlineData("The Dark Knight 1 1", "The Dark Knight", 1, 1)]
        [InlineData(" ExtraSpaces 1  1 ", "ExtraSpaces", 1, 1)]
        public void ParseCinemaCreation_Valid(string input, string title, int rows, int seats)
        {
            Assert.True(Validation.TryParseCinemaCreationInput(input, out var t, out var r, out var s));
            Assert.Equal(title, t);
            Assert.Equal(rows, r);
            Assert.Equal(seats, s);
        }

        [Theory]
        [InlineData("")]
        [InlineData("FewerThan3Inputs 5")]
        [InlineData("ZeroRows 0 10")]
        [InlineData("MoreThan26Rows 27 10")]
        [InlineData("ZeroCol 8 0")]
        [InlineData("MoreThan50Col 8 51")]
        public void ParseCinemaCreation_Invalid(string input)
        {
            Assert.False(Validation.TryParseCinemaCreationInput(input, out _, out _, out _));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("5")]
        public void ParseSeatNumber_Valid(string input)
        {
            Assert.True(Validation.TryParseSeatNumberInput(input, out var n));
            Assert.True(n >= 1);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("abc")]
        public void ParseSeatNumber_Invalid(string input)
        {
            Assert.False(Validation.TryParseSeatNumberInput(input, out _));
        }

        [Theory]
        [InlineData("A01", 0, 0)]
        [InlineData("A015", 1, 2)]
        [InlineData("B03", 1, 2)]
        [InlineData("b03", 1, 2)]
        public void ParseSeatPosition_Valid(string input, int expRow, int expCol)
        {
            Assert.True(Validation.TryParseSeatPositionInput(input, rows: 2, seatsPerRow: 10, out Seat seat));
            Assert.Equal(expRow, seat.Row);
            Assert.Equal(expCol, seat.Col);
        }

        [Theory]
        [InlineData("Z09")]   // row out of range
        [InlineData("A00")]   // col zero
        [InlineData("A11")]   // col out of range (when seatsPerRow=10)
        [InlineData("1A")]    // bad format
        [InlineData("B3")]    // no leading zeroes
        [InlineData("A0x")]   // non-digit columns 1
        [InlineData("A-1")]   // non-digit columns 2
        public void ParseSeatPosition_Invalid(string input)
        {
            Assert.False(Validation.TryParseSeatPositionInput(input, rows: 2, seatsPerRow: 10, out _));
        }
    }
}
