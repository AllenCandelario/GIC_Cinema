using GIC_Cinema.Core.Domain.Entities;

namespace GIC_Cinema.Core.Shared
{
    public static class Validation
    {
        #region Input validation for CLI app
        public static bool TryParseCinemaCreationInput_CLI(string input, out string title, out int rows, out int seatsPerRow, out string error)
        {
            title = "";
            rows = 0;
            seatsPerRow = 0;
            error = "";

            // Null check
            if (string.IsNullOrWhiteSpace(input)) 
            { 
                error = "Input cannot be empty";
                return false;
            }

            // Input portions >= 3 check
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (parts.Count < 3)
            {
                error = "Input count should be at least 3";
                return false;
            }

            // Title input is not null/empty check
            title = string.Join(' ', parts.Take(parts.Count - 2));
            if (string.IsNullOrEmpty(title))
            {
                error = "Title cannot be empty";
                return false;
            }

            // Row input is an integer check
            if (!int.TryParse(parts[^2], out rows))
            {
                error = "Input for rows should be a valid number";
                return false;
            }

            // seatsPerRow input is an integer check
            if (!int.TryParse(parts[^1], out seatsPerRow))
            {
                error = "Input for seats per row should be a valid number";
                return false;
            }

            return true;
        }

        public static bool TryParseSeatNumberInput_CLI(string input, out int seatNumber, out string error)
        {
            seatNumber = 0;
            error = "";

            // Seat number is an integer check
            if (!int.TryParse(input, out seatNumber))
            {
                error = "Input for seat number should be a valid number";
                return false;
            }

            return true;
        }

        public static bool TryParseSeatPositionInput_CLI(string input, int rows, int seatsPerRow, out Seat seat, out string error)
        {
            seat = default;
            error = "";

            // Input should have at least 3 characters check for e.g. B03, A14
            if (input.Length < 3)
            {
                error = "Input should contain at least 3 characters";
                return false;
            }

            var rowChar = char.ToUpperInvariant(input[0]);

            // Input's subsequent character should be a valid number
            var numberPart = input[1..];
            if (!(numberPart.All(char.IsDigit) && int.TryParse(numberPart, out var col)))
            {
                error = "Input for seat position should be a valid number";
                return false;
            }

            seat = new Seat(rowChar - 'A', col - 1);
            return true;
        }

        #endregion

        #region Domain validation
        public static bool TryParseCinemaCreationInput_Domain(string title, int rows, int seatsPerRow, out string error)
        {
            error = "";

            // Title input is not null/empty check
            if (string.IsNullOrWhiteSpace(title))
            {
                error = "Title cannot be empty";
                return false;
            }

            // Row input must be between numbers 1 and 26
            if (rows < 1 || rows > 26)
            {
                error = "Rows must be between 1 and 26";
                return false;
            }

            // seatsPerRow input must be between numbers 1 and 50
            if (seatsPerRow < 1 || seatsPerRow > 50)
            {
                error = "Seats per row must be between 1 and 50";
                return false;
            }

            return true;
        }

        public static bool TryParseSeatNumberInput_Domain(int seatNumber, out string error)
        {
            error = "";

            // Seat number cannot be negative check 
            if (seatNumber < 1)
            {
                error = "Seat number must be at least 1";
                return false;
            }

            return true;
        }

        public static bool TryParseSeatPositionInput_Domain(int rows, int seatsPerRow, Seat seat, out string error)
        {
            error = "";

            var rowChar = char.ToUpperInvariant((char)(seat.Row + 'A'));
            var col = seat.Col + 1;

            // Input's first character should be within A - Z
            if (rowChar < 'A' || rowChar >= ('A' + rows))
            {
                error = "First character of input should be a letter";
                return false;
            }

            // Input's subsequent character (confirmed as a valid number) should be within the cinema's boundaries (min 1, max seatsPerRow)
            if (col < 1 || col > seatsPerRow)
            {
                error = "Input for seat position exceeds movie's seat number";
                return false;
            }

            return true;
        }

        #endregion 

    }
}
