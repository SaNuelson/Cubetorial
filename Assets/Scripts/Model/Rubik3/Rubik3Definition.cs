using System;
using System.Collections.Generic;
using Model.Base;

namespace Model.Rubik3
{
    public static class Rubik3Definition
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

        public static PuzzleDefinition Create()
        {
            var draft = CreateStickerDraft();
            var moves = new List<PuzzleMove>();

            AddQuarterTurnFamily(ref moves, "U", draft, new Coord(0, 1, 0));
            AddQuarterTurnFamily(ref moves, "D", draft, new Coord(0, -1, 0));
            AddQuarterTurnFamily(ref moves, "F", draft, new Coord(0, 0, 1));
            AddQuarterTurnFamily(ref moves, "B", draft, new Coord(0, 0, -1));
            AddQuarterTurnFamily(ref moves, "R", draft, new Coord(1, 0, 0));
            AddQuarterTurnFamily(ref moves, "L", draft, new Coord(-1, 0, 0));

            return new PuzzleDefinition(
                "rubik-3",
                "3x3 Rubik's Cube",
                draft.Positions,
                draft.Stickers,
                moves);
        }

        private static void AddQuarterTurnFamily(ref List<PuzzleMove> moves, string notation, StickerDraft draft, Coord axis)
        {
            var clockwise = CreateLayerTurn(notation, notation, draft, axis);
            moves.Add(clockwise);
            moves.Add(clockwise.Then(clockwise, notation + "2", notation + "2"));
            moves.Add(clockwise.Inverse(notation + "-prime", notation + "'"));
        }

        private static PuzzleMove CreateLayerTurn(string id, string notation, StickerDraft draft, Coord axis)
        {
            var destinationBySource = CreateIdentity(draft.Geometry.Count);

            for (var source = 0; source < draft.Geometry.Count; source++)
            {
                var sourceGeometry = draft.Geometry[source];

                if (!IsInLayer(sourceGeometry.Cubie, axis))
                {
                    continue;
                }

                var destinationGeometry = new StickerGeometry(
                    RotateQuarter(sourceGeometry.Cubie, axis),
                    RotateQuarter(sourceGeometry.Normal, axis));

                destinationBySource[source] = draft.PositionByGeometry[destinationGeometry];
            }

            return new PuzzleMove(id, notation, destinationBySource);
        }

        private static bool IsInLayer(Coord cubie, Coord axis)
        {
            return axis.X != 0 && cubie.X == axis.X
                   || axis.Y != 0 && cubie.Y == axis.Y
                   || axis.Z != 0 && cubie.Z == axis.Z;
        }

        private static Coord RotateQuarter(Coord coord, Coord axis)
        {
            var direction = axis.X + axis.Y + axis.Z;

            if (axis.X != 0)
            {
                return direction > 0
                    ? new Coord(coord.X, -coord.Z, coord.Y)
                    : new Coord(coord.X, coord.Z, -coord.Y);
            }

            if (axis.Y != 0)
            {
                return direction > 0
                    ? new Coord(coord.Z, coord.Y, -coord.X)
                    : new Coord(-coord.Z, coord.Y, coord.X);
            }

            return direction > 0
                ? new Coord(-coord.Y, coord.X, coord.Z)
                : new Coord(coord.Y, -coord.X, coord.Z);
        }

        private static StickerDraft CreateStickerDraft()
        {
            var positions = new List<StickerPosition>();
            var stickers = new List<StickerDefinition>();
            var geometry = new List<StickerGeometry>();
            var positionByGeometry = new Dictionary<StickerGeometry, int>();

            foreach (var face in Faces)
            {
                for (var row = 0; row < 3; row++)
                {
                    for (var column = 0; column < 3; column++)
                    {
                        var cubie = CubieForFaceCell(face.Normal, row, column);
                        var id = positions.Count;
                        var pieceId = PieceId(cubie);
                        var slotId = $"{face.Id}{row}{column}";
                        var stickerGeometry = new StickerGeometry(cubie, face.Normal);

                        positions.Add(new StickerPosition(id, pieceId, face.Id, slotId));
                        stickers.Add(new StickerDefinition(id, pieceId, face.Id, face.ColorId));
                        geometry.Add(stickerGeometry);
                        positionByGeometry.Add(stickerGeometry, id);
                    }
                }
            }

            return new StickerDraft(positions, stickers, geometry, positionByGeometry);
        }

        private static Coord CubieForFaceCell(Coord normal, int row, int column)
        {
            var a = column - 1;
            var b = 1 - row;

            if (normal.Y != 0)
            {
                return new Coord(a, normal.Y, normal.Y > 0 ? b : -b);
            }

            if (normal.Z != 0)
            {
                return new Coord(a, b, normal.Z);
            }

            return new Coord(normal.X, b, normal.X > 0 ? -a : a);
        }

        private static string PieceId(Coord cubie)
        {
            return $"x{cubie.X}_y{cubie.Y}_z{cubie.Z}";
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
            public StickerGeometry(Coord cubie, Coord normal)
            {
                Cubie = cubie;
                Normal = normal;
            }

            public Coord Cubie { get; }

            public Coord Normal { get; }

            public bool Equals(StickerGeometry other)
            {
                return Cubie.Equals(other.Cubie) && Normal.Equals(other.Normal);
            }

            public override bool Equals(object? obj)
            {
                return obj is StickerGeometry other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Cubie, Normal);
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
