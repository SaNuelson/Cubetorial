using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Base
{
    public sealed class PuzzleDefinition
    {
        private readonly Dictionary<string, PuzzleMove> _movesByNotation;

        public PuzzleDefinition(
            string id,
            string displayName,
            IReadOnlyList<StickerPosition> stickerPositions,
            IReadOnlyList<StickerDefinition> stickers,
            IReadOnlyList<PuzzleMove> moves)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Puzzle id cannot be empty.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Puzzle display name cannot be empty.", nameof(displayName));
            }

            if (stickerPositions.Count != stickers.Count)
            {
                throw new ArgumentException("The solved draft expects one sticker per sticker position.");
            }

            Id = id;
            DisplayName = displayName;
            StickerPositions = stickerPositions.ToArray();
            Stickers = stickers.ToArray();
            Moves = moves.ToArray();
            _movesByNotation = Moves.ToDictionary(move => move.Notation, StringComparer.Ordinal);

            foreach (var move in Moves)
            {
                if (move.PositionCount != StickerPositions.Count)
                {
                    throw new ArgumentException($"Move '{move.Notation}' does not match the puzzle position count.");
                }
            }

            SolvedState = PuzzleState.CreateSolved(this);
        }

        public string Id { get; }

        public string DisplayName { get; }

        public IReadOnlyList<StickerPosition> StickerPositions { get; }

        public IReadOnlyList<StickerDefinition> Stickers { get; }

        public IReadOnlyList<PuzzleMove> Moves { get; }

        public PuzzleState SolvedState { get; }

        public PuzzleMove GetMove(string notation)
        {
            if (!_movesByNotation.TryGetValue(notation, out var move))
            {
                throw new KeyNotFoundException($"Puzzle '{DisplayName}' does not define move '{notation}'.");
            }

            return move;
        }
    }
}
