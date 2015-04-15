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
	/// <summary>A SpriterAbstractObject is, as the name says, an abstract object which holds the same properties a #SpriterObject and a #SpriterBone have.
	/// 	</summary>
	/// <remarks>
	/// A SpriterAbstractObject is, as the name says, an abstract object which holds the same properties a #SpriterObject and a #SpriterBone have.
	/// Such as x,y coordinates, angle, id, parent, scale and the timeline.
	/// </remarks>
	/// <author>Trixt0r</author>
	public abstract class SpriterAbstractObject
	{
		protected internal float x;

		protected internal float y;

		protected internal float angle;

		protected internal float scaleX;

		protected internal float scaleY;

		protected internal int id;

		protected internal int parentId;

		protected internal int timeline;

		protected internal int spin;

		protected internal SpriterAbstractObject parent;

		protected internal string name;

		public bool active = true;

		public SpriterAbstractObject()
		{
			this.x = 0;
			this.y = 0;
			this.angle = 0f;
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.id = -1;
			this.parentId = -1;
			this.name = string.Empty;
			this.parent = null;
		}

		/// <returns>the name</returns>
		public virtual string getName()
		{
			return name;
		}

		/// <param name="name">the name to set</param>
		public virtual void setName(string name)
		{
			this.name = name;
		}

		/// <returns>the x</returns>
		public virtual float getX()
		{
			return x;
		}

		/// <param name="x">the x to set</param>
		public virtual void setX(float x)
		{
			this.x = x;
		}

		/// <returns>the y</returns>
		public virtual float getY()
		{
			return y;
		}

		/// <param name="y">the y to set</param>
		public virtual void setY(float y)
		{
			this.y = y;
		}

		/// <returns>the angle</returns>
		public virtual float getAngle()
		{
			return angle;
		}

		/// <param name="angle">the angle to set</param>
		public virtual void setAngle(float angle)
		{
			this.angle = angle;
		}

		/// <returns>the scaleX</returns>
		public virtual float getScaleX()
		{
			return scaleX;
		}

		/// <param name="scaleX">the scaleX to set</param>
		public virtual void setScaleX(float scaleX)
		{
			this.scaleX = scaleX;
		}

		/// <returns>the scaleY</returns>
		public virtual float getScaleY()
		{
			return scaleY;
		}

		/// <param name="scaleY">the scaleY to set</param>
		public virtual void setScaleY(float scaleY)
		{
			this.scaleY = scaleY;
		}

		public virtual int getSpin()
		{
			return spin;
		}

		public virtual void setSpin(int spin)
		{
			this.spin = spin;
		}

		/// <returns>the id</returns>
		public virtual int getId()
		{
			return id;
		}

		/// <param name="id">the id to set</param>
		public virtual void setId(int id)
		{
			this.id = id;
		}

		/// <returns>the parent</returns>
		public virtual SpriterAbstractObject getParent()
		{
			return parent;
		}

		/// <param name="parent">the parent to set</param>
		public virtual void setParent(SpriterAbstractObject
			 parent)
		{
			this.parent = parent;
		}

		/// <returns>the parentId</returns>
		public virtual int getParentId()
		{
			return parentId;
		}

		/// <param name="parentId">the parentId to set</param>
		public virtual void setParentId(int parentId)
		{
			this.parentId = parentId;
		}

		/// <returns>the timeline</returns>
		public virtual int getTimeline()
		{
			return timeline;
		}

		/// <param name="timeline">the timeline to set</param>
		public virtual void setTimeline(int timeline)
		{
			this.timeline = timeline;
		}

		/// <summary>Sets the values of this instance to the given one.</summary>
		/// <remarks>Sets the values of this instance to the given one.</remarks>
		/// <param name="object">which has to be manipulated.</param>
		public virtual void copyValuesTo(SpriterAbstractObject
			 @object)
		{
			@object.setAngle(angle);
			@object.setScaleX(scaleX);
			@object.setScaleY(scaleY);
			@object.setX(x);
			@object.setY(y);
			@object.setId(id);
			@object.setParentId(parentId);
			@object.setParent(parent);
			@object.setTimeline(timeline);
			@object.setSpin(spin);
			@object.setName(name);
		}

		/// <param name="object">to compare with</param>
		/// <returns>true if both objects have the same id.</returns>
		public virtual bool equals(SpriterAbstractObject 
			@object)
		{
			if (@object == null)
			{
				return false;
			}
			return this.timeline == @object.getTimeline();
		}

		/// <returns>whether this has a parent or not.</returns>
		public virtual bool hasParent()
		{
			return this.parentId != -1;
		}

		public override string ToString()
		{
			return "id: " + this.id + ", name: " + this.name + ", parent: " + this.parentId +
				 ", x: " + this.x + ", y: " + this.y + ", angle:" + this.angle + " timeline: " +
				 this.timeline;
		}
	}
}
