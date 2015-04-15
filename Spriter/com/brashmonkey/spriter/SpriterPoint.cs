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

using System;
namespace Com.Brashmonkey.Spriter
{
	public class SpriterPoint
	{
		public float x;

		public float y;

		public SpriterPoint(float x, float y)
		{
			this.set(x, y);
		}

		public virtual void translate(float x, float y)
		{
			this.x += x;
			this.y += y;
		}

		public virtual void set(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public virtual void rotate(float degree)
		{
			double angle = SpriterCalculator.DegreeToRadian(degree);
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);
			float xx = x * cos - y * sin;
			float yy = x * sin + y * cos;
			this.x = xx;
			this.y = yy;
		}
	}
}
