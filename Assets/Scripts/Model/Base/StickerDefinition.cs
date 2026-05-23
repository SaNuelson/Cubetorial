namespace Model.Base
{
    public sealed class StickerDefinition
    {
        public StickerDefinition(int id, string pieceId, string faceId, string colorId)
        {
            Id = id;
            PieceId = pieceId;
            FaceId = faceId;
            ColorId = colorId;
        }

        public int Id { get; }

        public string PieceId { get; }

        public string FaceId { get; }

        public string ColorId { get; }
    }
}
