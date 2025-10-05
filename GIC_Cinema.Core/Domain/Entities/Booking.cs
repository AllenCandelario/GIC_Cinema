using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Domain.Entities
{
    // public for access for other layers
    // sealed to prevent inheritance
    public sealed class Booking
    {
        public string Id { get; }
        public List<List<int>> AllocatedSeats { get; }

        public Booking(string bookingId, List<List<int>> seatsToAllocate)
        {
            Id = bookingId;
            AllocatedSeats = seatsToAllocate;
        }
    }
}
