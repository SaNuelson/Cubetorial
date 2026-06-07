using System;

namespace Cubetorial.Model.Skewb
{
    public enum SkewbFace : byte
    {
        U = 1,
        D = 2,
        F = 3,
        B = 4,
        R = 5,
        L = 6,
    }
    
    public enum SkewbFaceMask : byte
    {
        U = 1 << 0,
        D = 1 << 1,
        F = 1 << 2,
        B = 1 << 3,
        R = 1 << 4,
        L = 1 << 5,
    }

    internal static class Rubik3FaceExtensions
    {
        public static SkewbFaceMask ToMask(this SkewbFace face)
        {
            return face switch
            {
                SkewbFace.U => SkewbFaceMask.U,
                SkewbFace.D => SkewbFaceMask.D,
                SkewbFace.F => SkewbFaceMask.F,
                SkewbFace.B => SkewbFaceMask.B,
                SkewbFace.R => SkewbFaceMask.R,
                SkewbFace.L => SkewbFaceMask.L,
                _ => throw new System.ArgumentOutOfRangeException(nameof(face), face, null)
            };
        }

        internal static SkewbFaceMask Opposite(this SkewbFaceMask face)
            => face switch
            {
                SkewbFaceMask.U => SkewbFaceMask.D,
                SkewbFaceMask.D => SkewbFaceMask.U,
                SkewbFaceMask.F => SkewbFaceMask.B,
                SkewbFaceMask.B => SkewbFaceMask.F,
                SkewbFaceMask.R => SkewbFaceMask.L,
                SkewbFaceMask.L => SkewbFaceMask.R,
                _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
            };
    }
}
