using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.controls
{
    public abstract class Controls
    {
        public bool remove = false;

        public abstract void Update();
        
        public abstract float LStickXAxis();
        public abstract float LStickYAxis();
        public abstract float RStickXAxis();
        public abstract float RStickYAxis();

        public abstract bool DownPressed();
        public abstract bool UpPressed();
        public abstract bool LeftPressed();
        public abstract bool RightPressed();

        public abstract bool InteractHeld();
        public abstract bool InteractPressed();
        public abstract bool InteractReleased();

        public abstract bool ActionHeld();
        public abstract bool ActionPressed();
        public abstract bool ActionReleased();

        public abstract bool BlockHeld();
        public abstract bool BlockPressed();
        public abstract bool BlockReleased();

        public abstract bool BoostHeld();
        public abstract bool BoostPressed();
        public abstract bool BoostReleased();

        public abstract bool StrafeHeld();

        public abstract bool StartPressed();
        public abstract bool SelectPressed();
        
    }
}
