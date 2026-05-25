using System.Collections.Generic;
using System.Linq;

namespace Cubetorial.Model.Base
{
    public sealed class PieceKind
    {
        public PieceKind(string id, int orientationCount)
        {
            Id = id;
            OrientationCount = orientationCount;
        }

        public string Id { get; }

        public int OrientationCount { get; }
    }

    public sealed class Piece
    {
        public Piece(string id, PieceKind kind, IEnumerable<string> stickerFaceIds)
        {
            Id = id;
            Kind = kind;
            StickerFaceIds = stickerFaceIds.ToArray();
        }

        public string Id { get; }

        public PieceKind Kind { get; }

        public IReadOnlyList<string> StickerFaceIds { get; }
    }
}
