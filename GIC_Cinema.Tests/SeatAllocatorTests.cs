using GIC_Cinema.Core.Domain.Entities;
using GIC_Cinema.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Tests
{
    public class SeatAllocatorTests
    {
        [Fact]
        public void DefaultAllocation_CenterOut_FromFurthestRow()
        {
            var movie = new CinemaMovie("Inception", rows: 3, seatsPerRow: 5);
            // Expect row 0 center-first: col 2, then 3,1,4,0 then row 1, etc.
            var seats = SeatAllocator.DefaultAllocation(movie, 4);

            Assert.Equal(4, seats.Count);
            Assert.Equal(new List<int> { 0, 2 }, seats[0]);
            Assert.Equal(new List<int> { 0, 3 }, seats[1]);
            Assert.Equal(new List<int> { 0, 1 }, seats[2]);
            Assert.Equal(new List<int> { 0, 4 }, seats[3]);
        }

        [Fact]
        public void AllocateFromPosition_FillRight_ThenNextRowsCenterOut()
        {
            var movie = new CinemaMovie("Inception", rows: 3, seatsPerRow: 5);
            // Occupy some seats to force skips
            movie.ConfirmBooking(new Booking("GIC0001", new() { new List<int> { 0, 2 }, new List<int> { 0, 3 } }));

            var result = SeatAllocator.AllocateFromPosition(movie, seatNumber: 5, startingPosition: new List<int> { 0, 1 });

            Assert.Equal(5, result.Count);
            Assert.Equal(new List<int> { 0, 1 }, result[0]);
            Assert.Equal(new List<int> { 0, 4 }, result[1]);
            Assert.Equal(new List<int> { 1, 2 }, result[2]);
            Assert.Equal(new List<int> { 1, 3 }, result[3]);
            Assert.Equal(new List<int> { 1, 1 }, result[4]);
        }

        [Fact]
        public void Allocation_OverflowsAcrossRows_WhenRowNotEnough()
        {
            var movie = new CinemaMovie("Inception", rows: 2, seatsPerRow: 3);
            var seats = SeatAllocator.DefaultAllocation(movie, 5);

            Assert.Equal(5, seats.Count);

            Assert.Contains(new List<int> { 0, 1 }, seats);
            Assert.Contains(new List<int> { 0, 2 }, seats);
            Assert.Contains(new List<int> { 0, 0 }, seats);
            Assert.Contains(new List<int> { 1, 1 }, seats);
            Assert.Contains(new List<int> { 1, 2 }, seats);
        }

        [Fact]
        public void Allocation_SkipsOccupiedSeats()
        {
            var movie = new CinemaMovie("Inception", rows: 1, seatsPerRow: 5);
            movie.ConfirmBooking(new Booking("GIC0001", new() { new List<int> { 0, 2 }, new List<int> { 0, 3 } }));

            var seats = SeatAllocator.DefaultAllocation(movie, 3);

            Assert.Equal(3, seats.Count);
            Assert.DoesNotContain(new List<int> { 0, 2 }, seats);
            Assert.DoesNotContain(new List<int> { 0, 3 }, seats);
        }
    }
}
