using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Base
{
    public sealed class PuzzleMove
    {
        private readonly int[] _destinationBySource;

        public PuzzleMove(string id, string notation, IReadOnlyList<int> destinationBySource)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Move id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(notation))
            {
                throw new ArgumentException("Move notation cannot be empty.", nameof(notation));
            }

            Id = id;
            Notation = notation;
            _destinationBySource = destinationBySource.ToArray();
            ValidatePermutation(_destinationBySource);
        }

        public string Id { get; }

        public string Notation { get; }

        public IReadOnlyList<int> DestinationBySource => _destinationBySource;

        public int PositionCount => _destinationBySource.Length;

        public PuzzleMove Inverse(string id, string notation)
        {
            var inverse = new int[_destinationBySource.Length];

            for (var source = 0; source < _destinationBySource.Length; source++)
            {
                inverse[_destinationBySource[source]] = source;
            }

            return new PuzzleMove(id, notation, inverse);
        }

        public PuzzleMove Then(PuzzleMove next, string id, string notation)
        {
            if (next.PositionCount != PositionCount)
            {
                throw new ArgumentException("Moves must have the same position count.", nameof(next));
            }

            var composed = new int[PositionCount];

            for (var source = 0; source < PositionCount; source++)
            {
                composed[source] = next._destinationBySource[_destinationBySource[source]];
            }

            return new PuzzleMove(id, notation, composed);
        }

        public static PuzzleMove Identity(string id, string notation, int positionCount)
        {
            var destinationBySource = new int[positionCount];

            for (var i = 0; i < positionCount; i++)
            {
                destinationBySource[i] = i;
            }

            return new PuzzleMove(id, notation, destinationBySource);
        }

        private static void ValidatePermutation(IReadOnlyList<int> destinationBySource)
        {
            var seen = new bool[destinationBySource.Count];

            for (var source = 0; source < destinationBySource.Count; source++)
            {
                var destination = destinationBySource[source];

                if (destination < 0 || destination >= destinationBySource.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(destinationBySource), "Move destination is outside the puzzle state.");
                }

                if (seen[destination])
                {
                    throw new ArgumentException("Move destinations must form a permutation.", nameof(destinationBySource));
                }

                seen[destination] = true;
            }
        }
    }
}
