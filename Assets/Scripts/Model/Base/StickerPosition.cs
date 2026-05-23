namespace Model.Base
{
    public sealed class StickerPosition
    {
        public StickerPosition(int id, string piecePositionId, string faceId, string slotId)
        {
            Id = id;
            PiecePositionId = piecePositionId;
            FaceId = faceId;
            SlotId = slotId;
        }

        public int Id { get; }

        public string PiecePositionId { get; }

        public string FaceId { get; }

        public string SlotId { get; }
    }
}
