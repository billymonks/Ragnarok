using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.display.primitives;

namespace wickedcrush.screen.transition
{
    public class SolidColorFadeTransition : Transition
    {
        public Color startColor, endColor, drawColor;

        public SolidColorFadeTransition(GameBase g, double transitionTime, bool exclusiveUpdate, Color startColor, Color endColor)
        {
            Initialize(g, transitionTime, exclusiveUpdate, startColor, endColor);
        }

        public void Initialize(GameBase g, double transitionTime, bool exclusiveUpdate, Color startColor, Color endColor)
        {
            base.Initialize(g, transitionTime);

            exclusiveDraw = false;
            this.exclusiveUpdate = exclusiveUpdate;

            this.startColor = startColor;
            this.endColor = endColor;

            this.drawColor = startColor;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateColor();

            float f = GetPercent();
        }
        public override void FullScreenDraw()
        {
            base.FullScreenDraw();
            PrimitiveDrawer.DrawFilledRectangle(game.spriteBatch, new Rectangle(0, 0, 1440, 1080), drawColor);
        }

        private void UpdateColor()
        {
            drawColor = new Color((byte)(startColor.R * (1 - GetPercent())
                + (endColor.R * GetPercent())),
                (byte)(startColor.G * (1 - GetPercent())
                + (endColor.G * GetPercent())),
                (byte)(startColor.B * (1 - GetPercent())
                + (endColor.B * GetPercent())),
                (byte)(startColor.A * (1 - GetPercent())
                + (endColor.A * GetPercent())));
        }
    }
}
