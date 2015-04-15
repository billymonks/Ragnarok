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

using com.discobeard.spriter.dom;
using Com.Brashmonkey.Spriter.objects;

namespace Com.Brashmonkey.Spriter.mergers
{
	public class SpriterObjectMerger : Com.Brashmonkey.Spriter.mergers.ISpriterMerger
		<AnimationObjectRef, Key, 
		SpriterObject>
	{
		public virtual SpriterObject merge(AnimationObjectRef
			 @ref, Key key)
		{
			AnimationObject obj = key.getObject()[0];
			SpriterObject spriterObject = new SpriterObject
                ();
            spriterObject.setId(@ref.getId());
            spriterObject.setParentId(@ref.getParent());
            spriterObject.setTimeline(@ref.getTimeline());
            spriterObject.setAngle(obj.getAngle());
            spriterObject.setRef(new Com.Brashmonkey.Spriter.file.Reference(obj.getFolder(),
                obj.getFile()));
            spriterObject.setPivotX(obj.getPivotX());
            spriterObject.setPivotY(obj.getPivotY());
            spriterObject.setX(obj.getX());
            spriterObject.setY(obj.getY());
            spriterObject.setZIndex(@ref.getZIndex());
            spriterObject.setSpin(key.getSpin());
            spriterObject.setAlpha(obj.getA());
            spriterObject.setScaleX(obj.getScaleX());
            spriterObject.setScaleY(obj.getScaleY());
			return spriterObject;
		}
	}
}
