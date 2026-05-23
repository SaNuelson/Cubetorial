using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Base
{
    public sealed class PuzzleState
    {
        private readonly int[] _stickerAtPosition;

        public PuzzleState(PuzzleDefinition definition, IReadOnlyList<int> stickerAtPosition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            _stickerAtPosition = stickerAtPosition.ToArray();
            ValidateStickerMapping(definition, _stickerAtPosition);
        }

        public PuzzleDefinition Definition { get; }

        public IReadOnlyList<int> StickerAtPosition => _stickerAtPosition;

        public int GetStickerAt(int positionId)
        {
            return _stickerAtPosition[positionId];
        }

        public PuzzleState Apply(PuzzleMove move)
        {
            if (move.PositionCount != _stickerAtPosition.Length)
            {
                throw new ArgumentException("Move does not match this puzzle state.", nameof(move));
            }

            var next = new int[_stickerAtPosition.Length];

            for (var source = 0; source < _stickerAtPosition.Length; source++)
            {
                next[move.DestinationBySource[source]] = _stickerAtPosition[source];
            }

            return new PuzzleState(Definition, next);
        }

        public PuzzleState Apply(IEnumerable<PuzzleMove> moves)
        {
            var state = this;

            foreach (var move in moves)
            {
                state = state.Apply(move);
            }

            return state;
        }

        public bool IsSolved()
        {
            for (var position = 0; position < _stickerAtPosition.Length; position++)
            {
                if (_stickerAtPosition[position] != position)
                {
                    return false;
                }
            }

            return true;
        }

        public static PuzzleState CreateSolved(PuzzleDefinition definition)
        {
            var stickers = new int[definition.StickerPositions.Count];

            for (var position = 0; position < stickers.Length; position++)
            {
                stickers[position] = position;
            }

            return new PuzzleState(definition, stickers);
        }

        private static void ValidateStickerMapping(PuzzleDefinition definition, IReadOnlyList<int> stickerAtPosition)
        {
            if (stickerAtPosition.Count != definition.StickerPositions.Count)
            {
                throw new ArgumentException("Sticker mapping length must match the puzzle definition.", nameof(stickerAtPosition));
            }

            var seen = new bool[stickerAtPosition.Count];

            for (var position = 0; position < stickerAtPosition.Count; position++)
            {
                var sticker = stickerAtPosition[position];

                if (sticker < 0 || sticker >= stickerAtPosition.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(stickerAtPosition), "Sticker id is outside the puzzle definition.");
                }

                if (seen[sticker])
                {
                    throw new ArgumentException("Sticker mapping must be a permutation.", nameof(stickerAtPosition));
                }

                seen[sticker] = true;
            }
        }
    }
}
