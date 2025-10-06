using GIC_Cinema.Core.Application.Services;
using GIC_Cinema.Core.Domain.Entities;
using GIC_Cinema.Core.Domain.Services;
using GIC_Cinema.Core.Shared;

namespace GIC_Cinema.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            CinemaMovie movie = CreateCinema();

            while (true)
            {
                Console.WriteLine("Welcome to GIC Cinemas");
                Console.WriteLine($"[1] Book tickets for {movie.Title} ({movie.AvailableSeats} seats available)");
                Console.WriteLine($"[2] Check bookings");
                Console.WriteLine($"[3] Exit");
                Console.Write("Please enter your selection:\n> ");
                var selection = Console.ReadLine()?.Trim();

                if (selection == "1")
                {
                    Console.WriteLine();
                    BookSeats(movie);
                }
                else if (selection == "2")
                {
                    CheckBooking(movie);
                }
                else if (selection == "3")
                {
                    Console.WriteLine();
                    Console.WriteLine("Thank you for using GIC Cinemas system. Bye!");
                    return;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter the numbers '1', '2', or '3' only");
                    Console.WriteLine();
                }
            }
        }

        private static CinemaMovie CreateCinema()
        {
            while (true)
            {
                #region Console read
                Console.WriteLine("Please define movie title and seating map in [Title] [Row] [SeatsPerRow] format:");
                Console.Write("> ");
                var cinemaCreationInput = Console.ReadLine() ?? "";
                Console.WriteLine();
                #endregion

                #region Input & Domain Validation
                if (!Validation.TryParseCinemaCreationInput_CLI(cinemaCreationInput, out var title, out var rows, out var seatsPerRow, out string cli_error))
                {
                    Console.WriteLine($"{cli_error}\n");
                    continue;
                }
                if (!Validation.TryParseCinemaCreationInput_Domain(title, rows, seatsPerRow, out string domain_error))
                {
                    Console.WriteLine($"{domain_error}\n");
                    continue;
                }
                #endregion

                #region Logic
                try
                {
                    CinemaMovie movie = new CinemaMovie(title, rows, seatsPerRow);
                    return movie;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                #endregion
            }
        }

        private static void BookSeats(CinemaMovie movie)
        {
            while (true)
            {
                #region Console read
                Console.WriteLine("Enter number of tickets to book, or enter blank to go back to main menu:");
                Console.Write("> ");
                var seatNumberInput = Console.ReadLine();
                Console.WriteLine();
                #endregion

                #region Input & Domain validation (accepts blank input)
                if (string.IsNullOrEmpty(seatNumberInput))
                {
                    return;
                }
                if (!Validation.TryParseSeatNumberInput_CLI(seatNumberInput, out int seatNumber, out string cli_error))
                {
                    Console.WriteLine($"{cli_error}\n");
                    continue;
                }
                if (!Validation.TryParseSeatNumberInput_Domain(seatNumber, out string domain_error))
                {
                    Console.WriteLine($"{domain_error}\n");
                    continue;
                }
                #endregion

                #region Logic - Check for available seats
                if (!BookingService.CanBook(movie, seatNumber))
                {
                    Console.WriteLine($"Sorry, there are only {movie.AvailableSeats} seats available.");
                    Console.WriteLine();
                    continue;
                }
                #endregion

                #region Logic - Default seat allocation
                List<Seat> seatsToAllocate = BookingService.DefaultSeatingPreview(movie, seatNumber);
                string bookingId = BookingIdGenerator.GenerateNextBookingId();
                #endregion

                #region Console print cinema
                Console.WriteLine($"Successfully reserved {seatNumber} {movie.Title} tickets.");
                Console.WriteLine($"Booking id: {bookingId}");
                Console.WriteLine($"Selected seats:");
                Console.WriteLine();
                CinemaRender.RenderCinema(movie, seatsToAllocate);
                #endregion

                while (true)
                {
                    #region Console read
                    Console.WriteLine("Enter blank to accept seat selection, or enter new seating position:");
                    Console.Write("> ");
                    var seatConfirmOrPosition = Console.ReadLine();
                    Console.WriteLine();
                    #endregion

                    #region Logic - Confirm booking
                    if (string.IsNullOrEmpty(seatConfirmOrPosition))
                    {
                        BookingService.Confirm(movie, bookingId, seatsToAllocate);
                        Console.WriteLine($"Booking id: {bookingId} confirmed.");
                        Console.WriteLine();
                        return;
                    }
                    #endregion

                    #region Input & Domain validation
                    if (!Validation.TryParseSeatPositionInput_CLI(seatConfirmOrPosition.Trim(), movie.Rows, movie.SeatsPerRow, out var start, out string cli_error2))
                    {
                        Console.WriteLine($"{cli_error2}\n");
                        continue;
                    }
                    if (!Validation.TryParseSeatPositionInput_Domain(movie.Rows, movie.SeatsPerRow, start, out string domain_error2))
                    {
                        Console.WriteLine($"{domain_error2}\n");
                        continue;
                    }
                    #endregion

                    #region Logic - Position seat allocation
                    seatsToAllocate = BookingService.SelectedSeatingPreview(movie, seatNumber,  start);
                    #endregion

                    #region Console print cinema
                    Console.WriteLine($"Booking id: {bookingId}");
                    Console.WriteLine($"Selected seats:");
                    Console.WriteLine();
                    CinemaRender.RenderCinema(movie, seatsToAllocate);
                    #endregion
                }
            }
        }

        private static void CheckBooking(CinemaMovie movie)
        {
            while (true)
            {
                #region Console read
                Console.WriteLine();
                Console.WriteLine("Enter booking id, or enter blank to go back to main menu:");
                Console.Write("> ");
                string bookingId = Console.ReadLine();
                Console.WriteLine();
                #endregion

                #region Input validation (accepts blank input)
                if (string.IsNullOrWhiteSpace(bookingId))
                {
                    return;
                }
                #endregion

                #region Logic
                bookingId = bookingId.Trim().ToUpperInvariant();
                var seats = BookingService.GetBookingSeats(movie, bookingId);
                if (seats is null)
                {
                    Console.WriteLine("Booking not found.");
                    continue;
                }
                #endregion

                #region Console print cinema
                Console.WriteLine();
                Console.WriteLine($"Booking id: {bookingId}");
                Console.WriteLine("Selected seats:");
                Console.WriteLine();
                CinemaRender.RenderCinema(movie, seats);
                #endregion
            }
        }
    }
}
