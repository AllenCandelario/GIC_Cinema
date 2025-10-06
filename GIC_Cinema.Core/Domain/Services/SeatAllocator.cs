using GIC_Cinema.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Domain.Services
{
    public class SeatAllocator
    {
        public static List<Seat> DefaultAllocation(CinemaMovie movie, int seatNumber)
        {
            int remainderSeatsToAllocate = seatNumber;
            List<Seat> seatsToAllocate = new List<Seat>(seatNumber);
            int startingSeatToCheck = (movie.SeatsPerRow - 1) / 2;


            for (int currentRow = 0; currentRow < movie.Rows && remainderSeatsToAllocate > 0; currentRow++)
            {
                int before = seatsToAllocate.Count;
                FillRowMiddleOut(movie, currentRow, remainderSeatsToAllocate, seatsToAllocate);
                remainderSeatsToAllocate -= (seatsToAllocate.Count - before);
            }
            return seatsToAllocate;
        }

        public static List<Seat> AllocateFromPosition(CinemaMovie movie, int seatNumber, Seat startingPosition)
        {
            int remainderSeatsToAllocate = seatNumber;
            List<Seat> seatsToAllocate = new List<Seat>(seatNumber);
            int startingRow = startingPosition.Row;
            int startingCol = startingPosition.Col;

            // fill to the right on the starting row
            for (int currentCol = startingCol; currentCol < movie.SeatsPerRow && remainderSeatsToAllocate > 0; currentCol++)
            {
                if (!movie.IsSeatOccupied(startingRow, currentCol))
                {
                    int before = seatsToAllocate.Count;
                    seatsToAllocate.Add(new Seat(startingRow, currentCol));
                    remainderSeatsToAllocate -= (seatsToAllocate.Count - before);
                }
            }

            // middle out for subsequent rows
            int startingSeatToCheck = (movie.SeatsPerRow - 1) / 2;
            for (int currentRow = startingRow + 1; currentRow < movie.Rows && remainderSeatsToAllocate > 0; currentRow++)
            {
                int before = seatsToAllocate.Count;
                FillRowMiddleOut(movie, currentRow, remainderSeatsToAllocate, seatsToAllocate);
                remainderSeatsToAllocate -= (seatsToAllocate.Count - before);
            }

            // overflow back to furthest row if there are still remaining seats
            for (int currentRow = 0; currentRow < movie.Rows && remainderSeatsToAllocate > 0; currentRow++)
            {
                int before = seatsToAllocate.Count;
                FillRowMiddleOut(movie, currentRow, remainderSeatsToAllocate, seatsToAllocate);
                remainderSeatsToAllocate -= (seatsToAllocate.Count - before);
            }
            return seatsToAllocate;
        }

        private static void FillRowMiddleOut(CinemaMovie movie, int row, int remainderSeatsToAllocate, List<Seat> seatsToAllocate)
        {
            int startingSeatColumnToCheck = (movie.SeatsPerRow - 1) / 2;

            // we use the columns / seats per row as an interation tracker
            for (int currentIteration = 0; currentIteration < movie.SeatsPerRow && remainderSeatsToAllocate > 0; currentIteration++)
            {
                int offset = (currentIteration + 1) / 2; // 0, 1, 1, 2, 2, 3, 3
                int seatCol = startingSeatColumnToCheck;
                if (currentIteration != 0)
                {
                    if (currentIteration % 2 == 0)
                    {
                        seatCol -= offset;
                    }
                    else
                    {
                        seatCol += offset;
                    }
                }

                if (!movie.IsSeatOccupied(row, seatCol))
                {
                    seatsToAllocate.Add(new Seat(row, seatCol));
                    remainderSeatsToAllocate--;
                }
                
            }
        }
    }
}
