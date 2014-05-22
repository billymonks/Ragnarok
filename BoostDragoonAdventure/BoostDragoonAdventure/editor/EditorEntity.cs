using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;

namespace wickedcrush.editor
{
    public class EditorEntity
    {
        public String code, name;
        public int x, y;
        public bool canRotate;
        public Direction angle = Direction.East;

        public EditorEntity(String code, String name, int x, int y, Direction angle)
        {
            this.code = code;
            this.name = name;
            this.x = x;
            this.y = y;
            this.angle = angle;
        }

        public void rotateCW()
        {
            angle += 45;
            angle = (Direction)(((int)angle) % 360);
        }

        public void rotateCCW()
        {
            angle -= 45;
            angle += 360;
            angle = (Direction)(((int)angle) % 360);
        }
    }
}
