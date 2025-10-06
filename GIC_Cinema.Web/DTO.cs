namespace GIC_Cinema.Web
{
    // request dtos
    public sealed record CreateCinemaRequestDto(string Title, int Rows, int SeatsPerRow);
    public sealed record StartBookingRequestDto(int SeatCount);
    public sealed record StartBookingFromPositionRequestDto(int SeatCount, string StartSeat);
    public sealed record ConfirmBookingRequestDto(List<string> Seats);

    // response dtos
    public sealed record CinemaResponseDto(string Title, int Rows, int SeatsPerRow, int AvailableSeats);
    public sealed record BookingPreviewResponseDto(string BookingId, List<string> Seats);
    public sealed record BookingSeatsResponseDto(string BookingId, List<string> Seats);
    public sealed record BookingListItemResponseDto(string BookingId, int SeatCount, List<string> Seats);
}
