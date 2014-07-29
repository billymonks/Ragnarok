﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.editor;

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

        public static int degreeConversion(float radians) // makes radian angle positive and converts to decimals in 45 degree intervals, for 8 directional things
        {
            return(((((int)MathHelper.ToDegrees(radians) + 360) % 360) / 45) * 45);
        }

        public static Point convertPositionToCoordinate(Vector2 pos, EditorMap map, LayerType layerType)
        {
            int gridSize = map.width / map.layerList[layerType].GetLength(0);

            return new Point((int)pos.X / gridSize, (int)pos.Y / gridSize);
        }
    }
}
