using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.editor;
using wickedcrush.entity;
using wickedcrush.map.path;

namespace wickedcrush.helper
{
    public static class Helper
    {
        static int uid = 0;

        public static bool angleInFov(int facing, int angle, int fovSize)
        {
            facing = (facing + 360) % 360;
            angle = (angle + 360) % 360;

            int halfFov = fovSize / 2;

            if (angle - halfFov < facing && angle + halfFov > facing)
                return true;

            if (facing - halfFov < 0)
            {
                if (angle < facing + halfFov)
                    return true;
                else if (facing - halfFov + 360 < angle)
                    return true;
            }

            if (facing + halfFov > 360)
            {
                if (angle > facing - halfFov)
                    return true;
                else if ((facing + halfFov) % 360 > angle)
                    return true;
            }

            return false;
        }

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

        public static int getUID()
        {
            return uid++;
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

        public static int degreeConversion(float radians) // makes radian angle positive
        {
            return (((int)MathHelper.ToDegrees(radians) + 360) % 360);
        }

        public static Direction radiansToDirection(float radians) // makes radian angle positive and converts to decimals in 45 degree intervals, for 8 directional things
        {
            return((Direction)(((((int)MathHelper.ToDegrees(radians) + 360f) % 360f) / 45f) * 45f));
        }

        public static Direction constrainDirection(Direction direction)
        {
            return (Direction)(((((int)direction + 360) % 360) / 45) * 45);
        }

        public static Point convertPositionToCoordinate(Vector2 pos, EditorRoom map, LayerType layerType)
        {
            int gridSize = map.width / map.layerList[layerType].GetLength(0);

            return new Point((int)pos.X / gridSize, (int)pos.Y / gridSize);
        }

        public static float getDistance(Vector2 posA, Vector2 posB)
        {
            return (float)Math.Sqrt(Math.Pow(posA.X - posB.X, 2) + Math.Pow(posA.Y - posB.Y, 2));
        }

        public static bool CharToBool(Char c)
        {
            return (c == '1');
        }

        public static Vector2 GetDirectionVectorFromDegrees(float Degrees)
        {
            //Vector2 North = new Vector2(0, -1);
            float Angle = MathHelper.ToRadians(Degrees);
            return new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
            //float Radians = MathHelper.ToRadians(Degrees);

            //var RotationMatrix = Matrix.CreateRotationZ(Radians);
            //return Vector2.Transform(North, RotationMatrix);
        }

        public static float GetRadiansFromVector(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float GetDegreesFromVector(Vector2 vector)
        {
            return MathHelper.ToDegrees((float)Math.Atan2(vector.Y, vector.X));
        }
    }
}
