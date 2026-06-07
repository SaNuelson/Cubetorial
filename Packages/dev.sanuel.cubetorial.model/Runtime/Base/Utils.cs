using System;

namespace Cubetorial.Model.Base
{
    public static class Utils
    {
        private static int CountSetBits(ulong value)
        {
            var count = 0;

            while (value != 0)
            {
                value &= value - 1; // clears the lowest set bit
                count++;
            }

            return count;
        }
        
        public static int FaceCount<T>(this T face) where T : Enum
        {
            if (!System.Attribute.IsDefined(typeof(T), typeof(System.FlagsAttribute)))
                throw new System.ArgumentException($"{typeof(T).Name} must be a [Flags] enum.");

            var common = System.Convert.ToUInt64(face);
            return CountSetBits(common);
        }
    }
}