namespace Cubetorial.Model.Rubik3
{
    public enum Rubik3Face : byte
    {
        U = 1,
        D = 2,
        F = 3,
        B = 4,
        R = 5,
        L = 6,
    }
    
    public enum Rubik3FaceMask : byte
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
        public static Rubik3FaceMask ToMask(this Rubik3Face face)
        {
            return face switch
            {
                Rubik3Face.U => Rubik3FaceMask.U,
                Rubik3Face.D => Rubik3FaceMask.D,
                Rubik3Face.F => Rubik3FaceMask.F,
                Rubik3Face.B => Rubik3FaceMask.B,
                Rubik3Face.R => Rubik3FaceMask.R,
                Rubik3Face.L => Rubik3FaceMask.L,
                _ => throw new System.ArgumentOutOfRangeException(nameof(face), face, null)
            };
        }
    }
}
