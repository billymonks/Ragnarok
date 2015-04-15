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
using System.Collections.Generic;
namespace Com.Brashmonkey.Spriter.animation
{
	public class SpriterAnimation
	{
		public readonly List<SpriterKeyFrame> frames;

		public readonly string name;

		public readonly int id;

		public readonly long length;

		public SpriterAnimation(int id, string name, long length)
		{
			this.frames = new List<SpriterKeyFrame
				>();
			this.id = id;
			this.name = name;
			this.length = length;
		}

		/// <summary>Searches for a keyframe in this animation which has a smaller or equal starting time as the given time.
		/// 	</summary>
		/// <remarks>Searches for a keyframe in this animation which has a smaller or equal starting time as the given time.
		/// 	</remarks>
		/// <param name="time"></param>
		/// <returns>A keyframe object which has a smaller or equal starting time than the given time.
		/// 	</returns>
		public virtual SpriterKeyFrame getPreviousFrame
			(long time)
		{
			SpriterKeyFrame frame = null;
			foreach (SpriterKeyFrame key in this.frames)
			{
				if (key.getTime() <= time)
				{
					frame = key;
				}
				else
				{
					break;
				}
			}
			return frame;
		}

		/// <summary>Searches for a keyframe in this animation which has a smaller or equal starting time as the given time and contains the given bone.
		/// 	</summary>
		/// <remarks>Searches for a keyframe in this animation which has a smaller or equal starting time as the given time and contains the given bone.
		/// 	</remarks>
		/// <param name="bone"></param>
		/// <param name="time"></param>
		/// <returns>A keyframe object which has a smaller or equal starting time than the given time and contains the given bone.
		/// 	</returns>
		public virtual SpriterKeyFrame getPreviousFrameForBone
			(SpriterBone bone, long time)
		{
			SpriterKeyFrame frame = null;
			foreach (SpriterKeyFrame key in this.frames)
			{
				if (!key.containsBone(bone))
				{
					continue;
				}
				if (key.getTime() <= time)
				{
					frame = key;
				}
				else
				{
					break;
				}
			}
			return frame;
		}

		/// <summary>Searches for a keyframe in this animation which has a smaller or equal starting time as the given time and contains the given object.
		/// 	</summary>
		/// <remarks>Searches for a keyframe in this animation which has a smaller or equal starting time as the given time and contains the given object.
		/// 	</remarks>
		/// <param name="object"></param>
		/// <param name="time"></param>
		/// <returns>A keyframe object which has a smaller or equal starting time than the given time and contains the given object.
		/// 	</returns>
		public virtual SpriterKeyFrame getPreviousFrameForObject
			(SpriterObject @object, long time)
		{
			SpriterKeyFrame frame = null;
			foreach (SpriterKeyFrame key in this.frames)
			{
				if (!key.containsObject(@object))
				{
					continue;
				}
				if (key.getTime() <= time)
				{
					frame = key;
				}
				else
				{
					break;
				}
			}
			return frame;
		}

		/// <returns>number of frames in this animation.</returns>
		public virtual int keyframes()
		{
			return this.frames.Count;
		}

		public virtual SpriterKeyFrame getNextFrameFor(
			SpriterAbstractObject @object, SpriterKeyFrame
			 currentFrame, int direction)
		{
			SpriterKeyFrame nextFrame = null;
			int cnt = 0;
			bool isBone = @object is SpriterBone;
			for (int j = (currentFrame.getId() + direction + this.keyframes()) % this.keyframes(); 
				nextFrame == null && cnt < this.keyframes(); j = (j + direction + this.keyframes()) % 
				this.keyframes(), cnt++)
			{
				SpriterKeyFrame frame = this.frames[j];
				bool contains = (isBone) ? frame.containsBone((SpriterBone
					)@object) : frame.containsObject((SpriterObject)
					@object);
				if (contains)
                {
                    SpriterAbstractObject objectInFrame;
                    if (isBone) objectInFrame = frame.getBoneFor((SpriterBone)@object);
                    else objectInFrame = frame.getObjectFor((SpriterObject)@object);
					if (@object.equals(objectInFrame))
					{
						nextFrame = frame;
					}
				}
			}
			return nextFrame;
		}
	}
}
