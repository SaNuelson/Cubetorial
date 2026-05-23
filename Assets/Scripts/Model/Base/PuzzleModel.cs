namespace Model.Base
{
    public abstract class PuzzleModel
    {
        protected PuzzleModel(PuzzleDefinition definition)
        {
            Definition = definition;
            State = definition.SolvedState;
        }

        public PuzzleDefinition Definition { get; }

        public PuzzleState State { get; private set; }

        public void Apply(PuzzleMove move)
        {
            State = State.Apply(move);
        }

        public void Reset()
        {
            State = Definition.SolvedState;
        }
    }
}
