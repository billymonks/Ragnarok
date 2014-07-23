using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.display.animation
{
    public class Tween
    {
        private TimeSpan elapsed, duration;
        private float start, end;

        public bool finished = false, readyForRemoval = false;

        public Tween(float start, float end, TimeSpan duration)
        {
            this.elapsed = new TimeSpan();
            this.duration = duration;
            this.start = start;
            this.end = end;
        }

        public void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime;
            
            if (elapsed >= duration)
                finished = true;
        }

        public float getValue()
        {
            return start + ((end - start) * (float)(elapsed.TotalMilliseconds / duration.TotalMilliseconds));
        }

        public void Remove()
        {
            readyForRemoval = true;
        }
    }
}
