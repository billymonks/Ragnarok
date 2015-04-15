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

namespace Com.Brashmonkey.Spriter
{
	public class SpriterRectangle
	{
		public float left;

		public float top;

		public float right;

		public float bottom;

		public float width;

		public float height;

		public SpriterRectangle(float left, float top, float right, float bottom)
		{
			this.set(left, top, right, bottom);
			this.calculateSize();
		}

		public SpriterRectangle(Com.Brashmonkey.Spriter.SpriterRectangle rect)
		{
			this.set(rect);
		}

		public virtual bool isInisde(float x, float y)
		{
			return x >= this.left && x <= this.right && y <= this.top && y >= this.bottom;
		}

		public virtual void calculateSize()
		{
			this.width = right - left;
			this.height = top - bottom;
		}

		public virtual void set(Com.Brashmonkey.Spriter.SpriterRectangle rect)
		{
			if (rect == null)
			{
				return;
			}
			this.bottom = rect.bottom;
			this.left = rect.left;
			this.right = rect.right;
			this.top = rect.top;
			this.calculateSize();
		}

		public virtual void set(float left, float top, float right, float bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public static bool areIntersecting(Com.Brashmonkey.Spriter.SpriterRectangle rect1
			, Com.Brashmonkey.Spriter.SpriterRectangle rect2)
		{
			return rect1.isInisde(rect2.left, rect2.top) || rect1.isInisde(rect2.right, rect2
				.top) || rect1.isInisde(rect2.left, rect2.bottom) || rect1.isInisde(rect2.right, 
				rect2.bottom);
		}
	}
}
