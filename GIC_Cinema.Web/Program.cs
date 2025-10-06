using GIC_Cinema.Core.Domain.Entities;
using GIC_Cinema.Core.Application.Services;
using GIC_Cinema.Core.Shared;
using System.Diagnostics;

namespace GIC_Cinema.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddAuthorization()
                .AddSingleton<CinemaState>()
                .AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Create cinema
            app.MapPost("/cinema", (CinemaState state, CreateCinemaRequestDto req) =>
            {
                try
                {
                    if (!Validation.TryParseCinemaCreationInput_Domain(req.Title, req.Rows, req.SeatsPerRow, out string domain_error))
                    {
                        return Results.BadRequest(new { error = domain_error });
                    }
                    state.Movie = new CinemaMovie(req.Title, req.Rows, req.SeatsPerRow);
                    var m = state.Movie;
                    return Results.Ok(new CinemaResponseDto(m.Title, m.Rows, m.SeatsPerRow, m.AvailableSeats));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message }    );
                }
            });

            // Get cinema
            app.MapGet("/cinema", (CinemaState state) =>
            {
                var m = state.Movie;
                
                if (m is null) return Results.NotFound();
                
                CinemaResponseDto cinemaResponseDto = new CinemaResponseDto(m.Title, m.Rows, m.SeatsPerRow, m.AvailableSeats);
                return Results.Ok(cinemaResponseDto);
            });

            // Get cinema layout
            app.MapGet("/cinemalayout", (CinemaState state) =>
            {
                var m = state.Movie;
                
                if (m is null) return Results.NotFound();

                var grid = new bool[m.Rows][];
                for (int r = 0; r < m.Rows; r++)
                {
                    grid[r] = new bool[m.SeatsPerRow];
                    for (int c = 0; c < m.SeatsPerRow; c++)
                    {
                        grid[r][c] = m.IsSeatOccupied(r, c);
                    }
                }

                return Results.Ok(grid);
            });

            // Book seats via default allocation
            app.MapPost("/bookings", (CinemaState state, StartBookingRequestDto req) =>
            {
                var m = state.Movie;

                if (m is null) return Results.BadRequest(new { error = "Cinema does not exist" });

                if (!Validation.TryParseSeatNumberInput_Domain(req.SeatCount, out string domain_error))
                {
                    return Results.BadRequest(new { error = domain_error });
                }
                
                if (!BookingService.CanBook(m, req.SeatCount)) return Results.BadRequest(new { error = $"Sorry, there are only {m.AvailableSeats} seats available." });

                var preview = BookingService.DefaultSeatingPreview(m, req.SeatCount);
                var bookingId = BookingIdGenerator.PeekNextBookingId();

                BookingPreviewResponseDto bookingPreviewResponseDto = new BookingPreviewResponseDto(bookingId, preview.Select(s => s.ToString()).ToList());
                return Results.Ok(bookingPreviewResponseDto);
            });

            // Book seats via selected position
            app.MapPost("/bookings/from-position", (CinemaState state, StartBookingFromPositionRequestDto req) =>
            {
                var m = state.Movie;

                if (m is null) return Results.BadRequest(new { error = "Cinema does not exist" });

                if (!Validation.TryParseSeatNumberInput_Domain(req.SeatCount, out string domain_error))
                {
                    return Results.BadRequest(new { error = domain_error });
                }

                if (!BookingService.CanBook(m, req.SeatCount))
                {
                    return Results.BadRequest(new { error = $"Sorry, there are only {m.AvailableSeats} seats available." });
                }

                if (!Validation.TryParseSeatPositionInput_CLI(req.StartSeat.Trim(), m.Rows, m.SeatsPerRow, out var start, out string cli_error))
                {
                    return Results.BadRequest(new { error = cli_error });
                }

                if (!Validation.TryParseSeatPositionInput_Domain(m.Rows, m.SeatsPerRow, start, out string domain_error2))
                {
                    return Results.BadRequest(new { error = domain_error2 });
                }

                var preview = BookingService.SelectedSeatingPreview(m, req.SeatCount, start);
                var bookingId = BookingIdGenerator.PeekNextBookingId();

                var dto = new BookingPreviewResponseDto(bookingId, preview.Select(s => s.ToString()).ToList());
                return Results.Ok(dto);
            });

            // Confirm booking
            app.MapPut("/bookings/{id}/confirm", (CinemaState state, string id, ConfirmBookingRequestDto req) =>
            {
                var m = state.Movie;

                if (m is null) return Results.BadRequest(new { error = "Cinema does not exist" });

                id = id?.Trim().ToUpperInvariant() ?? "";

                if (string.IsNullOrEmpty(id)) return Results.BadRequest(new { error = "Booking ID is required" });

                if (req.Seats is null || req.Seats.Count == 0) return Results.BadRequest(new { error = "Seats required" });

                var seats = new List<Seat>(req.Seats.Count);
                foreach (var code in req.Seats)
                {
                    if (!Validation.TryParseSeatPositionInput_CLI(code.Trim(), m.Rows, m.SeatsPerRow, out var s, out string cli_error))
                    {
                        return Results.BadRequest(new { error = cli_error });
                    }
                    if (!Validation.TryParseSeatPositionInput_Domain(m.Rows, m.SeatsPerRow, s, out string domain_error))
                    {
                        return Results.BadRequest(new { error = domain_error });
                    }

                    if (m.IsSeatOccupied(s.Row, s.Col)) return Results.BadRequest(new { error = "Invalid seat" });

                    seats.Add(s);
                }

                var expected = BookingIdGenerator.PeekNextBookingId();
                if (!string.Equals(id, expected, StringComparison.Ordinal))
                {
                    return Results.BadRequest(new { error = "Booking ID is no longer valid. Please preview again." });
                }
                var generated = BookingIdGenerator.GenerateNextBookingId();

                BookingService.Confirm(m, id, seats);
                return Results.Ok();
            });

            // Check all bookings
            app.MapGet("/bookings", (CinemaState state) =>
            {
                var m = state.Movie;

                if (m is null) return Results.Ok(Array.Empty<BookingListItemResponseDto>());
                var list = m.GetAllBookings()
                            .Select(b => new BookingListItemResponseDto(
                                b.Id,
                                b.AllocatedSeats.Count,
                                b.AllocatedSeats.Select(s => s.ToString()).ToList()
                            ))
                            .ToList();
                return Results.Ok(list);
            });

            // Check booking
            app.MapGet("/bookings/{id}", (CinemaState state, string id) =>
            {
                var m = state.Movie;

                if (m is null) return Results.BadRequest(new { error = "Cinema does not exist" });

                id = id.Trim().ToUpperInvariant();

                var seats = BookingService.GetBookingSeats(m, id);
                if (seats is null)
                {
                    return Results.NotFound();
                }
                else
                {
                    BookingSeatsResponseDto bookingSeatsResponseDto = new BookingSeatsResponseDto(id, seats.Select(s => s.ToString()).ToList());
                    return Results.Ok(bookingSeatsResponseDto);
                }
            });

            // Reset (equivalent to option 3)
            app.MapPost("/reset", (CinemaState state) =>
            {
                state.Movie = null;
                BookingIdGenerator.ResetBookingId();
                return Results.Ok();
            });

            app.Run();
        }

        // singleton mutable holder for the CinemaMovie object 
        sealed class CinemaState { public CinemaMovie? Movie { get; set; } }

    }
}
