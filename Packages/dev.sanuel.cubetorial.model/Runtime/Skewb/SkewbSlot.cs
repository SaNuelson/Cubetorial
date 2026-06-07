using System;

namespace Cubetorial.Model.Skewb
{
    [Flags]
    public enum SkewbSlot : byte
    {
        URF = SkewbFaceMask.U | SkewbFaceMask.R | SkewbFaceMask.F,
        UBR = SkewbFaceMask.U | SkewbFaceMask.B | SkewbFaceMask.R,
        ULB = SkewbFaceMask.U | SkewbFaceMask.L | SkewbFaceMask.B,
        UFL = SkewbFaceMask.U | SkewbFaceMask.F | SkewbFaceMask.L,
        
        DFR = SkewbFaceMask.D | SkewbFaceMask.F | SkewbFaceMask.R,
        DRB = SkewbFaceMask.D | SkewbFaceMask.R | SkewbFaceMask.B,
        DBL = SkewbFaceMask.D | SkewbFaceMask.B | SkewbFaceMask.L,
        DLF = SkewbFaceMask.D | SkewbFaceMask.L | SkewbFaceMask.F,
        
        F = SkewbFaceMask.F,
        B = SkewbFaceMask.B,
        R = SkewbFaceMask.R,
        L = SkewbFaceMask.L,
        U = SkewbFaceMask.U,
        D = SkewbFaceMask.D,
    }
}