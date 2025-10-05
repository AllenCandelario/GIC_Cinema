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
                // we use the columns or seats per row as an interation tracker
                for (int currentIteration = 0; currentIteration < movie.SeatsPerRow && remainderSeatsToAllocate > 0; currentIteration++)
                {
                    if (currentIteration == 0)
                    {
                        if (!movie.IsSeatOccupied(currentRow, startingSeatToCheck))
                        {
                            seatsToAllocate.Add(new Seat(currentRow, startingSeatToCheck));
                            remainderSeatsToAllocate--;
                        }
                    }
                    else
                    {
                        int multiplier = (int)Math.Ceiling((double)currentIteration / 2);
                        multiplier = currentIteration % 2 == 0 ? -1 * multiplier : multiplier;
                        int currentSeatToCheck = startingSeatToCheck + multiplier;

                        if (!movie.IsSeatOccupied(currentRow, currentSeatToCheck))
                        {
                            seatsToAllocate.Add(new Seat(currentRow, currentSeatToCheck));
                            remainderSeatsToAllocate--;
                        }
                    }
                }
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
                    seatsToAllocate.Add(new Seat(startingRow, currentCol));
                    remainderSeatsToAllocate--;
                }
            }

            // middle out for subsequent rows
            int startingSeatToCheck = (movie.SeatsPerRow - 1) / 2;
            for (int currentRow = startingRow + 1; currentRow < movie.Rows && remainderSeatsToAllocate > 0; currentRow++)
            {
                // we use the columns or seats per row as an interation tracker
                for (int currentIteration = 0; currentIteration < movie.SeatsPerRow && remainderSeatsToAllocate > 0; currentIteration++)
                {
                    if (currentIteration == 0)
                    {
                        if (!movie.IsSeatOccupied(currentRow, startingSeatToCheck))
                        {
                            seatsToAllocate.Add(new Seat(currentRow, startingSeatToCheck));
                            remainderSeatsToAllocate--;
                        }
                    }
                    else
                    {
                        int multiplier = (int)Math.Ceiling((double)currentIteration / 2);
                        multiplier = currentIteration % 2 == 0 ? -1 * multiplier : multiplier;
                        int currentSeatToCheck = startingSeatToCheck + multiplier;

                        if (!movie.IsSeatOccupied(currentRow, currentSeatToCheck))
                        {
                            seatsToAllocate.Add(new Seat ( currentRow, currentSeatToCheck ));
                            remainderSeatsToAllocate--;
                        }
                    }
                }
            }

            // overflow back to furthest row if there are still remaining seats
            for (int currentRow = 0; currentRow < movie.Rows && remainderSeatsToAllocate > 0; currentRow++)
            {
                // we use the columns or seats per row as an interation tracker
                for (int currentIteration = 0; currentIteration < movie.SeatsPerRow && remainderSeatsToAllocate > 0; currentIteration++)
                {
                    if (currentIteration == 0)
                    {
                        if (!movie.IsSeatOccupied(currentRow, startingSeatToCheck))
                        {
                            seatsToAllocate.Add(new Seat (currentRow, startingSeatToCheck ));
                            remainderSeatsToAllocate--;
                        }
                    }
                    else
                    {
                        int multiplier = (int)Math.Ceiling((double)currentIteration / 2);
                        multiplier = currentIteration % 2 == 0 ? -1 * multiplier : multiplier;
                        int currentSeatToCheck = startingSeatToCheck + multiplier;

                        if (!movie.IsSeatOccupied(currentRow, currentSeatToCheck))
                        {
                            seatsToAllocate.Add(new Seat ( currentRow, currentSeatToCheck ));
                            remainderSeatsToAllocate--;
                        }
                    }
                }
            }
            return seatsToAllocate;
        }
    }
}
