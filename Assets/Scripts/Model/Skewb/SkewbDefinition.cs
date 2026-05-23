using System;
using System.Collections.Generic;
using System.Linq;
using Model.Base;

namespace Model.Skewb
{
    public static class SkewbDefinition
    {
        private static readonly FaceSpec[] Faces =
        {
            new("U", "White", new Coord(0, 1, 0)),
            new("D", "Yellow", new Coord(0, -1, 0)),
            new("F", "Green", new Coord(0, 0, 1)),
            new("B", "Blue", new Coord(0, 0, -1)),
            new("R", "Red", new Coord(1, 0, 0)),
            new("L", "Orange", new Coord(-1, 0, 0))
        };

        private static readonly Coord[] CornerPositions =
        {
            new(1, 1, 1),
            new(-1, 1, 1),
            new(-1, 1, -1),
            new(1, 1, -1),
            new(1, -1, 1),
            new(-1, -1, 1),
            new(-1, -1, -1),
            new(1, -1, -1)
        };

        public static PuzzleDefinition Create()
        {
            var draft = CreateStickerDraft();
            var moves = new List<PuzzleMove>();

            AddCornerTurnFamily(moves, "URF", draft, new Coord(1, 1, 1));
            AddCornerTurnFamily(moves, "UFL", draft, new Coord(-1, 1, 1));
            AddCornerTurnFamily(moves, "ULB", draft, new Coord(-1, 1, -1));
            AddCornerTurnFamily(moves, "UBR", draft, new Coord(1, 1, -1));

            return new PuzzleDefinition(
                "skewb",
                "Skewb",
                draft.Positions,
                draft.Stickers,
                moves);
        }

        private static void AddCornerTurnFamily(List<PuzzleMove> moves, string notation, StickerDraft draft, Coord axis)
        {
            var clockwise = CreateCornerTurn(notation, notation, draft, axis);
            moves.Add(clockwise);
            moves.Add(clockwise.Inverse(notation + "-prime", notation + "'"));
        }

        private static PuzzleMove CreateCornerTurn(string id, string notation, StickerDraft draft, Coord axis)
        {
            var destinationBySource = CreateIdentity(draft.Geometry.Count);

            for (var source = 0; source < draft.Geometry.Count; source++)
            {
                var sourceGeometry = draft.Geometry[source];

                if (!IsInMovingHalf(sourceGeometry, axis))
                {
                    continue;
                }

                var destinationGeometry = sourceGeometry.Kind == StickerKind.Center
                    ? new StickerGeometry(StickerKind.Center, sourceGeometry.Piece, RotateAroundCornerAxis(sourceGeometry.Normal, axis))
                    : new StickerGeometry(
                        StickerKind.Corner,
                        RotateAroundCornerAxis(sourceGeometry.Piece, axis),
                        RotateAroundCornerAxis(sourceGeometry.Normal, axis));

                destinationBySource[source] = draft.PositionByGeometry[destinationGeometry];
            }

            return new PuzzleMove(id, notation, destinationBySource);
        }

        private static bool IsInMovingHalf(StickerGeometry geometry, Coord axis)
        {
            if (geometry.Kind == StickerKind.Center)
            {
                return Dot(geometry.Normal, axis) > 0;
            }

            return Dot(geometry.Piece, axis) >= 1;
        }

        private static Coord RotateAroundCornerAxis(Coord coord, Coord axis)
        {
            var localX = coord.X * axis.X;
            var localY = coord.Y * axis.Y;
            var localZ = coord.Z * axis.Z;

            // 120-degree turn around the body diagonal. In local space the axis is (1, 1, 1).
            return new Coord(localZ * axis.X, localX * axis.Y, localY * axis.Z);
        }

        private static StickerDraft CreateStickerDraft()
        {
            var positions = new List<StickerPosition>();
            var stickers = new List<StickerDefinition>();
            var geometry = new List<StickerGeometry>();
            var positionByGeometry = new Dictionary<StickerGeometry, int>();

            foreach (var corner in CornerPositions)
            {
                foreach (var normal in CornerStickerNormals(corner))
                {
                    var face = FaceByNormal(normal);
                    var id = positions.Count;
                    var pieceId = PieceId(corner);
                    var slotId = $"{face.Id}_{pieceId}";
                    var stickerGeometry = new StickerGeometry(StickerKind.Corner, corner, normal);

                    positions.Add(new StickerPosition(id, pieceId, face.Id, slotId));
                    stickers.Add(new StickerDefinition(id, pieceId, face.Id, face.ColorId));
                    geometry.Add(stickerGeometry);
                    positionByGeometry.Add(stickerGeometry, id);
                }
            }

            foreach (var face in Faces)
            {
                var id = positions.Count;
                var pieceId = $"{face.Id}_center";
                var stickerGeometry = new StickerGeometry(StickerKind.Center, face.Normal, face.Normal);

                positions.Add(new StickerPosition(id, pieceId, face.Id, $"{face.Id}_center"));
                stickers.Add(new StickerDefinition(id, pieceId, face.Id, face.ColorId));
                geometry.Add(stickerGeometry);
                positionByGeometry.Add(stickerGeometry, id);
            }

            return new StickerDraft(positions, stickers, geometry, positionByGeometry);
        }

        private static IEnumerable<Coord> CornerStickerNormals(Coord corner)
        {
            yield return new Coord(corner.X, 0, 0);
            yield return new Coord(0, corner.Y, 0);
            yield return new Coord(0, 0, corner.Z);
        }

        private static FaceSpec FaceByNormal(Coord normal)
        {
            return Faces.Single(face => face.Normal.Equals(normal));
        }

        private static int Dot(Coord a, Coord b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        private static string PieceId(Coord corner)
        {
            return $"x{corner.X}_y{corner.Y}_z{corner.Z}";
        }

        private static int[] CreateIdentity(int count)
        {
            var identity = new int[count];

            for (var i = 0; i < count; i++)
            {
                identity[i] = i;
            }

            return identity;
        }

        private enum StickerKind
        {
            Corner,
            Center
        }

        private sealed class FaceSpec
        {
            public FaceSpec(string id, string colorId, Coord normal)
            {
                Id = id;
                ColorId = colorId;
                Normal = normal;
            }

            public string Id { get; }

            public string ColorId { get; }

            public Coord Normal { get; }
        }

        private sealed class StickerDraft
        {
            public StickerDraft(
                IReadOnlyList<StickerPosition> positions,
                IReadOnlyList<StickerDefinition> stickers,
                IReadOnlyList<StickerGeometry> geometry,
                IReadOnlyDictionary<StickerGeometry, int> positionByGeometry)
            {
                Positions = positions;
                Stickers = stickers;
                Geometry = geometry;
                PositionByGeometry = positionByGeometry;
            }

            public IReadOnlyList<StickerPosition> Positions { get; }

            public IReadOnlyList<StickerDefinition> Stickers { get; }

            public IReadOnlyList<StickerGeometry> Geometry { get; }

            public IReadOnlyDictionary<StickerGeometry, int> PositionByGeometry { get; }
        }

        private readonly struct StickerGeometry : IEquatable<StickerGeometry>
        {
            public StickerGeometry(StickerKind kind, Coord piece, Coord normal)
            {
                Kind = kind;
                Piece = piece;
                Normal = normal;
            }

            public StickerKind Kind { get; }

            public Coord Piece { get; }

            public Coord Normal { get; }

            public bool Equals(StickerGeometry other)
            {
                return Kind == other.Kind && Piece.Equals(other.Piece) && Normal.Equals(other.Normal);
            }

            public override bool Equals(object? obj)
            {
                return obj is StickerGeometry other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine((int)Kind, Piece, Normal);
            }
        }

        private readonly struct Coord : IEquatable<Coord>
        {
            public Coord(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public int X { get; }

            public int Y { get; }

            public int Z { get; }

            public bool Equals(Coord other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }

            public override bool Equals(object? obj)
            {
                return obj is Coord other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }
        }
    }
}
