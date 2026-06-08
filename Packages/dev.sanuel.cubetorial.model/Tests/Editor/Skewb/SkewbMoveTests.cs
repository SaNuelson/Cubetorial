using System.Text;
using System.Linq;
using Cubetorial.Model.Base;
using Cubetorial.Model.Skewb;
using NUnit.Framework;

namespace Cubetorial.Model.Tests.Skewb
{
    public sealed class SkewbMoveTests
    {
        [Test]
        public void Create_BuildsExpectedSolvedPuzzle()
        {
            var puzzle = SkewbPuzzle.Create();

            Assert.That(puzzle.Id, Is.EqualTo("skewb"));
            Assert.That(puzzle.Faces, Has.Count.EqualTo(6));
            Assert.That(puzzle.Pieces, Has.Count.EqualTo(14));
            Assert.That(puzzle.Slots, Has.Count.EqualTo(14));
            Assert.That(puzzle.Moves.Select(move => move.Notation), Is.EquivalentTo(new[]
            {
                nameof(SkewbSlot.UBR),
                nameof(SkewbSlot.URF),
                nameof(SkewbSlot.UFL),
                nameof(SkewbSlot.ULB),
                nameof(SkewbSlot.DLF),
                nameof(SkewbSlot.DFR),
                nameof(SkewbSlot.DRB),
                nameof(SkewbSlot.DBL),
            }));
            Assert.That(puzzle.SolvedState.IsSolved(), Is.True);
        }

        [TestCase(nameof(SkewbSlot.UBR))]
        [TestCase(nameof(SkewbSlot.URF))]
        [TestCase(nameof(SkewbSlot.UFL))]
        [TestCase(nameof(SkewbSlot.ULB))]
        [TestCase(nameof(SkewbSlot.DLF))]
        [TestCase(nameof(SkewbSlot.DFR))]
        [TestCase(nameof(SkewbSlot.DRB))]
        [TestCase(nameof(SkewbSlot.DBL))]
        public void CornerMove_AppliedThreeTimes_ReturnsSolvedState(string notation)
        {
            var puzzle = SkewbPuzzle.Create();
            var state = puzzle.SolvedState;
            var move = puzzle.GetMove(notation);

            for (var i = 0; i < 3; i++)
            {
                state = state.Apply(move);
            }

            Assert.That(state.IsSolved(), Is.True, SkewbStateText.Dump(state));
        }

        [Test]
        public void URFMove_MovesDfrCornerToUflSlotWithDelta()
        {
            var puzzle = SkewbPuzzle.Create();
            var state = puzzle.SolvedState.Apply(puzzle.GetMove(nameof(SkewbSlot.URF)));

            var pieceInUfl = state.GetPieceInSlot(puzzle.GetSlotIndex(nameof(SkewbSlot.UFL)));
            var dfrSlot = puzzle.GetSlotIndex(nameof(SkewbSlot.DFR));

            Assert.That(pieceInUfl.PieceIndex, Is.EqualTo(dfrSlot), SkewbStateText.Dump(state));
            Assert.That(pieceInUfl.Orientation, Is.EqualTo(2), SkewbStateText.Dump(state));
        }

        [Test]
        public void URFMove_MovesFFaceToUSlot()
        {
            var puzzle = SkewbPuzzle.Create();
            var state = puzzle.SolvedState.Apply(puzzle.GetMove(nameof(SkewbSlot.URF)));

            var pieceInUs = state.GetPieceInSlot(puzzle.GetSlotIndex(nameof(SkewbSlot.U)));
            var fSlot = puzzle.GetSlotIndex(nameof(SkewbSlot.F));

            Assert.That(pieceInUs.PieceIndex, Is.EqualTo(fSlot), SkewbStateText.Dump(state));
            Assert.That(pieceInUs.Orientation, Is.EqualTo(0), SkewbStateText.Dump(state));
        }

        private static class SkewbStateText
        {
            private static readonly string[] CornerSlots =
            {
                nameof(SkewbSlot.URF),
                nameof(SkewbSlot.UBR),
                nameof(SkewbSlot.ULB),
                nameof(SkewbSlot.UFL),
                nameof(SkewbSlot.DFR),
                nameof(SkewbSlot.DRB),
                nameof(SkewbSlot.DBL),
                nameof(SkewbSlot.DLF)
            };

            private static readonly string[] CenterSlots =
            {
                nameof(SkewbSlot.U),
                nameof(SkewbSlot.D),
                nameof(SkewbSlot.L),
                nameof(SkewbSlot.R),
                nameof(SkewbSlot.F),
                nameof(SkewbSlot.B)
            };

            public static string Dump(PuzzleState state)
            {
                var builder = new StringBuilder();
                builder.AppendLine();
                builder.AppendLine("Skewb state dump: slot = piece/orientation");
                AppendGroup(builder, state, "Corners", CornerSlots);
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
