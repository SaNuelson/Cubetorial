using System;
using System.Collections.Generic;

namespace Cubetorial.Model.Rubik3
{
    public static class Definition
    {
        /// <summary>
        /// Facelets on the cube that determine 0 orientation. Clockwise rotation equals 1 orientation delta.
        /// </summary>
        /// <remarks>
        /// I.e., R(URF) = { c: DFR, o: 2 }
        /// - R rotation replaces cubie on URF with cubie from DFR
        /// - DFR reference facelet D rotates to face F. Reference facelet of URF is U. F is -90 deg from U, so orientation delta is -1 = 2 mod 3
        /// </remarks>
        public static readonly IReadOnlyDictionary<Rubik3Slot, Rubik3Face> ReferenceFacelets = new Dictionary<Rubik3Slot, Rubik3Face>()
        {
            [Rubik3Slot.URF] = Rubik3Face.U,
            [Rubik3Slot.UFL] = Rubik3Face.U,
            [Rubik3Slot.ULB] = Rubik3Face.U,
            [Rubik3Slot.UBR] = Rubik3Face.U,
            [Rubik3Slot.UF] = Rubik3Face.U,
            [Rubik3Slot.UR] = Rubik3Face.U,
            [Rubik3Slot.UB] = Rubik3Face.U,
            [Rubik3Slot.UL] = Rubik3Face.U,
            
            [Rubik3Slot.DFR] = Rubik3Face.D,
            [Rubik3Slot.DRB] = Rubik3Face.D,
            [Rubik3Slot.DBL] = Rubik3Face.D,
            [Rubik3Slot.DLF] = Rubik3Face.D,
            [Rubik3Slot.DF] = Rubik3Face.D,
            [Rubik3Slot.DR] = Rubik3Face.D,
            [Rubik3Slot.DB] = Rubik3Face.D,
            [Rubik3Slot.DL] = Rubik3Face.D,
            
            [Rubik3Slot.FR] = Rubik3Face.F,
            [Rubik3Slot.FL] = Rubik3Face.F,
            
            [Rubik3Slot.BR] = Rubik3Face.B,
            [Rubik3Slot.BL] = Rubik3Face.B,
            
            [Rubik3Slot.F] = Rubik3Face.F,
            [Rubik3Slot.B] = Rubik3Face.B,
            [Rubik3Slot.R] = Rubik3Face.R,
            [Rubik3Slot.L] = Rubik3Face.L,
            [Rubik3Slot.U] = Rubik3Face.U,
            [Rubik3Slot.D] = Rubik3Face.D,
        };

        private static readonly Dictionary<Rubik3Slot, Rubik3Face[]> FaceletOrder = new()
        {
            [Rubik3Slot.URF] = new[] { Rubik3Face.U, Rubik3Face.R, Rubik3Face.F },
            [Rubik3Slot.UBR] = new[] { Rubik3Face.U, Rubik3Face.B, Rubik3Face.R },
            [Rubik3Slot.ULB] = new[] { Rubik3Face.U, Rubik3Face.L, Rubik3Face.B },
            [Rubik3Slot.UFL] = new[] { Rubik3Face.U, Rubik3Face.F, Rubik3Face.L },
            [Rubik3Slot.DFR] = new[] { Rubik3Face.D, Rubik3Face.F, Rubik3Face.R },
            [Rubik3Slot.DRB] = new[] { Rubik3Face.D, Rubik3Face.R, Rubik3Face.B },
            [Rubik3Slot.DBL] = new[] { Rubik3Face.D, Rubik3Face.B, Rubik3Face.L },
            [Rubik3Slot.DLF] = new[] { Rubik3Face.D, Rubik3Face.L, Rubik3Face.F },
        };

        public static int GetCornerOrientationDelta(Rubik3Slot slot, Rubik3Face face)
        {
            var slotReferenceFacelet = ReferenceFacelets[slot];
            var faceletOrder = FaceletOrder[slot];
            
            return (
                       Array.IndexOf(faceletOrder, face)
                       - Array.IndexOf(faceletOrder, slotReferenceFacelet)
                       + faceletOrder.Length) 
                   % faceletOrder.Length;
        }

        public static int GetEdgeOrientationDelta(Rubik3Slot slot, Rubik3Face face)
        {
            var slotReferenceFacelet = ReferenceFacelets[slot];

            return slotReferenceFacelet == face ? 0 : 1;
        }
    }
}
