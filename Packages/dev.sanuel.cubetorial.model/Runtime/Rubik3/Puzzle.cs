using System.Collections.Generic;
using System.Linq;
using Cubetorial.Model.Base;

namespace Cubetorial.Model.Rubik3
{
    public static class Rubik3Puzzle
    {
        public static Puzzle Create()
        {
            var corner = new PieceKind("corner", 3);
            var edge = new PieceKind("edge", 2);
            var center = new PieceKind("center", 1);

            var faces = new[]
            {
                new Face(nameof(Rubik3Face.U), "Up"),
                new Face(nameof(Rubik3Face.D), "Down"),
                new Face(nameof(Rubik3Face.L), "Left"),
                new Face(nameof(Rubik3Face.R), "Right"),
                new Face(nameof(Rubik3Face.F), "Front"),
                new Face(nameof(Rubik3Face.B), "Back")
            };

            var pieceSlots = new[]
            {
                CreatePieceSlot(Rubik3Slot.URF, corner, "U", "F", "R"),
                CreatePieceSlot(Rubik3Slot.UBR, corner, "U", "R", "B"),
                CreatePieceSlot(Rubik3Slot.ULB, corner, "U", "B", "L"),
                CreatePieceSlot(Rubik3Slot.UFL, corner, "U", "L", "F"),
                CreatePieceSlot(Rubik3Slot.DFR, corner, "D", "R", "F"),
                CreatePieceSlot(Rubik3Slot.DRB, corner, "D", "B", "R"),
                CreatePieceSlot(Rubik3Slot.DBL, corner, "D", "L", "B"),
                CreatePieceSlot(Rubik3Slot.DLF, corner, "D", "F", "L"),

                CreatePieceSlot(Rubik3Slot.UF, edge, "U", "F"),
                CreatePieceSlot(Rubik3Slot.UR, edge, "U", "R"),
                CreatePieceSlot(Rubik3Slot.UB, edge, "U", "B"),
                CreatePieceSlot(Rubik3Slot.UL, edge, "U", "L"),
                CreatePieceSlot(Rubik3Slot.FR, edge, "F", "R"),
                CreatePieceSlot(Rubik3Slot.BR, edge, "B", "R"),
                CreatePieceSlot(Rubik3Slot.BL, edge, "B", "L"),
                CreatePieceSlot(Rubik3Slot.FL, edge, "F", "L"),
                CreatePieceSlot(Rubik3Slot.DF, edge, "D", "F"),
                CreatePieceSlot(Rubik3Slot.DR, edge, "D", "R"),
                CreatePieceSlot(Rubik3Slot.DB, edge, "D", "B"),
                CreatePieceSlot(Rubik3Slot.DL, edge, "D", "L"),

                CreatePieceSlot(Rubik3Slot.U, center, "U"),
                CreatePieceSlot(Rubik3Slot.D, center, "D"),
                CreatePieceSlot(Rubik3Slot.L, center, "L"),
                CreatePieceSlot(Rubik3Slot.R, center, "R"),
                CreatePieceSlot(Rubik3Slot.F, center, "F"),
                CreatePieceSlot(Rubik3Slot.B, center, "B")
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
                CreateMove(
                    Rubik3Face.R,
                    slotIndex,
                    new List<Rubik3Face>() {Rubik3Face.U, Rubik3Face.B, Rubik3Face.D, Rubik3Face.F}),
                CreateMove( 
                    Rubik3Face.L,
                    slotIndex,
                    new List<Rubik3Face>() {Rubik3Face.F, Rubik3Face.D, Rubik3Face.B, Rubik3Face.U}),
                CreateMove( 
                    Rubik3Face.U,
                    slotIndex,
                    new List<Rubik3Face>() {Rubik3Face.F, Rubik3Face.L, Rubik3Face.B, Rubik3Face.R}),
                CreateMove( 
                    Rubik3Face.D,
                    slotIndex,
                    new List<Rubik3Face>() {Rubik3Face.F, Rubik3Face.R, Rubik3Face.B, Rubik3Face.L}),
                CreateMove( 
                    Rubik3Face.F,
                    slotIndex,
                    new List<Rubik3Face>() {Rubik3Face.U, Rubik3Face.R, Rubik3Face.D, Rubik3Face.L}),
                CreateMove( 
                    Rubik3Face.B,
                    slotIndex,
                    new List<Rubik3Face>() {Rubik3Face.D, Rubik3Face.R, Rubik3Face.U, Rubik3Face.L}),
            };

            return new Model.Base.Puzzle("rubik-3", faces, pieces, slots, moves);
        }

        private static PieceSlotPair CreatePieceSlot(
            Rubik3Slot slot,
            PieceKind kind,
            params string[] faceIds)
        {
            return new PieceSlotPair(
                slot,
                new Piece(slot.ToString(), kind, faceIds),
                new PieceSlot(slot.ToString(), kind, faceIds));
        }

        private static Dictionary<Rubik3Slot, int> CreateSlotIndex(IReadOnlyList<PieceSlotPair> pieceSlots)
        {
            var result = new Dictionary<Rubik3Slot, int>();

            for (var i = 0; i < pieceSlots.Count; i++)
            {
                result.Add(pieceSlots[i].SlotId, i);
            }

            return result;
        }
        
        private static PuzzleMove CreateMove(
            Rubik3Face axis,
            IReadOnlyDictionary<Rubik3Slot, int> slotIndex,
            List<Rubik3Face> faceTransition)
        {
            var moves = new List<SlotMove>();

            // Edge transitions follow transitions + axis
            var edgeCycle = faceTransition
                .Select(t => (Rubik3Slot)t | (Rubik3Slot)axis)
                .ToList();
            
            // Corner transitions follow transition pairs + axis
            var cornerCycle = faceTransition
                .Select((t, i) => (Rubik3Slot)(faceTransition[(i - 1 + faceTransition.Count) % faceTransition.Count] | t | axis)) // TODO: Any nicer way to do this?
                .ToList();
            
            var cornerOrientationDeltas = new int[cornerCycle.Count];
            for (var i = 0; i < cornerCycle.Count; i++)
            {
                var cubie = cornerCycle[(i - 1 + cornerCycle.Count) % cornerCycle.Count];
                var cubieReferenceFacelet = Definition.ReferenceFacelets[cubie];
                var rotatedCubieFacelet = faceTransition[(faceTransition.IndexOf(cubieReferenceFacelet) + 1) % faceTransition.Count];
                
                cornerOrientationDeltas[i] = Definition.GetCornerOrientationDelta(cubie, rotatedCubieFacelet);
            }
            
            var edgeOrientationDeltas = new int[edgeCycle.Count];
            for (var i = 0; i < edgeCycle.Count; i++)
            {
                var cubie = edgeCycle[(i - 1 + edgeCycle.Count) % edgeCycle.Count];
                var cubieReferenceFacelet = Definition.ReferenceFacelets[cubie];
                var rotatedCubieFacelet = faceTransition[(faceTransition.IndexOf(cubieReferenceFacelet) + 1) % faceTransition.Count];
                
                edgeOrientationDeltas[i] = Definition.GetEdgeOrientationDelta(cubie, rotatedCubieFacelet);
            }
            
            AddCycle(moves, slotIndex, cornerCycle, cornerOrientationDeltas);
            AddCycle(moves, slotIndex, edgeCycle, edgeOrientationDeltas);

            return new PuzzleMove(axis.ToString(), moves);
        }

        private static void AddCycle(
            List<SlotMove> moves,
            IReadOnlyDictionary<Rubik3Slot, int> slotIndex,
            IReadOnlyList<Rubik3Slot> cycle,
            IReadOnlyList<int> orientationDeltas)
        {
            for (var i = 0; i < cycle.Count; i++)
            {
                var source = cycle[i];
                var destination = cycle[(i + 1) % cycle.Count];
                moves.Add(new SlotMove(slotIndex[source], slotIndex[destination], orientationDeltas[i]));
            }
        }

        private sealed class PieceSlotPair
        {
            public PieceSlotPair(Rubik3Slot slotId, Piece piece, PieceSlot slot)
            {
                SlotId = slotId;
                Piece = piece;
                Slot = slot;
            }

            public Rubik3Slot SlotId { get; }

            public Piece Piece { get; }

            public PieceSlot Slot { get; }
        }
    }
}
