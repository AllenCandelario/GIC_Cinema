using GIC_Cinema.Core.Application.Services;
using GIC_Cinema.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Tests
{
    public class BookingServiceTests
    {
        [Fact]
        public void CanBook_True_WhenWithinCapacity()
        {
            var m = new CinemaMovie("Inception", rows: 2, seatsPerRow: 3); // 2 x 3 = 6 seats
            Assert.True(BookingService.CanBook(m, 6));
            Assert.True(BookingService.CanBook(m, 1));
        }

        [Fact]
        public void CanBook_False_WhenExceedsCapacity()
        {
            var m = new CinemaMovie("Inception", rows: 2, seatsPerRow: 3); // 2 x 3 = 6 seats
            Assert.False(BookingService.CanBook(m, 7));
            Assert.False(BookingService.CanBook(m, 0));
            Assert.False(BookingService.CanBook(m, -1));
        }
    }
}
