
namespace GIC_Cinema.Core.Domain.Entities
{
    // public for access for other layers
    // sealed to prevent inheritance
    public sealed class Booking
    {
        public string Id { get; }
        public IReadOnlyList<Seat> AllocatedSeats { get; }

        public Booking(string bookingId, List<Seat> seatsToAllocate)
        {
            Id = bookingId;
            AllocatedSeats = seatsToAllocate.AsReadOnly();
        }
    }
}
