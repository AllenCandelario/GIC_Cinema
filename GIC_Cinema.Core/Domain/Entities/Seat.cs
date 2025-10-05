using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIC_Cinema.Core.Domain.Entities
{
    // struct instead of List<int> to make Seat a compact value type
    // readonly to ensure immutability
    // record for built-in features 
    public readonly record struct Seat
    {
        public int Row { get; }
        public int Col { get; }

        public Seat(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override string ToString() => $"{(char)('A' + Row)}{(Col + 1):D2}";
    }
}
