using System.Collections.Generic;
using System.Linq;

namespace Model.Base
{
    public sealed class PuzzleAlgorithm
    {
        public PuzzleAlgorithm(string id, string displayName, IReadOnlyList<PuzzleMove> moves)
        {
            Id = id;
            DisplayName = displayName;
            Moves = moves.ToArray();
        }

        public string Id { get; }

        public string DisplayName { get; }

        public IReadOnlyList<PuzzleMove> Moves { get; }

        public PuzzleState ApplyTo(PuzzleState state)
        {
            return state.Apply(Moves);
        }
    }
}
