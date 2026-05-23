using System.Collections.Generic;
using System.Linq;

namespace Model.Base
{
    public sealed class PuzzlePiece
    {
        public PuzzlePiece(string id, IReadOnlyList<int> stickerIds)
        {
            Id = id;
            StickerIds = stickerIds.ToArray();
        }

        public string Id { get; }

        public IReadOnlyList<int> StickerIds { get; }
    }
}
