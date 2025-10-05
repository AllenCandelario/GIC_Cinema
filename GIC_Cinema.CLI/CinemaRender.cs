using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GIC_Cinema.Core.Domain.Entities;

namespace GIC_Cinema.CLI
{
    public static class CinemaRender
    {
        // o = current booking
        // # = other confirmed seats
        // . = empty
        public static void RenderCinema(CinemaMovie movie, List<List<int>> seatsToBook)
        {
            Console.WriteLine("          S C R E E N");
            Console.WriteLine("--------------------------------");

            for (int r = movie.Rows - 1; r >= 0; r--)
            {
                Console.Write($"{(char)('A' + r)} ");

                for (int c = 0; c < movie.SeatsPerRow; c++)
                {
                    List<int> currentSeat = new List<int> { r, c };
                    char ch = '.';

                    if (seatsToBook.Any(innerList => innerList.SequenceEqual(currentSeat)))
                    {
                        ch = 'o';
                    }
                    else
                    {
                        if (movie.IsSeatOccupied(r, c))
                        {
                            ch = '#';
                        }
                    }

                    Console.Write(ch);
                    Console.Write("  ");
                }
                Console.WriteLine();
            }

            Console.Write(" ");
            for (int c = 1; c <= movie.SeatsPerRow; c++)
            {
                Console.Write(c.ToString(CultureInfo.InvariantCulture).PadLeft(2, ' '));
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
}
