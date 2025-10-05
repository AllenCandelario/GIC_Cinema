using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Shared
{
    public static class Validation
    {
        // Base validation for null/invalid input checks
        // Domain validation will be done within the model itself
        public static bool TryParseCinemaCreationInput(string input, out string title, out int rows, out int seatsPerRow)
        {
            title = "";
            rows = 0;
            seatsPerRow = 0;

            try
            {
                // Null check
                if (string.IsNullOrWhiteSpace(input)) throw new Exception("Input cannot be empty");

                // Input portions >= 3 check
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (parts.Count < 3) throw new Exception("Input count should be at least 3");

                // Title input is not null/empty check
                title = string.Join(' ', parts.Take(parts.Count - 2));
                if (string.IsNullOrEmpty(title)) throw new Exception("Title cannot be empty");

                // Row input is an integer check
                if (!int.TryParse(parts[^2], out rows)) throw new Exception("Input for rows should be a valid number");

                // Row input must be between numbers 1 and 26
                if (rows < 1 || rows > 26) throw new Exception("Rows must be between 1 and 26");

                // seatsPerRow input is an integer check
                if (!int.TryParse(parts[^1], out seatsPerRow)) throw new Exception("Input for seats per row should be a valid number");

                // seatsPerRow input must be between numbers 1 and 50
                if (seatsPerRow < 1 || seatsPerRow > 50) throw new Exception("Seats per row must be between 1 and 50");


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool TryParseSeatNumberInput(string input, out int seatNumber)
        {
            seatNumber = 0;

            try
            {
                // Seat number is an integer check
                if (!int.TryParse(input, out seatNumber)) throw new Exception("Input for seat number should be a valid number");

                // Seat number cannot be negative check 
                if (seatNumber < 1) throw new Exception("Seat number must be at least 1");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool TryParseSeatPositionInput(string input, int rows, int seatsPerRow, out List<int> seat)
        {
            seat = default;
            try
            {
                // Input should have at least 3 characters check for e.g. B03, A14
                if (input.Length < 2) throw new Exception("Input should contain at least 3 characters");

                // Input's first character should be within A - Z
                var rowChar = char.ToUpperInvariant(input[0]);
                if (rowChar < 'A' || rowChar >= ('A' + rows)) throw new Exception("First character of input should be a letter");

                // Input's subsequent character should be a valid number
                if (!int.TryParse(input[1..], out var col)) throw new Exception("Input for seat position should be a valid number");

                // Input's subsequent character (confirmed as a valid number) should be within the cinema's boundaries (min 1, max seatsPerRow)
                if (col < 1 || col > seatsPerRow) throw new Exception("Input for seat position exceeds movie's seat number");

                seat = new List<int> { rowChar - 'A', col - 1 };
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
