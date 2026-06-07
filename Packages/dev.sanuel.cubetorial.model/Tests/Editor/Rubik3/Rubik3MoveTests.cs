using System.Text;
using System.Linq;
using Cubetorial.Model.Base;
using Cubetorial.Model.Rubik3;
using NUnit.Framework;

namespace Cubetorial.Model.Tests.Rubik3
{
    public sealed class Rubik3MoveTests
    {
        [Test]
        public void Create_BuildsExpectedSolvedPuzzle()
        {
            var puzzle = Rubik3Puzzle.Create();

            Assert.That(puzzle.Id, Is.EqualTo("rubik-3"));
            Assert.That(puzzle.Faces, Has.Count.EqualTo(6));
            Assert.That(puzzle.Pieces, Has.Count.EqualTo(26));
            Assert.That(puzzle.Slots, Has.Count.EqualTo(26));
            Assert.That(puzzle.Moves.Select(move => move.Notation), Is.EquivalentTo(new[] { "R", "L", "U", "D", "F", "B" }));
            Assert.That(puzzle.SolvedState.IsSolved(), Is.True);
        }

        [TestCase("R")]
        [TestCase("L")]
        [TestCase("U")]
        [TestCase("D")]
        [TestCase("F")]
        [TestCase("B")]
        public void FaceMove_AppliedFourTimes_ReturnsSolvedState(string notation)
        {
            var puzzle = Rubik3Puzzle.Create();
            var state = puzzle.SolvedState;
            var move = puzzle.GetMove(notation);

            for (var i = 0; i < 4; i++)
            {
                state = state.Apply(move);
            }

            Assert.That(state.IsSolved(), Is.True, Rubik3StateText.Dump(state));
        }

        [Test]
        public void RMove_MovesDfrCornerToUrfWithKociembaOrientation()
        {
            var puzzle = Rubik3Puzzle.Create();
            var state = puzzle.SolvedState.Apply(puzzle.GetMove("R"));

            var urf = state.GetPieceInSlot(puzzle.GetSlotIndex(nameof(Rubik3Slot.URF)));
            var dfrPieceIndex = puzzle.GetSlotIndex(nameof(Rubik3Slot.DFR));

            Assert.That(urf.PieceIndex, Is.EqualTo(dfrPieceIndex), Rubik3StateText.Dump(state));
            Assert.That(urf.Orientation, Is.EqualTo(2), Rubik3StateText.Dump(state));
        }

        [Test]
        public void RMove_MovesUrEdgeToBrWithoutFlipping()
        {
            var puzzle = Rubik3Puzzle.Create();
            var state = puzzle.SolvedState.Apply(puzzle.GetMove("R"));

            var br = state.GetPieceInSlot(puzzle.GetSlotIndex(nameof(Rubik3Slot.BR)));
            var urPieceIndex = puzzle.GetSlotIndex(nameof(Rubik3Slot.UR));

            Assert.That(br.PieceIndex, Is.EqualTo(urPieceIndex), Rubik3StateText.Dump(state));
            Assert.That(br.Orientation, Is.EqualTo(0), Rubik3StateText.Dump(state));
        }

        [TestCase(Rubik3Slot.URF, Rubik3Face.U, 0)]
        [TestCase(Rubik3Slot.URF, Rubik3Face.R, 1)]
        [TestCase(Rubik3Slot.URF, Rubik3Face.F, 2)]
        [TestCase(Rubik3Slot.DFR, Rubik3Face.D, 0)]
        [TestCase(Rubik3Slot.DFR, Rubik3Face.F, 1)]
        [TestCase(Rubik3Slot.DFR, Rubik3Face.R, 2)]
        public void GetCornerOrientationDelta_UsesSlotFaceletOrder(Rubik3Slot slot, Rubik3Face face, int expectedDelta)
        {
            Assert.That(Definition.GetCornerOrientationDelta(slot, face), Is.EqualTo(expectedDelta));
        }

        [TestCase(Rubik3Slot.UR, Rubik3Face.U, 0)]
        [TestCase(Rubik3Slot.UR, Rubik3Face.R, 1)]
        [TestCase(Rubik3Slot.FR, Rubik3Face.F, 0)]
        [TestCase(Rubik3Slot.FR, Rubik3Face.R, 1)]
        public void GetEdgeOrientationDelta_UsesReferenceFacelet(Rubik3Slot slot, Rubik3Face face, int expectedDelta)
        {
            Assert.That(Definition.GetEdgeOrientationDelta(slot, face), Is.EqualTo(expectedDelta));
        }

        private static class Rubik3StateText
        {
            private static readonly string[] CornerSlots =
            {
                nameof(Rubik3Slot.URF),
                nameof(Rubik3Slot.UBR),
                nameof(Rubik3Slot.ULB),
                nameof(Rubik3Slot.UFL),
                nameof(Rubik3Slot.DFR),
                nameof(Rubik3Slot.DRB),
                nameof(Rubik3Slot.DBL),
                nameof(Rubik3Slot.DLF)
            };

            private static readonly string[] EdgeSlots =
            {
                nameof(Rubik3Slot.UF),
                nameof(Rubik3Slot.UR),
                nameof(Rubik3Slot.UB),
                nameof(Rubik3Slot.UL),
                nameof(Rubik3Slot.FR),
                nameof(Rubik3Slot.BR),
                nameof(Rubik3Slot.BL),
                nameof(Rubik3Slot.FL),
                nameof(Rubik3Slot.DF),
                nameof(Rubik3Slot.DR),
                nameof(Rubik3Slot.DB),
                nameof(Rubik3Slot.DL)
            };

            private static readonly string[] CenterSlots =
            {
                nameof(Rubik3Slot.U),
                nameof(Rubik3Slot.D),
                nameof(Rubik3Slot.L),
                nameof(Rubik3Slot.R),
                nameof(Rubik3Slot.F),
                nameof(Rubik3Slot.B)
            };

            public static string Dump(PuzzleState state)
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                builder.AppendLine("Rubik3 state dump: slot = piece/orientation");
                AppendGroup(builder, state, "Corners", CornerSlots);
                AppendGroup(builder, state, "Edges", EdgeSlots);
                AppendGroup(builder, state, "Centers", CenterSlots);
                return builder.ToString();
            }

            private static void AppendGroup(StringBuilder builder, PuzzleState state, string title, string[] slotIds)
            {
                builder.AppendLine(title + ":");

                foreach (var slotId in slotIds)
                {
                    var slotIndex = state.Puzzle.GetSlotIndex(slotId);
                    var placedPiece = state.GetPieceInSlot(slotIndex);
                    var piece = state.Puzzle.Pieces[placedPiece.PieceIndex];

                    builder
                        .Append("  ")
                        .Append(slotId)
                        .Append(" = ")
                        .Append(piece.Id)
                        .Append("/")
                        .Append(placedPiece.Orientation)
                        .AppendLine();
                }
            }
        }
    }
}
