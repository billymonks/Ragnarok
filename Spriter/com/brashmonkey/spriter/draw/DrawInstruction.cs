/**************************************************************************
 * Copyright 2013 by Trixt0r
 * (https://github.com/Trixt0r, Heinrich Reich, e-mail: trixter16@web.de)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
***************************************************************************/
using Com.Brashmonkey.Spriter.objects;
using Com.Brashmonkey.Spriter.file;

namespace Com.Brashmonkey.Spriter.draw
{
	/// <summary>A DrawIntruction is an object which holds all information you need to draw the previous transformed objects.
	/// 	</summary>
	/// <remarks>A DrawIntruction is an object which holds all information you need to draw the previous transformed objects.
	/// 	</remarks>
	/// <author>Trixt0r</author>
	public class DrawInstruction
	{
		public Reference @ref;

		public float x;

		public float y;

		public float pivotX;

		public float pivotY;

		public float angle;

		public float alpha;

		public float scaleX;

		public float scaleY;

		public SpriterObject obj = null;

		public FileLoader loader = null;

		public SpriterRectangle rect = null;

        public float depth;

		public DrawInstruction(Com.Brashmonkey.Spriter.file.Reference @ref, float x, float
			 y, float pivotX, float pivotY, float scaleX, float scaleY, float angle, float alpha,
            float depth
			)
		{
			this.@ref = @ref;
			//rect = new SpriterRectangle(ref.dimensions);
			this.x = x;
			this.y = y;
			this.pivotX = pivotX;
			this.pivotY = pivotY;
			this.angle = angle;
			this.alpha = alpha;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
            this.depth = depth;
		}

		/// <returns>the ref</returns>
		public virtual Reference getRef()
		{
			return @ref;
		}

		/// <returns>the x</returns>
		public virtual float getX()
		{
			return x;
		}

		/// <returns>the y</returns>
		public virtual float getY()
		{
			return y;
		}

		/// <returns>the pivotX</returns>
		public virtual float getPivotX()
		{
			return pivotX;
		}

		/// <returns>the pivotY</returns>
		public virtual float getPivotY()
		{
			return pivotY;
		}

		/// <returns>the angle</returns>
		public virtual float getAngle()
		{
			return angle;
		}

		/// <returns>the alpha</returns>
		public virtual float getAlpha()
		{
			return alpha;
		}

        /// <returns>the Z depth</returns>
        public virtual float getDepth()
        {
            return depth;
        }

		/// <returns>the scaleX</returns>
		public virtual float getScaleX()
		{
			return scaleX;
		}

		/// <returns>the scaleY</returns>
		public virtual float getScaleY()
		{
			return scaleY;
		}

		/// <returns>the obj</returns>
		public virtual SpriterObject getObj()
		{
			return obj;
		}

        public virtual void setDepth(float depth)
        {
            this.depth = depth;
        }
	}
}
