using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
