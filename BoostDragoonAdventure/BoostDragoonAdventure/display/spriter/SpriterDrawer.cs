using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Brashmonkey.Spriter.draw;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Com.Brashmonkey.Spriter.file;

namespace wickedcrush.display.spriter
{
    public class SpriterDrawer : AbstractDrawer
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch batch;
        private Texture2D blank;
        private Color color;

        public SpriterDrawer(SpriterLoader loader, GraphicsDeviceManager graphics)
            : base(loader)
        {
            this.graphics = graphics;
            this.blank = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            this.blank.SetData(new[] { Color.White });
            this.color = new Color(1, 1, 1, 1);
        }

        public SpriterDrawer(GraphicsDeviceManager graphics)
            : this(null, graphics)
        {
        }

        public void dispose()
        {
            this.blank.Dispose();
        }

        public override void draw(DrawInstruction instruction)
        {
            draw(instruction.getRef(), instruction.getX(), instruction.getY(), instruction.getPivotX(),
                instruction.getPivotY(), instruction.getScaleX(), instruction.getScaleY(), instruction.getAngle(),
                instruction.getAlpha());
        }

        private void draw(Reference reference, float x, float y, float pivotX, float pivotY, float scaleX, float scaleY,
            float angle, float alpha)
        {
            if (reference == null) return;
            Texture2D sprite = (Texture2D)this.loader.get(reference);
            if (sprite == null) return;
            Vector2 position = new Vector2(x, -y);
            Vector2 origin = new Vector2(reference.dimensions.width * pivotX, reference.dimensions.height * (1 - pivotY));
            Vector2 scale = new Vector2(scaleX, scaleY);
            Color color = new Color(1, 1, 1, alpha);
            //this.batch.Draw(sprite, position, null, color, this.DegreeToRadian(-angle), origin, scale, SpriteEffects.None, 1);
            this.batch.Draw(sprite, position, null, color, this.DegreeToRadian(-angle), origin, scale, SpriteEffects.None, 0f);
        }

        protected override void drawLine(float x1, float y1, float x2, float y2)
        {
            this.DrawLine(new Vector2(x1, -y1), new Vector2(x2, -y2));
        }

        protected override void drawRectangle(float x, float y, float width, float height)
        {
            this.DrawRectangle(x, -y, x + width, -y - height);
        }

        protected override void setDrawColor(float r, float g, float b, float a)
        {
            this.color = new Color(r, g, b, a);
        }



        void DrawLine(SpriteBatch batch, Texture2D blank, float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                        angle, Vector2.Zero, new Vector2(length, width),
                        SpriteEffects.None, 0);
        }

        void DrawLine(Vector2 point1, Vector2 point2)
        {
            this.DrawLine(this.batch, this.blank, 1f, this.color, point1, point2);
        }

        void DrawLine(Vector2 point1, Vector2 point2, Color color)
        {
            this.DrawLine(this.batch, this.blank, 1f, color, point1, point2);
        }

        void DrawRectangle(float left, float top, float right, float bottom)
        {
            this.DrawLine(new Vector2(left, top), new Vector2(right, top));
            this.DrawLine(new Vector2(right, top), new Vector2(right, bottom));
            this.DrawLine(new Vector2(right, bottom), new Vector2(left, bottom));
            this.DrawLine(new Vector2(left, top), new Vector2(left, bottom));
        }

        void DrawRectangle(float left, float top, float right, float bottom, Color color)
        {
            this.DrawLine(new Vector2(left, top), new Vector2(right, top), color);
            this.DrawLine(new Vector2(right, top), new Vector2(right, bottom), color);
            this.DrawLine(new Vector2(right, bottom), new Vector2(left, bottom), color);
            this.DrawLine(new Vector2(left, top), new Vector2(left, bottom), color);
        }
    }
}
