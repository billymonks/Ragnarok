using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.helper
{
    public static class Helper
    {
        public static int smallestNumber(int[] list)
        {
            int num = list[0];
            for (int i = 1; i < list.Length; i++)
            {
                if (list[i] < num)
                    num = list[i];
            }
            return num;
        }

        public static float roundTowardZero(float f)
        {
            if (f < 0)
            {
                return (float)Math.Ceiling(f);
            }
            else
            {
                return (float)Math.Floor(f);
            }
        }

        public static int roundUpDivision(int a, int b) // a/b
        {
            return (a / b + (a % b > 0 ? 1 : 0));
        }

        public static int degreeConversion(float a) // converts normal degrees to weird ones where south = 0 and 45 degree intervals
        {
            return(((((int)MathHelper.ToDegrees(a) + 270) % 360) / 45) * 45);
        }
    }
}
