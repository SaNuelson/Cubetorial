using System;

namespace Cubetorial.Model.Rubik3
{
    [Flags]
    public enum Rubik3Slot : byte
    {
        URF = Rubik3FaceMask.U | Rubik3FaceMask.R | Rubik3FaceMask.F,
        UBR = Rubik3FaceMask.U | Rubik3FaceMask.B | Rubik3FaceMask.R,
        ULB = Rubik3FaceMask.U | Rubik3FaceMask.L | Rubik3FaceMask.B,
        UFL = Rubik3FaceMask.U | Rubik3FaceMask.F | Rubik3FaceMask.L,
        
        DFR = Rubik3FaceMask.D | Rubik3FaceMask.F | Rubik3FaceMask.R,
        DRB = Rubik3FaceMask.D | Rubik3FaceMask.R | Rubik3FaceMask.B,
        DBL = Rubik3FaceMask.D | Rubik3FaceMask.B | Rubik3FaceMask.L,
        DLF = Rubik3FaceMask.D | Rubik3FaceMask.L | Rubik3FaceMask.F,
        
        UF = Rubik3FaceMask.U | Rubik3FaceMask.F,
        UR = Rubik3FaceMask.U | Rubik3FaceMask.R,
        UB = Rubik3FaceMask.U | Rubik3FaceMask.B,
        UL = Rubik3FaceMask.U | Rubik3FaceMask.L,
        
        DF = Rubik3FaceMask.D | Rubik3FaceMask.F,
        DR = Rubik3FaceMask.D | Rubik3FaceMask.R,
        DB = Rubik3FaceMask.D | Rubik3FaceMask.B,
        DL = Rubik3FaceMask.D | Rubik3FaceMask.L,
        
        FR = Rubik3FaceMask.F | Rubik3FaceMask.R,
        FL = Rubik3FaceMask.F | Rubik3FaceMask.L,
        
        BR = Rubik3FaceMask.B | Rubik3FaceMask.R,
        BL = Rubik3FaceMask.B | Rubik3FaceMask.L,
        
        F = Rubik3FaceMask.F,
        B = Rubik3FaceMask.B,
        R = Rubik3FaceMask.R,
        L = Rubik3FaceMask.L,
        U = Rubik3FaceMask.U,
        D = Rubik3FaceMask.D,
    }
}