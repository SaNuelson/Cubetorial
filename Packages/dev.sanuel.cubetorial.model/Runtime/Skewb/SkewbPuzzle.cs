using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common.Merge;
using Cubetorial.Model.Base;

namespace Cubetorial.Model.Skewb
{
    public class SkewbPuzzle
    {
        public const PuzzleFamily Family = PuzzleFamily.Skewb;
        
        public static Puzzle Create()
        {
            var corner = new PieceKind("corner", 3);
            var center = new PieceKind("center", 1);

            var faces = new[]
            {
                new Face(nameof(SkewbFace.U), "Up"),
                new Face(nameof(SkewbFace.D), "Down"),
                new Face(nameof(SkewbFace.L), "Left"),
                new Face(nameof(SkewbFace.R), "Right"),
                new Face(nameof(SkewbFace.F), "Front"),
                new Face(nameof(SkewbFace.B), "Back")
            };

            var pieceSlots = new[]
            {
                CreatePieceSlot(SkewbSlot.URF, corner, "U", "F", "R"),
                CreatePieceSlot(SkewbSlot.UBR, corner, "U", "R", "B"),
                CreatePieceSlot(SkewbSlot.ULB, corner, "U", "B", "L"),
                CreatePieceSlot(SkewbSlot.UFL, corner, "U", "L", "F"),
                CreatePieceSlot(SkewbSlot.DFR, corner, "D", "R", "F"),
                CreatePieceSlot(SkewbSlot.DRB, corner, "D", "B", "R"),
                CreatePieceSlot(SkewbSlot.DBL, corner, "D", "L", "B"),
                CreatePieceSlot(SkewbSlot.DLF, corner, "D", "F", "L"),

                CreatePieceSlot(SkewbSlot.U, center, "U"),
                CreatePieceSlot(SkewbSlot.D, center, "D"),
                CreatePieceSlot(SkewbSlot.L, center, "L"),
                CreatePieceSlot(SkewbSlot.R, center, "R"),
                CreatePieceSlot(SkewbSlot.F, center, "F"),
                CreatePieceSlot(SkewbSlot.B, center, "B")
            };

            var pieces = new List<Piece>();
            var slots = new List<PieceSlot>();

            foreach (var pieceSlot in pieceSlots)
            {
                pieces.Add(pieceSlot.Piece);
                slots.Add(pieceSlot.Slot);
            }

            var slotIndex = CreateSlotIndex(pieceSlots);
            var moves = new[]
            {
                // Axis corner URF (delta++)
                // Neighbor faces U, R, F (cycle)
                // Neighbor corners UFL, UBR, DFR (cycle, delta +,-,)
                CreateMove(
                    SkewbSlot.URF,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.U, SkewbFace.R, SkewbFace.F}),
                CreateMove( 
                    SkewbSlot.UBR,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.U, SkewbFace.B, SkewbFace.R}),
                CreateMove( 
                    SkewbSlot.ULB,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.U, SkewbFace.L, SkewbFace.B}),
                CreateMove( 
                    SkewbSlot.UFL,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.U, SkewbFace.F, SkewbFace.L}),
                CreateMove(
                    SkewbSlot.DFR,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.D, SkewbFace.F, SkewbFace.R}),
                CreateMove( 
                    SkewbSlot.DRB,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.D, SkewbFace.R, SkewbFace.B}),
                CreateMove( 
                    SkewbSlot.DBL,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.D, SkewbFace.B, SkewbFace.L}),
                CreateMove( 
                    SkewbSlot.DLF,
                    slotIndex,
                    new List<SkewbFace>() {SkewbFace.D, SkewbFace.L, SkewbFace.F}),
            };

            return new Model.Base.Puzzle(Family.ToString(), faces, pieces, slots, moves);
        }

        private static PieceSlotPair CreatePieceSlot(
            SkewbSlot slot,
            PieceKind kind,
            params string[] faceIds)
        {
            return new PieceSlotPair(
                slot,
                new Piece(slot.ToString(), kind, faceIds),
                new PieceSlot(slot.ToString(), kind, faceIds));
        }

        private static Dictionary<SkewbSlot, int> CreateSlotIndex(IReadOnlyList<PieceSlotPair> pieceSlots)
        {
            var result = new Dictionary<SkewbSlot, int>();

            for (var i = 0; i < pieceSlots.Count; i++)
            {
                result.Add(pieceSlots[i].SlotId, i);
            }

            return result;
        }
        
        private static PuzzleMove CreateMove(
            SkewbSlot axis,
            IReadOnlyDictionary<SkewbSlot, int> slotIndex,
            List<SkewbFace> faceTransition)
        {
            var moves = new List<SlotMove>();

            // Axis corner gets rotated
            AddRotation(moves, slotIndex, axis);
            
            // Neighboring faces get moved in cycle
            AddCycle(moves, slotIndex, faceTransition.Select(t => ToSlot(t)).ToList(), 0);
            
            // Neighboring corners are those with 1 face flipped
            var cornerCycle = faceTransition
                .Select(face => axis ^ (SkewbSlot)face.ToMask() ^ (SkewbSlot)face.ToMask().Opposite())
                .ToList();
            
            var cornerOrientationDeltas = new int[cornerCycle.Count];
            for (var i = 0; i < cornerCycle.Count; i++)
            {
                var source = cornerCycle[i];
                var destination = cornerCycle[(i + 1) % cornerCycle.Count];
                var sourceReferenceFacelet = Definition.ReferenceFacelets[source];
                var rotatedReferenceFacelet = ApplyFaceTransition(faceTransition, sourceReferenceFacelet);
                
                cornerOrientationDeltas[i] = Definition.GetCornerOrientationDelta(destination, rotatedReferenceFacelet);
            }
            
            AddCycle(moves, slotIndex, cornerCycle, cornerOrientationDeltas);

            return new PuzzleMove(axis.ToString(), moves);
        }

        private static SkewbFace ApplyFaceTransition(List<SkewbFace> faceTransition, SkewbFace face)
        {
            var index = faceTransition.IndexOf(face);

            if (index >= 0)
            {
                return faceTransition[(index + 1) % faceTransition.Count];
            }

            var oppositeFace = ToFace(face.ToMask().Opposite());
            var oppositeIndex = faceTransition.IndexOf(oppositeFace);

            if (oppositeIndex < 0)
            {
                return face;
            }

            return ToFace(faceTransition[(oppositeIndex + 1) % faceTransition.Count].ToMask().Opposite());
        }

        private static SkewbFace ToFace(SkewbFaceMask mask)
        {
            return mask switch
            {
                SkewbFaceMask.U => SkewbFace.U,
                SkewbFaceMask.D => SkewbFace.D,
                SkewbFaceMask.F => SkewbFace.F,
                SkewbFaceMask.B => SkewbFace.B,
                SkewbFaceMask.R => SkewbFace.R,
                SkewbFaceMask.L => SkewbFace.L,
                _ => throw new ArgumentOutOfRangeException(nameof(mask), mask, null)
            };
        }

        private static SkewbSlot ToSlot(params SkewbFace[] faces)
        {
            var mask = default(SkewbFaceMask);

            foreach (var face in faces)
            {
                mask |= face.ToMask();
            }

            return (SkewbSlot)mask;
        }

        private static void AddCycle(
            List<SlotMove> moves,
            IReadOnlyDictionary<SkewbSlot, int> slotIndex,
            IReadOnlyList<SkewbSlot> cycle,
            IReadOnlyList<int> orientationDeltas)
        {
            for (var i = 0; i < cycle.Count; i++)
            {
                var source = cycle[i];
                var destination = cycle[(i + 1) % cycle.Count];
                moves.Add(new SlotMove(slotIndex[source], slotIndex[destination], orientationDeltas[i]));
            }
        }

        private static void AddCycle(
            List<SlotMove> moves,
            IReadOnlyDictionary<SkewbSlot, int> slotIndex,
            IReadOnlyList<SkewbSlot> cycle,
            int orientationDelta)
        {
            AddCycle(moves, slotIndex, cycle, Enumerable.Repeat(orientationDelta, cycle.Count).ToArray());
        }

        private static void AddRotation(
            List<SlotMove> moves,
            IReadOnlyDictionary<SkewbSlot, int> slotIndex,
            SkewbSlot piece,
            int delta = 1)
        {
            moves.Add(new SlotMove(slotIndex[piece], slotIndex[piece], delta));
        }

        private sealed class PieceSlotPair
        {
            public PieceSlotPair(SkewbSlot slotId, Piece piece, PieceSlot slot)
            {
                SlotId = slotId;
                Piece = piece;
                Slot = slot;
            }

            public SkewbSlot SlotId { get; }

            public Piece Piece { get; }

            public PieceSlot Slot { get; }
        }
    }
}
