using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubetorial.Model.Base
{
    public sealed class PieceSlot
    {
        public PieceSlot(string id, PieceKind kind, IEnumerable<string> surfaceFaceIds)
        {
            Id = id;
            Kind = kind;
            SurfaceFaceIds = surfaceFaceIds.ToArray();
        }

        public string Id { get; }

        public PieceKind Kind { get; }

        public IReadOnlyList<string> SurfaceFaceIds { get; }
    }

    /// <summary>
    /// Identifies a physical piece and its orientation while it occupies a slot.
    /// </summary>
    /// <remarks>
    /// The slot is implied by this value's index in <see cref="PuzzleState.PiecesBySlot"/>.
    /// </remarks>
    public readonly struct PlacedPiece
    {
        public PlacedPiece(int pieceIndex, int orientation)
        {
            PieceIndex = pieceIndex;
            Orientation = orientation;
        }

        public int PieceIndex { get; }

        public int Orientation { get; }
    }

    /// <summary>
    /// Describes one piece transfer within a <see cref="PuzzleMove"/>:
    /// source slot, destination slot, and orientation delta.
    /// </summary>
    public sealed class SlotMove
    {
        public SlotMove(int sourceSlotIndex, int destinationSlotIndex, int orientationDelta)
        {
            SourceSlotIndex = sourceSlotIndex;
            DestinationSlotIndex = destinationSlotIndex;
            OrientationDelta = orientationDelta;
        }

        public int SourceSlotIndex { get; }

        public int DestinationSlotIndex { get; }

        public int OrientationDelta { get; }
    }

    /// <summary>
    /// Definition of a puzzle move, defines a transition of puzzle pieces via <see cref="SlotMove"/>.
    /// </summary>
    /// <code>
    /// var rubik3Moves = [
    ///   new PuzzleMove("R", rMoveTransforms),
    ///   new PuzzleMove("U", uMoveTransforms),
    ///   // ...
    /// ];
    /// </code>
    public sealed class PuzzleMove
    {
        public PuzzleMove(string notation, IEnumerable<SlotMove> slotMoves)
        {
            Notation = notation;
            SlotMoves = slotMoves.ToArray();
        }

        public string Notation { get; }

        public IReadOnlyList<SlotMove> SlotMoves { get; }
    }

    /// <summary>
    /// A model holding a specific scramble of a puzzle.
    ///
    /// Contains mapping of puzzle pieces to slots they currently occupy (piecesBySlot).
    ///
    /// Can be provided a <see cref="PuzzleMove"/> to obtain a resulting new <see cref="PuzzleState"/>.
    /// </summary>
    public sealed class PuzzleState
    {
        private readonly PlacedPiece[] piecesBySlot;

        public PuzzleState(Puzzle puzzle, IEnumerable<PlacedPiece> piecesBySlot)
        {
            Puzzle = puzzle;
            this.piecesBySlot = piecesBySlot.ToArray();

            if (this.piecesBySlot.Length != puzzle.Slots.Count)
            {
                throw new ArgumentException("State must contain one piece for every puzzle slot.");
            }
        }

        public Puzzle Puzzle { get; }

        public IReadOnlyList<PlacedPiece> PiecesBySlot => piecesBySlot;

        public PlacedPiece GetPieceInSlot(int slotIndex)
        {
            return piecesBySlot[slotIndex];
        }

        public PuzzleState Apply(PuzzleMove move)
        {
            var next = piecesBySlot.ToArray();

            foreach (var slotMove in move.SlotMoves)
            {
                var sourcePiece = piecesBySlot[slotMove.SourceSlotIndex];
                var piece = Puzzle.Pieces[sourcePiece.PieceIndex];
                var orientationCount = piece.Kind.OrientationCount;
                var nextOrientation = orientationCount <= 1
                    ? 0
                    : Mod(sourcePiece.Orientation + slotMove.OrientationDelta, orientationCount);

                next[slotMove.DestinationSlotIndex] = new PlacedPiece(sourcePiece.PieceIndex, nextOrientation);
            }

            return new PuzzleState(Puzzle, next);
        }

        public bool IsSolved()
        {
            for (var slotIndex = 0; slotIndex < piecesBySlot.Length; slotIndex++)
            {
                var placedPiece = piecesBySlot[slotIndex];

                if (placedPiece.PieceIndex != slotIndex || placedPiece.Orientation != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static int Mod(int value, int divisor)
        {
            var result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }

    /// <summary>
    /// Contains a definition of a specific type of puzzle (e.g., 3x3, Skewb, Pyraminx).
    ///
    /// Contains its name, faces, pieces, slots, and a valid set of moves.
    /// </summary>
    public sealed class Puzzle
    {
        private readonly Dictionary<string, int> slotIndexById;
        private readonly Dictionary<string, PuzzleMove> moveByNotation;

        public Puzzle(
            string id,
            IEnumerable<Face> faces,
            IEnumerable<Piece> pieces,
            IEnumerable<PieceSlot> slots,
            IEnumerable<PuzzleMove> moves)
        {
            Id = id;
            Faces = faces.ToList();
            Pieces = pieces.ToList();
            Slots = slots.ToList();
            Moves = moves.ToList();
            slotIndexById = Slots
                .Select((slot, index) => new { slot.Id, Index = index })
                .ToDictionary(slot => slot.Id, slot => slot.Index);
            moveByNotation = Moves.ToDictionary(move => move.Notation);

            Validate();
            SolvedState = CreateSolvedState();
        }

        public string Id { get; }

        public List<Face> Faces { get; }

        public List<Piece> Pieces { get; }

        public List<PieceSlot> Slots { get; }

        public List<PuzzleMove> Moves { get; }

        public PuzzleState SolvedState { get; }

        public int GetSlotIndex(string slotId)
        {
            return slotIndexById[slotId];
        }

        public PuzzleMove GetMove(string notation)
        {
            return moveByNotation[notation];
        }

        private PuzzleState CreateSolvedState()
        {
            var solvedPieces = Pieces
                .Select((piece, index) => new PlacedPiece(index, 0))
                .ToArray();

            return new PuzzleState(this, solvedPieces);
        }

        private void Validate()
        {
            if (Pieces.Count != Slots.Count)
            {
                throw new ArgumentException("This simple draft expects one piece for every slot.");
            }

            for (var index = 0; index < Pieces.Count; index++)
            {
                if (Pieces[index].Kind != Slots[index].Kind)
                {
                    throw new ArgumentException($"Solved piece '{Pieces[index].Id}' does not fit slot '{Slots[index].Id}'.");
                }
            }
        }
    }
}
