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
using Com.Brashmonkey.Spriter.file;

namespace Com.Brashmonkey.Spriter.objects
{
	/// <summary>A SpriterModObject is an object which is able to manipulate animated bones and objects at runtime.
	/// 	</summary>
	/// <remarks>A SpriterModObject is an object which is able to manipulate animated bones and objects at runtime.
	/// 	</remarks>
	/// <author>Trixt0r</author>
	public class SpriterModObject : SpriterAbstractObject
	{
		private float alpha;

		private Reference @ref;

		private FileLoader loader;

		public SpriterModObject() : base()
		{
			this.alpha = 1f;
			this.@ref = null;
			this.loader = null;
			this.active = true;
		}

		public virtual float getAlpha()
		{
			return alpha;
		}

		public virtual void setAlpha(float alpha)
		{
			this.alpha = alpha;
		}

		public virtual Reference getRef()
		{
			return @ref;
		}

		public virtual void setRef(Reference @ref)
		{
			this.@ref = @ref;
		}

		public virtual FileLoader getLoader()
		{
			return loader;
		}

		public virtual void setLoader(FileLoader loader)
		{
			this.loader = loader;
		}

		public virtual bool isActive()
		{
			return active;
		}

		public virtual void setActive(bool active)
		{
			this.active = active;
		}

		private void modObject(SpriterAbstractObject @object
			)
		{
			@object.setAngle(@object.getAngle() + this.angle);
			@object.setScaleX(@object.getScaleX() * this.scaleX);
			@object.setScaleY(@object.getScaleY() * this.scaleY);
			@object.setX(@object.getX() + this.x);
			@object.setY(@object.getY() + this.y);
		}

		/// <summary>Manipulates the given object.</summary>
		/// <remarks>Manipulates the given object.</remarks>
		/// <param name="object"></param>
		public virtual void modSpriterObject(SpriterObject
			 @object)
		{
			this.modObject(@object);
			@object.setAlpha(@object.getAlpha() * this.alpha);
		}

		/// <summary>Manipulates the given bone.</summary>
		/// <remarks>Manipulates the given bone.</remarks>
		/// <param name="bone"></param>
		public virtual void modSpriterBone(SpriterBone bone
			)
		{
			this.modObject(bone);
		}
	}
}
