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

namespace Com.Brashmonkey.Spriter.animation
{
	/// <summary>A SpriterKeyFrame holds an array of #SpriterBone and an array of #SpriterObject and their transformations.
	/// 	</summary>
	/// <remarks>
	/// A SpriterKeyFrame holds an array of #SpriterBone and an array of #SpriterObject and their transformations.
	/// It also holds an start and end time, which are necessary to interpolate the data with the data of another #SpriterKeyFrame.
	/// </remarks>
	/// <author>Trixt0r</author>
	public class SpriterKeyFrame
	{
		private SpriterBone[] bones;

		private SpriterObject[] objects;

		private long time;

		private int id;

		/// <returns>array of bones, this keyframe holds.</returns>
		public virtual SpriterBone[] getBones()
		{
			return bones;
		}

		/// <param name="bones">to set to this keyframe.</param>
		public virtual void setBones(SpriterBone[] bones)
		{
			this.bones = bones;
		}

		/// <returns>array of objects, this keyframe holds.</returns>
		public virtual SpriterObject[] getObjects()
		{
			return objects;
		}

		/// <param name="objects">to set to this keyframe.</param>
		public virtual void setObjects(SpriterObject[] objects
			)
		{
			this.objects = objects;
		}

		/// <returns>start time of this frame.</returns>
		public virtual long getTime()
		{
			return time;
		}

		/// <param name="startTime">of this frame.</param>
		public virtual void setTime(long startTime)
		{
			this.time = startTime;
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

		/// <summary>Returns whether this frame has information about the given object.</summary>
		/// <remarks>Returns whether this frame has information about the given object.</remarks>
		/// <param name="object"></param>
		/// <returns>True if this frame contains the object, false otherwise.</returns>
		public virtual bool containsObject(SpriterObject @object)
		{
			return this.getObjectFor(@object) != null;
		}

		/// <summary>Returns whether this frame has information about the given bone.</summary>
		/// <remarks>Returns whether this frame has information about the given bone.</remarks>
		/// <param name="bone"></param>
		/// <returns>True if this frame contains the bone, false otherwise.</returns>
		public virtual bool containsBone(SpriterBone bone
			)
		{
			return this.getBoneFor(bone) != null;
		}

		public virtual SpriterBone getBoneFor(SpriterBone
			 bone)
		{
			foreach (SpriterBone b in this.bones)
			{
				if (b.equals(bone))
				{
					return b;
				}
			}
			return null;
		}

		public virtual SpriterObject getObjectFor(SpriterObject
			 @object)
		{
			foreach (SpriterObject obj in this.objects)
			{
				if (obj.equals(@object))
				{
					return obj;
				}
			}
			return null;
		}
	}
}
