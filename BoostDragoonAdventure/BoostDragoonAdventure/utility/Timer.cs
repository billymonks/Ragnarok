using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.utility
{
    public class Timer
    {
        private TimeSpan interval;
        private TimeSpan currentTime;
        private bool active = false;
        private bool done = false;

        public Timer()
        {
            interval = TimeSpan.FromMilliseconds(0);
            reset();
        }

        public Timer(double milliseconds)
        {
            interval = TimeSpan.FromMilliseconds(milliseconds);
            reset();
        }

        public void Update(GameTime gameTime)
        {
            if(active)
                currentTime += gameTime.ElapsedGameTime;

            checkIfDone();
        }

        public void start()
        {
            active = true;
        }

        public void pause()
        {
            active = false;
        }

        public void reset()
        {
            currentTime = TimeSpan.FromMilliseconds(0);
            active = false;
            done = false;
        }

        public void resetAndStart()
        {
            reset();
            start();
        }

        public void activate()
        {
            active = true;
        }

        public bool isActive()
        {
            return active;
        }

        public void deactivate()
        {
            active = false;
        }

        public bool isDone()
        {
            return done;
        }

        public void setInterval(double milliseconds)
        {
            interval = TimeSpan.FromMilliseconds(milliseconds);
        }

        private void checkIfDone()
        {
            if (currentTime > interval)
            {
                done = true;
            }
        }
    }
}
