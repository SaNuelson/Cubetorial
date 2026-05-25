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
}