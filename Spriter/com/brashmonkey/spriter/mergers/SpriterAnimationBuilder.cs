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
using System.Collections.Generic;
using Com.Brashmonkey.Spriter.objects;
using Com.Brashmonkey.Spriter.animation;
using com.discobeard.spriter.dom;

namespace Com.Brashmonkey.Spriter.mergers
{
    public class SpriterAnimationBuilder
    {
        private readonly SpriterBoneMerger boneMerger = new
            SpriterBoneMerger();

        private readonly SpriterObjectMerger objectMerger
             = new SpriterObjectMerger();

        internal Dictionary<SpriterBone, int> bonesToTween;

        internal Dictionary<SpriterObject, int> objectsToTween;

        //import Com.Brashmonkey.Spriter.converters.SpriterObjectConverter;
        //import AnimationObject;
        //final private SpriterObjectConverter objectConverter = new SpriterObjectConverter();
        public virtual SpriterAnimation buildAnimation(
            Animation animation)
        {
            MainLine mainline = animation.getMainline();
            IList<TimeLine> timeLines =
                animation.getTimeline();
            IList<Key> keyFrames = mainline
                .getKey();
            bonesToTween = new Dictionary<SpriterBone
                , int>();
            objectsToTween = new Dictionary<SpriterObject
                , int>();
            SpriterAnimation spriterAnimation = new SpriterAnimation(animation.getId(), animation.getName(), animation.getLength());
            for (int k = 0; k < keyFrames.Count; k++)
            {
                Key mainlineKey = keyFrames[k];
                IList<SpriterObject> tempObjects
                     = new List<SpriterObject
                    >();
                IList<SpriterBone> tempBones
                     = new List<SpriterBone
                    >();
                SpriterKeyFrame frame = new SpriterKeyFrame
                    ();
                frame.setTime(mainlineKey.getTime());
                frame.setId(mainlineKey.getId());
                foreach (BoneRef boneRef in mainlineKey.getBoneRef())
                {
                    TimeLine timeline = timeLines[boneRef.getTimeline()];
                    Key timelineKey = timeline.getKey()[boneRef.getKey()];
                    SpriterBone bone = boneMerger.merge(boneRef, timelineKey
                        );
                    bone.setName(timeline.getName());
                    if (mainlineKey.getTime() != timelineKey.getTime())
                    {
                        bonesToTween.Add(bone, k);
                    }
                    else
                    {
                        tempBones.Add(bone);
                    }
                }
                //}
                foreach (AnimationObjectRef objectRef in mainlineKey.getObjectRef
                    ())
                {
                    TimeLine timeline = timeLines[objectRef.getTimeline()];
                    Key timelineKey = timeline.getKey()[objectRef.getKey()
                        ];
                    SpriterObject @object = objectMerger.merge(objectRef
                        , timelineKey);
                    @object.setName(timeline.getName());
                    if (mainlineKey.getTime() != timelineKey.getTime())
                    {
                        objectsToTween.Add(@object, k);
                    }
                    else
                    {
                        tempObjects.Add(@object);
                    }
                }
                //}
                SpriterObject[] objArray = new SpriterObject[tempObjects.Count];
                tempObjects.CopyTo(objArray,0);
                frame.setObjects(objArray);
                SpriterBone[] boneArray = new SpriterBone[tempBones.Count];
                tempBones.CopyTo(boneArray, 0);
                frame.setBones(boneArray);
                spriterAnimation.frames.Add(frame);
            }
            this.tweenBones(spriterAnimation);
            this.tweenObjects(spriterAnimation);
            return spriterAnimation;
        }

        public virtual void tweenBones(SpriterAnimation  animation)
        {
            foreach (SpriterBone key in bonesToTween.Keys)
            {
                SpriterBone toTween = key;
                SpriterKeyFrame frame = animation.frames[bonesToTween[key]];
                long time = frame.getTime();
                SpriterKeyFrame currentFrame = animation.getPreviousFrameForBone
                    (toTween, time);
                SpriterKeyFrame nextFrame = animation.getNextFrameFor
                    (toTween, currentFrame, 1);
                if (nextFrame != currentFrame)
                {
                    SpriterBone bone1 = currentFrame.getBoneFor(toTween
                        );
                    SpriterBone bone2 = nextFrame.getBoneFor(toTween);
                    this.interpolateAbstractObject(toTween, bone1, bone2, currentFrame.getTime(), nextFrame
                        .getTime(), time);
                }
                SpriterBone[] bones = new SpriterBone
                    [frame.getBones().Length + 1];
                for (int i = 0; i < bones.Length - 1; i++)
                {
                    bones[i] = frame.getBones()[i];
                }
                bones[bones.Length - 1] = toTween;
                frame.setBones(bones);
            }
        }

        public virtual void tweenObjects(SpriterAnimation  animation)
        {
            foreach (SpriterObject key in objectsToTween.Keys)
            {
                SpriterObject toTween = key;
                SpriterKeyFrame frame = animation.frames[objectsToTween[key]];
                long time = frame.getTime();
                SpriterKeyFrame currentFrame = animation.getPreviousFrameForObject
                    (toTween, time);
                SpriterKeyFrame nextFrame = animation.getNextFrameFor
                    (toTween, currentFrame, 1);
                if (nextFrame != currentFrame)
                {
                    SpriterObject object1 = currentFrame.getObjectFor
                        (toTween);
                    SpriterObject object2 = nextFrame.getObjectFor(toTween
                        );
                    this.interpolateSpriterObject(toTween, object1, object2, currentFrame.getTime(),
                        nextFrame.getTime(), time);
                }
                SpriterObject[] objects = new SpriterObject
                    [frame.getObjects().Length + 1];
                for (int i = 0; i < objects.Length - 1; i++)
                {
                    objects[i] = frame.getObjects()[i];
                }
                objects[objects.Length - 1] = toTween;
                frame.setObjects(objects);
            }
        }

        private void interpolateAbstractObject(SpriterAbstractObject
             target, SpriterAbstractObject obj1, SpriterAbstractObject
             obj2, float startTime, float endTime, float frame)
        {
            if (obj2 == null)
            {
                return;
            }
            target.setX(this.interpolate(obj1.getX(), obj2.getX(), startTime, endTime, frame)
                );
            target.setY(this.interpolate(obj1.getY(), obj2.getY(), startTime, endTime, frame)
                );
            target.setScaleX(this.interpolate(obj1.getScaleX(), obj2.getScaleX(), startTime,
                endTime, frame));
            target.setScaleY(this.interpolate(obj1.getScaleY(), obj2.getScaleY(), startTime,
                endTime, frame));
            target.setAngle(this.interpolateAngle(obj1.getAngle(), obj2.getAngle(), startTime
                , endTime, frame));
        }

        private void interpolateSpriterObject(SpriterObject
             target, SpriterObject obj1, SpriterObject
             obj2, float startTime, float endTime, float frame)
        {
            if (obj2 == null)
            {
                return;
            }
            this.interpolateAbstractObject(target, obj1, obj2, startTime, endTime, frame);
            target.setPivotX(this.interpolate(obj1.getPivotX(), obj2.getPivotX(), startTime,
                endTime, frame));
            target.setPivotY(this.interpolate(obj1.getPivotY(), obj2.getPivotY(), startTime,
                endTime, frame));
            target.setAlpha(this.interpolateAngle(obj1.getAlpha(), obj2.getAlpha(), startTime
                , endTime, frame));
        }

        /// <summary>
        /// See
        /// <see cref="Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
        /// 	">Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
        /// 	</see>
        /// Can be inherited, to handle other interpolation techniques. Standard is linear interpolation.
        /// </summary>
        protected internal virtual float interpolate(float a, float b, float timeA, float
             timeB, float currentTime)
        {
            return Com.Brashmonkey.Spriter.interpolation.SpriterLinearInterpolator.interpolator
                .interpolate(a, b, timeA, timeB, currentTime);
        }

        /// <summary>
        /// See
        /// <see cref="Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
        /// 	">Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
        /// 	</see>
        /// Can be inherited, to handle other interpolation techniques. Standard is linear interpolation.
        /// </summary>
        protected internal virtual float interpolateAngle(float a, float b, float timeA,
            float timeB, float currentTime)
        {
            return Com.Brashmonkey.Spriter.interpolation.SpriterLinearInterpolator.interpolator
                .interpolateAngle(a, b, timeA, timeB, currentTime);
        }
    }
}
