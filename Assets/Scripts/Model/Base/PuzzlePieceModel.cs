namespace Model.Base
{
    public sealed class PuzzlePieceModel
    {
        public PuzzlePieceModel(string id, string kind)
        {
            Id = id;
            Kind = kind;
        }

        public string Id { get; }

        public string Kind { get; }
    }
}
