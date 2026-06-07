using System;
using System.Collections.Generic;
using Cubetorial.Model.Rubik3;

namespace Cubetorial.Model.Skewb
{
    public static class Definition
    {
        public static readonly IReadOnlyDictionary<SkewbSlot, SkewbFace> ReferenceFacelets = new Dictionary<SkewbSlot, SkewbFace>()
        {
            [SkewbSlot.URF] = SkewbFace.U,
            [SkewbSlot.UFL] = SkewbFace.U,
            [SkewbSlot.ULB] = SkewbFace.U,
            [SkewbSlot.UBR] = SkewbFace.U,
            
            [SkewbSlot.DFR] = SkewbFace.D,
            [SkewbSlot.DRB] = SkewbFace.D,
            [SkewbSlot.DBL] = SkewbFace.D,
            [SkewbSlot.DLF] = SkewbFace.D,
            
            [SkewbSlot.F] = SkewbFace.F,
            [SkewbSlot.B] = SkewbFace.B,
            [SkewbSlot.R] = SkewbFace.R,
            [SkewbSlot.L] = SkewbFace.L,
            [SkewbSlot.U] = SkewbFace.U,
            [SkewbSlot.D] = SkewbFace.D,
        };

        private static readonly Dictionary<SkewbSlot, SkewbFace[]> FaceletOrder = new()
        {
            [SkewbSlot.URF] = new[] { SkewbFace.U, SkewbFace.R, SkewbFace.F },
            [SkewbSlot.UBR] = new[] { SkewbFace.U, SkewbFace.B, SkewbFace.R },
            [SkewbSlot.ULB] = new[] { SkewbFace.U, SkewbFace.L, SkewbFace.B },
            [SkewbSlot.UFL] = new[] { SkewbFace.U, SkewbFace.F, SkewbFace.L },
            [SkewbSlot.DFR] = new[] { SkewbFace.D, SkewbFace.F, SkewbFace.R },
            [SkewbSlot.DRB] = new[] { SkewbFace.D, SkewbFace.R, SkewbFace.B },
            [SkewbSlot.DBL] = new[] { SkewbFace.D, SkewbFace.B, SkewbFace.L },
            [SkewbSlot.DLF] = new[] { SkewbFace.D, SkewbFace.L, SkewbFace.F },
        };

        public static int GetCornerOrientationDelta(SkewbSlot slot, SkewbFace face)
        {
            var slotReferenceFacelet = ReferenceFacelets[slot];
            var faceletOrder = FaceletOrder[slot];
            
            return (
                       Array.IndexOf(faceletOrder, face)
                       - Array.IndexOf(faceletOrder, slotReferenceFacelet)
                       + faceletOrder.Length) 
                   % faceletOrder.Length;
        }

        public static int GetEdgeOrientationDelta(SkewbSlot slot, SkewbFace face)
        {
            var slotReferenceFacelet = ReferenceFacelets[slot];

            return slotReferenceFacelet == face ? 0 : 1;
        }
    }
}
