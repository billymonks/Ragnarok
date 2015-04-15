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

namespace Com.Brashmonkey.Spriter.objects
{
	/// <summary>A SpriterObject is an object which holds the transformations for an object which was animated in the Spriter editor.
	/// 	</summary>
	/// <remarks>
	/// A SpriterObject is an object which holds the transformations for an object which was animated in the Spriter editor.
	/// It also holds information about things which will be drawn on the screen, such as sprite, depth and transparency.
	/// </remarks>
	/// <author>Trixt0r</author>
	public class SpriterObject : SpriterAbstractObject
	{
		internal float pivotX;

		internal float pivotY;

		internal float alpha;

		internal int zIndex;

		internal bool transientObject = false;

		internal bool visible = true;

		internal Com.Brashmonkey.Spriter.file.Reference @ref;

		internal Com.Brashmonkey.Spriter.file.FileLoader loader = null;

		internal Com.Brashmonkey.Spriter.SpriterRectangle rect = new Com.Brashmonkey.Spriter.SpriterRectangle
			(0, 0, 0, 0);

		private Com.Brashmonkey.Spriter.SpriterPoint[] boundingPoints;

		public SpriterObject()
		{
			boundingPoints = new Com.Brashmonkey.Spriter.SpriterPoint[4];
			for (int i = 0; i < this.boundingPoints.Length; i++)
			{
				this.boundingPoints[i] = new Com.Brashmonkey.Spriter.SpriterPoint(0, 0);
			}
		}

		public virtual void setRef(Com.Brashmonkey.Spriter.file.Reference @ref)
		{
			this.@ref = @ref;
			this.rect.set(@ref.dimensions);
		}

		public virtual Com.Brashmonkey.Spriter.file.Reference getRef()
		{
			return this.@ref;
		}

		public virtual float getPivotX()
		{
			return pivotX;
		}

		public virtual void setPivotX(float pivotX)
		{
			this.pivotX = pivotX;
		}

		public virtual float getPivotY()
		{
			return pivotY;
		}

		public virtual void setPivotY(float pivotY)
		{
			this.pivotY = pivotY;
		}

		public virtual int getZIndex()
		{
			return zIndex;
		}

		public virtual void setZIndex(int zIndex)
		{
			this.zIndex = zIndex;
		}

		public override void setAngle(float angle)
		{
			this.angle = angle;
		}

		public virtual float getAlpha()
		{
			return alpha;
		}

		public virtual void setAlpha(float alpha)
		{
			this.alpha = alpha;
		}

		public virtual bool isTransientObject()
		{
			return transientObject;
		}

		public virtual void setTransientObject(bool transientObject)
		{
			this.transientObject = transientObject;
		}

		/// <summary>Compares the z_index of the given SpriterObject with this.</summary>
		/// <remarks>Compares the z_index of the given SpriterObject with this.</remarks>
		/// <param name="o">SpriterObject to compare with.</param>
		public virtual int compareTo(SpriterObject o)
		{
			if (this.zIndex < o.zIndex)
			{
				return -1;
			}
			else
			{
				if (this.zIndex > o.zIndex)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}

		public virtual void setLoader(Com.Brashmonkey.Spriter.file.FileLoader loader)
		{
			this.loader = loader;
		}

		public virtual Com.Brashmonkey.Spriter.file.FileLoader getLoader()
		{
			return this.loader;
		}

		public virtual bool isVisible()
		{
			return visible;
		}

		public virtual void setVisible(bool visible)
		{
			this.visible = visible;
		}

		public override void copyValuesTo(SpriterAbstractObject
			 @object)
		{
			base.copyValuesTo(@object);
			if (!(@object is SpriterObject))
			{
				return;
			}
			((SpriterObject)@object).setAlpha(alpha);
			((SpriterObject)@object).setRef(@ref);
			((SpriterObject)@object).setPivotX(pivotX);
			((SpriterObject)@object).setPivotY(pivotY);
			((SpriterObject)@object).setTransientObject(transientObject
				);
			((SpriterObject)@object).setZIndex(zIndex);
			((SpriterObject)@object).setLoader(loader);
			((SpriterObject)@object).setVisible(visible);
			((SpriterObject)@object).rect.set(this.rect);
		}

		public virtual void copyValuesTo(Com.Brashmonkey.Spriter.draw.DrawInstruction instruction
			)
		{
			instruction.x = this.x;
			instruction.y = this.y;
			instruction.scaleX = this.scaleX;
			instruction.scaleY = this.scaleY;
			instruction.pivotX = this.pivotX;
			instruction.pivotY = this.pivotY;
			instruction.angle = this.angle;
			instruction.alpha = this.alpha;
			instruction.@ref = this.@ref;
			instruction.loader = this.loader;
			instruction.obj = this;
		}

		public virtual Com.Brashmonkey.Spriter.SpriterPoint[] getBoundingBox()
		{
			float width = this.@ref.dimensions.width * this.scaleX;
			float height = this.@ref.dimensions.height * this.scaleY;
			float pivotX = width * this.pivotX;
			float pivotY = height * this.pivotY;
			this.boundingPoints[0].set(-pivotX, -pivotY);
			this.boundingPoints[1].set(width - pivotX, -pivotY);
			this.boundingPoints[2].set(-pivotX, height - pivotY);
			this.boundingPoints[3].set(width - pivotX, height - pivotY);
			this.boundingPoints[0].rotate(angle);
			this.boundingPoints[1].rotate(angle);
			this.boundingPoints[2].rotate(angle);
			this.boundingPoints[3].rotate(angle);
			for (int i = 0; i < this.boundingPoints.Length; i++)
			{
				this.boundingPoints[i].translate(x, y);
			}
			return this.boundingPoints;
		}
	}
}
