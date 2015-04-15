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
using System.Collections.Generic;
using Com.Brashmonkey.Spriter;

namespace Com.Brashmonkey.Spriter.objects
{
	/// <summary>A SpriterBone is a bone like in the Spriter editor.</summary>
	/// <remarks>A SpriterBone is a bone like in the Spriter editor. It can hold children (#SpriterObject and #SpriterBone) which get manipulated relative to this object.
	/// 	</remarks>
	/// <author>Trixt0r</author>
	public class SpriterBone : SpriterAbstractObject
	{
        internal List<SpriterBone> childBones;

        internal List<SpriterObject> childObjects;

		public SpriterRectangle boundingBox;

		public SpriterBone()
		{
			this.childBones = new List<SpriterBone>();
            this.childObjects = new List<SpriterObject>();
			this.boundingBox = new Com.Brashmonkey.Spriter.SpriterRectangle(0, 0, 0, 0);
		}

		public virtual void addChildBone(SpriterBone bone
			)
		{
			bone.setParent(this);
			childBones.Add(bone);
		}

		public virtual IList<SpriterBone
			> getChildBones()
		{
			return childBones;
		}

		public virtual void addChildObject(SpriterObject 
			@object)
		{
			@object.setParent(this);
			childObjects.Add(@object);
		}

		public virtual List<SpriterObject> getChildObjects()
		{
			return childObjects;
		}

		public override void copyValuesTo(SpriterAbstractObject
			 bone)
		{
			base.copyValuesTo(bone);
			if (!(bone is SpriterBone))
			{
				return;
			}
			((SpriterBone)bone).childBones = this.childBones;
			((SpriterBone)bone).childObjects = this.childObjects;
		}

		public virtual void calcBoundingBox(Com.Brashmonkey.Spriter.SpriterRectangle @base
			)
		{
			this.boundingBox.set(@base);
			foreach (SpriterObject @object in this.childObjects)
			{
				Com.Brashmonkey.Spriter.SpriterPoint[] points = @object.getBoundingBox();
				this.boundingBox.left = Math.Min(Math.Min(Math.Min(Math
					.Min(points[0].x, points[1].x), points[2].x), points[3].x), this.boundingBox.left
					);
				this.boundingBox.right = Math.Max(Math.Max(Math.Max(Math
					.Max(points[0].x, points[1].x), points[2].x), points[3].x), this.boundingBox.right
					);
				this.boundingBox.top = Math.Max(Math.Max(Math.Max(Math
					.Max(points[0].y, points[1].y), points[2].y), points[3].y), this.boundingBox.top
					);
				this.boundingBox.bottom = Math.Min(Math.Min(Math.Min(Math
					.Min(points[0].y, points[1].y), points[2].y), points[3].y), this.boundingBox.bottom
					);
			}
			foreach (SpriterBone child in this.childBones)
			{
				child.calcBoundingBox(boundingBox);
				this.boundingBox.set(child.boundingBox);
			}
		}
	}
}
