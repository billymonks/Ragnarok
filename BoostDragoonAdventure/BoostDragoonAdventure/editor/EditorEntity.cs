using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace wickedcrush.editor
{
    public class EditorEntity
    {
        public String code, name;
        public Vector2 pos, size, origin;
        public bool canRotate;
        public Direction angle = Direction.East;

        //editor sprite (to have different textures for different directions built in)

        public EditorEntity(String code, String name, Vector2 pos, Vector2 size, Vector2 origin, bool canRotate, Direction angle)
        {
            this.code = code;
            this.name = name;
            this.pos = pos;
            this.size = size;
            this.origin = origin;
            this.canRotate = canRotate;
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

        public void Draw(SpriteBatch sb)
        {
            //sb.Draw(texture, pos, 
        }

        public void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(wTex, pos-origin, null, c, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);

            DrawName(spriteBatch, f);
        }

        protected void DrawName(SpriteBatch spriteBatch, SpriteFont f)
        {
            spriteBatch.DrawString(f, name, pos - new Vector2(0, 11), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(0, 9), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(1, 10), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(-1, 10), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(0, 10), Color.White);
        }

        public bool Collision(EditorEntity e)
        {
            if (horizontalCollision(e) && verticalCollision(e))
                return true;

            return false;
        }

        private bool horizontalCollision(EditorEntity e)
        {
            if (this.pos.X <= e.pos.X
                && this.pos.X + this.size.X > e.pos.X)
                return true;

            if (e.pos.X <= this.pos.X
                && e.pos.X + e.size.X > this.pos.X)
                return true;

            return false;
        }

        private bool verticalCollision(EditorEntity e)
        {
            if (this.pos.Y <= e.pos.Y
                && this.pos.Y + this.size.Y > e.pos.Y)
                return true;

            if (e.pos.Y <= this.pos.Y
                && e.pos.Y + e.size.Y > this.pos.Y)
                return true;


            return false;
        }
    }
}
