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
using Com.Brashmonkey.Spriter;
using Com.Brashmonkey.Spriter.animation;
using Com.Brashmonkey.Spriter.draw;
using Com.Brashmonkey.Spriter.interpolation;
using Com.Brashmonkey.Spriter.file;

namespace Com.Brashmonkey.Spriter.player
{
	/// <summary>SpriterAbstractPlayer is meant to be a base for SpriterPlayer.</summary>
	/// <remarks>
	/// SpriterAbstractPlayer is meant to be a base for SpriterPlayer.
	/// This abstract class has been created, to have the ability to interpolate
	/// other running players at runtime. See #SpriterPlayerInterpolator.
	/// </remarks>
	/// <author>Trixt0r</author>
	public abstract class SpriterAbstractPlayer
	{
		public readonly SpriterKeyFrame lastFrame;

		public readonly SpriterKeyFrame lastTempFrame;

		public bool transitionFixed = true;

		public bool drawn = false;

		protected internal SpriterInterpolator interpolator;

		protected internal int currenObjectsToDraw;

		protected internal int flippedX = 1;

		protected internal int flippedY = 1;

		protected internal int frameSpeed = 10;

		protected internal int zIndex = 0;

		protected internal int currentBonesToAnimate;

		protected internal DrawInstruction[] instructions;

		protected internal SpriterModObject[] moddedObjects;

		protected internal SpriterModObject[] moddedBones;

		protected internal SpriterObject[] tempObjects;

		protected internal SpriterObject[] tempObjects2;

		protected internal SpriterObject[] nonTransformedTempObjects;

		protected internal SpriterBone[] tempBones;

		protected internal SpriterBone[] tempBones2;

		protected internal SpriterBone[] nonTransformedTempBones;

		protected internal IList<SpriterAnimation> animations;

		protected internal SpriterAbstractObject rootParent;

		protected internal SpriterAbstractObject tempParent;

		protected internal float angle = 0f;

		protected internal float scale = 1f;

		protected internal float pivotX = 0f;

		protected internal float pivotY = 0f;

		protected internal long frame;

		protected internal List<SpriterAbstractPlayer> players;

		private bool generated = false;

		internal SpriterRectangle rect;

		public readonly FileLoader loader;

		public bool updateObjects = true;

		public bool updateBones = true;

        public float depth = 1f;

		/// <summary>Constructs a new SpriterAbstractPlayer object which is able to animate SpriterBone instances and SpriterObject instances.
		/// 	</summary>
		/// <remarks>Constructs a new SpriterAbstractPlayer object which is able to animate SpriterBone instances and SpriterObject instances.
		/// 	</remarks>
		/// <param name="loader">
		/// 
		/// <see cref="FileLoader{I}">FileLoader&lt;I&gt;
		/// 	</see>
		/// which you have to implement on your own.
		/// </param>
		/// <param name="keyframes">
		/// A list of SpriterKeyFrame arrays. See
		/// <see cref="SpriterKeyFrameProvider.generateKeyFramePool(com.discobeard.spriter.dom.SpriterData, com.discobeard.spriter.dom.Entity)
		/// 	">SpriterKeyFrameProvider.generateKeyFramePool(com.discobeard.spriter.dom.SpriterData, com.discobeard.spriter.dom.Entity)
		/// 	</see>
		/// to get the list.
		/// Generate these keyframes once to save memory.
		/// </param>
		public SpriterAbstractPlayer(FileLoader loader, IList<SpriterAnimation> animations)
		{
			this.loader = loader;
			this.animations = animations;
			this.rootParent = new SpriterBone();
			this.tempParent = new SpriterBone();
			this.rootParent.setName("playerRoot");
			this.tempParent.setName("playerRoot");
			this.lastFrame = new SpriterKeyFrame();
			this.lastTempFrame = new SpriterKeyFrame();
			this.interpolator = SpriterLinearInterpolator
				.interpolator;
			this.players = new List<SpriterAbstractPlayer>();
			rect = new SpriterRectangle(0, 0, 0, 0);
		}

		/// <summary>Generates data which is necessary to animate all animations as intended.
		/// 	</summary>
		/// <remarks>
		/// Generates data which is necessary to animate all animations as intended.
		/// This method has to called inside the specific constructor.
		/// </remarks>
		protected internal void generateData()
		{
			int maxObjects = 0;
			int maxBones = 0;
			int maxBonesFrameIndex = 0;
			int maxObjectsFrameIndex = 0;
			int maxBonesAnimationIndex = 0;
			int maxObjectsAnimationIndex = 0;
			foreach (SpriterAnimation animation in this.animations)
			{
				foreach (SpriterKeyFrame frame in animation.frames)
				{
					maxBones = System.Math.Max(frame.getBones().Length, maxBones);
					if (maxBones == frame.getBones().Length)
					{
						maxBonesFrameIndex = frame.getId();
						maxBonesAnimationIndex = animation.id;
					}
					maxObjects = System.Math.Max(frame.getObjects().Length, maxObjects);
					if (maxObjects == frame.getObjects().Length)
					{
						maxObjectsFrameIndex = frame.getId();
						maxObjectsAnimationIndex = animation.id;
					}
					foreach (SpriterObject o in frame.getObjects())
					{
						o.setLoader(this.loader);
						o.setRef(this.loader.findReference(o.getRef()));
					}
				}
			}
			this.instructions = new DrawInstruction[maxObjects];
			this.moddedObjects = new SpriterModObject[this.instructions
				.Length];
			this.tempObjects = new SpriterObject[this.instructions
				.Length];
			this.tempObjects2 = new SpriterObject[this.instructions
				.Length];
			this.nonTransformedTempObjects = new SpriterObject
				[this.instructions.Length];
			for (int i = 0; i < this.instructions.Length; i++)
			{
				this.instructions[i] = new DrawInstruction(null, 0,
                    0, 0, 0, 0, 0, 0, 0, depth);
				this.tempObjects[i] = new SpriterObject();
				this.tempObjects2[i] = new SpriterObject();
				this.nonTransformedTempObjects[i] = new SpriterObject
					();
				this.nonTransformedTempObjects[i].setId(i);
				this.moddedObjects[i] = new SpriterModObject();
				this.moddedObjects[i].setId(i);
			}
			this.tempBones = new SpriterBone[maxBones];
			this.tempBones2 = new SpriterBone[tempBones.Length
				];
			this.nonTransformedTempBones = new SpriterBone[tempBones
				.Length];
			this.moddedBones = new SpriterModObject[this.tempBones
				.Length];
			for (int i_1 = 0; i_1 < this.tempBones.Length; i_1++)
			{
				this.tempBones[i_1] = new SpriterBone();
				this.tempBones2[i_1] = new SpriterBone();
				this.nonTransformedTempBones[i_1] = new SpriterBone
					();
				this.nonTransformedTempBones[i_1].setId(i_1);
				this.moddedBones[i_1] = new SpriterModObject();
				this.moddedBones[i_1].setId(i_1);
			}
			SpriterBone[] tmpBones1 = new SpriterBone
				[this.tempBones.Length];
			SpriterBone[] tmpBones2 = new SpriterBone
				[this.tempBones.Length];
			SpriterObject[] tmpObjs1 = new SpriterObject
				[this.instructions.Length];
			SpriterObject[] tmpObjs2 = new SpriterObject
				[this.instructions.Length];
			for (int i_2 = 0; i_2 < tmpObjs1.Length; i_2++)
			{
				tmpObjs1[i_2] = new SpriterObject();
				tmpObjs2[i_2] = new SpriterObject();
			}
			for (int i_3 = 0; i_3 < tmpBones1.Length; i_3++)
			{
				tmpBones1[i_3] = new SpriterBone();
				tmpBones2[i_3] = new SpriterBone();
			}
			this.lastFrame.setBones(tmpBones1);
			this.lastFrame.setObjects(tmpObjs1);
			this.lastTempFrame.setBones(tmpBones2);
			this.lastTempFrame.setObjects(tmpObjs2);
			foreach (SpriterObject @object in this.animations
				[maxObjectsAnimationIndex].frames[maxObjectsFrameIndex].getObjects())
			{
				for (int i_4 = 0; i_4 < this.nonTransformedTempObjects.Length; i_4++)
				{
					if (this.nonTransformedTempObjects[i_4].getId() == @object.getId())
					{
						@object.copyValuesTo(this.nonTransformedTempObjects[i_4]);
					}
				}
			}
			foreach (SpriterBone bone in this.animations[maxBonesAnimationIndex
				].frames[maxBonesFrameIndex].getBones())
			{
				for (int i_4 = 0; i_4 < this.nonTransformedTempBones.Length; i_4++)
				{
					if (this.nonTransformedTempBones[i_4].getId() == bone.getId())
					{
						bone.copyValuesTo(this.nonTransformedTempBones[i_4]);
					}
				}
			}
			this.generated = true;
		}

		/// <summary>Updates this player and translates the animation to xOffset and yOffset.
		/// 	</summary>
		/// <remarks>
		/// Updates this player and translates the animation to xOffset and yOffset.
		/// Frame is updated by previous set frame speed (See
		/// <see cref="setFrameSpeed(int)">setFrameSpeed(int)</see>
		/// ).
		/// This method makes sure that the keyframes get played back.
		/// </remarks>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		public void update(float xOffset, float yOffset)
		{
			if (!this.generated)
			{
				System.Console.Out.WriteLine("Warning! You can not update this player, since necessary data has not been initialized!"
					);
			}
			else
			{
				this.step(xOffset, yOffset);
			}
		}

		/// <summary>Has to be implemented by the specific player.</summary>
		/// <remarks>Has to be implemented by the specific player.</remarks>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		protected internal abstract void step(float xOffset, float yOffset);

		/// <summary>Interpolates the objects of firstFrame and secondFrame.</summary>
		/// <remarks>Interpolates the objects of firstFrame and secondFrame.</remarks>
		/// <param name="currentFrame"></param>
		/// <param name="nextFrame"></param>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		protected internal virtual void transformObjects(SpriterKeyFrame
			 currentFrame, SpriterKeyFrame nextFrame, float
			 xOffset, float yOffset)
		{
			this.rect.set(xOffset, yOffset, xOffset, yOffset);
			if (!this.updateObjects)
			{
				return;
			}
			this.updateTransformedTempObjects(nextFrame.getObjects(), this.tempObjects2);
			this.updateTempObjects(currentFrame.getObjects(), this.nonTransformedTempObjects);
			for (int i = 0; i < this.currenObjectsToDraw; i++)
			{
				if (this.tempObjects[i].active)
				{
					this.tweenObject(this.nonTransformedTempObjects[i], nextFrame.getObjectFor(this.nonTransformedTempObjects
						[i]), i, currentFrame.getTime(), nextFrame.getTime());
				}
			}
		}

		//else this.tweenObject(this.animations.get(0).frames.get(0).getObjectFor(this.nonTransformedTempObjects[i]), nextFrame.getObjectFor(this.nonTransformedTempObjects[i]), i, currentFrame.getTime(), nextFrame.getTime());
		protected internal virtual void setInstructionRef(DrawInstruction
			 dI, SpriterObject obj1, SpriterObject
			 obj2)
		{
			dI.@ref = obj1.getRef();
			dI.loader = obj1.getLoader();
			dI.obj = obj1;
		}

		/// <summary>Interpolates the bones for this animation.</summary>
		/// <remarks>Interpolates the bones for this animation.</remarks>
		/// <param name="currentFrame">first keyframe</param>
		/// <param name="nextFrame">second keyframe</param>
		/// <param name="currentAnimationTime"></param>
		/// <param name="key2StartTime"></param>
		/// <returns>interpolated SpriterBone array</returns>
		protected internal virtual void transformBones(SpriterKeyFrame
			 currentFrame, SpriterKeyFrame nextFrame, float
			 xOffset, float yOffset)
		{
			if (this.rootParent.getParent() != null)
			{
				this.translateRoot();
			}
			else
			{
				this.tempParent.setX(xOffset);
				this.tempParent.setY(yOffset);
				this.tempParent.setAngle(this.angle);
				this.tempParent.setScaleX(this.flippedX);
				this.tempParent.setScaleY(this.flippedY);
			}
			this.setScale(this.scale);
			if (!this.updateBones)
			{
				return;
			}
			this.updateTransformedTempObjects(nextFrame.getBones(), this.tempBones2);
			this.updateTempObjects(currentFrame.getBones(), this.nonTransformedTempBones);
			for (int i = 0; i < this.nonTransformedTempBones.Length; i++)
			{
				if (this.tempBones[i].active)
				{
					this.tweenBone(this.nonTransformedTempBones[i], nextFrame.getBoneFor(this.nonTransformedTempBones
						[i]), i, currentFrame.getTime(), nextFrame.getTime());
				}
			}
		}

		//else this.tweenBone(this.nonTransformedTempBones[i], this.animations.get(0).frames.get(0).getBoneFor(this.nonTransformedTempBones[i]), i, currentFrame.getTime(), nextFrame.getTime());
		private void tweenObject(SpriterObject currentObject
			, SpriterObject nextObject, int i, long startTime
			, long endTime)
		{
			DrawInstruction dI = this.instructions[i];
			currentObject.copyValuesTo(this.tempObjects[i]);
			SpriterAbstractObject parent = null;
			if (!currentObject.isTransientObject())
			{
				this.tempObjects[i].setTimeline((nextObject != null) ? currentObject.getTimeline(
					) : -1);
				parent = (currentObject.hasParent()) ? this.tempBones[currentObject.getParentId()
					] : this.tempParent;
				if (nextObject != null)
				{
					if (parent != this.tempParent)
					{
						if (!currentObject.getParent().equals(nextObject.getParent()))
						{
							nextObject = (SpriterObject)this.getTimelineObject
								(currentObject, this.tempObjects2);
							SpriterCalculator.reTranslateRelative(parent, nextObject);
							nextObject.setAngle(nextObject.getAngle() * this.flippedX * this.flippedY);
						}
					}
					else
					{
						if (nextObject.hasParent())
						{
							nextObject = (SpriterObject)this.getTimelineObject
								(currentObject, this.tempObjects2);
							SpriterCalculator.reTranslateRelative(parent, nextObject);
							nextObject.setAngle(nextObject.getAngle() * this.flippedX * this.flippedY);
						}
					}
					if (this.tempObjects[i].active)
					{
						this.interpolateSpriterObject(this.tempObjects[i], currentObject, nextObject, startTime
							, endTime);
					}
				}
				this.moddedObjects[currentObject.getId()].modSpriterObject(this.tempObjects[i]);
				if (this.transitionFixed)
				{
					this.tempObjects[i].copyValuesTo(this.lastFrame.getObjects()[i]);
				}
				else
				{
					this.tempObjects[i].copyValuesTo(this.lastTempFrame.getObjects()[i]);
				}
			}
			else
			{
				parent = this.tempParent;
			}
			if (!this.tempObjects[i].hasParent())
			{
				this.tempObjects[i].setX(this.tempObjects[i].getX() + this.pivotX);
				this.tempObjects[i].setY(this.tempObjects[i].getY() + this.pivotY);
			}
			this.translateRelative(this.tempObjects[i], parent);
			if (this.moddedObjects[currentObject.getId()].getRef() != null)
			{
				this.tempObjects[i].setRef(this.moddedObjects[currentObject.getId()].getRef());
			}
			if (this.moddedObjects[currentObject.getId()].getLoader() != null)
			{
				this.tempObjects[i].setLoader(this.moddedObjects[currentObject.getId()].getLoader
					());
			}
			this.tempObjects[i].copyValuesTo(dI);
			this.setInstructionRef(dI, this.tempObjects[i], nextObject);
		}

		private void tweenBone(SpriterBone currentBone, SpriterBone
			 nextBone, int i, long startTime, long endTime)
		{
			currentBone.copyValuesTo(this.tempBones[i]);
			this.tempBones[i].setTimeline((nextBone != null) ? currentBone.getTimeline() : -1
				);
			SpriterAbstractObject parent = (this.tempBones[i]
				.hasParent()) ? this.tempBones[this.tempBones[i].getParentId()] : this.tempParent;
			if (nextBone != null)
			{
				if (parent != this.tempParent)
				{
					if (!currentBone.getParent().equals(nextBone.getParent()))
					{
						nextBone = (SpriterBone)this.getTimelineObject(currentBone
							, this.tempBones2);
						SpriterCalculator.reTranslateRelative(parent, nextBone);
						nextBone.setAngle(nextBone.getAngle() * this.flippedX * this.flippedY);
					}
				}
				else
				{
					if (nextBone.hasParent())
					{
						nextBone = (SpriterBone)this.getTimelineObject(currentBone
							, this.tempBones2);
						SpriterCalculator.reTranslateRelative(parent, nextBone);
						nextBone.setAngle(nextBone.getAngle() * this.flippedX * this.flippedY);
					}
				}
				if (this.tempBones[i].active)
				{
					this.interpolateAbstractObject(this.tempBones[i], currentBone, nextBone, startTime
						, endTime);
				}
			}
			this.moddedBones[currentBone.getId()].modSpriterBone(this.tempBones[i]);
			if (this.transitionFixed)
			{
				this.tempBones[i].copyValuesTo(this.lastFrame.getBones()[i]);
			}
			else
			{
				this.tempBones[i].copyValuesTo(this.lastTempFrame.getBones()[i]);
			}
			if (!this.tempBones[i].hasParent() || !this.moddedBones[currentBone.getId()].isActive
				())
			{
				this.tempBones[i].setX(this.tempBones[i].getX() + this.pivotX);
				this.tempBones[i].setY(this.tempBones[i].getY() + this.pivotY);
			}
			this.translateRelative(this.tempBones[i], parent);
		}

		private void updateTransformedTempObjects(SpriterAbstractObject
			[] source, SpriterAbstractObject[] target)
		{
			for (int i = 0; i < source.Length; i++)
			{
				this.updateTransformedTempObject(source[i], target[i]);
			}
		}

		private void updateTransformedTempObject(SpriterAbstractObject
			 source, SpriterAbstractObject target)
		{
			source.copyValuesTo(target);
			if (!target.hasParent())
			{
				target.setX(target.getX() + this.pivotX);
				target.setY(target.getY() + this.pivotY);
			}
			this.translateRelative(target, (target.hasParent()) ? this.tempBones2[target.getParentId
				()] : this.tempParent);
		}

		private void updateTempObjects(SpriterAbstractObject
			[] source, SpriterAbstractObject[] target)
		{
			for (int i = 0; i < source.Length; i++)
			{
				this.updateTempObject(source[i], target);
			}
		}

		private void updateTempObject(SpriterAbstractObject
			 source, SpriterAbstractObject[] target)
		{
			bool found = false;
			for (int j = 0; j < target.Length && !found; j++)
			{
				if (source.getId() == target[j].getId())
				{
					source.copyValuesTo(target[j]);
					found = true;
				}
			}
		}

		private void translateRoot()
		{
			this.rootParent.copyValuesTo(tempParent);
			this.tempParent.setAngle(this.tempParent.getAngle() * this.flippedX * this.flippedY + this
				.rootParent.getParent().getAngle());
			this.tempParent.setScaleX(this.tempParent.getScaleX() * this.rootParent.getParent
				().getScaleX());
			this.tempParent.setScaleY(this.tempParent.getScaleY() * this.rootParent.getParent
				().getScaleY());
			SpriterCalculator.translateRelative(this.rootParent.getParent
				(), this.tempParent);
		}

		protected internal virtual SpriterAbstractObject 
			getTimelineObject(SpriterAbstractObject @object, 
			SpriterAbstractObject[] objects)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i].getTimeline().Equals(@object.getTimeline()))
				{
					return objects[i];
				}
			}
			return null;
		}

		private void interpolateAbstractObject(SpriterAbstractObject
			 target, SpriterAbstractObject obj1, SpriterAbstractObject
			 obj2, float startTime, float endTime)
		{
			if (obj2 == null)
			{
				return;
			}
			target.setX(this.interpolate(obj1.getX(), obj2.getX(), startTime, endTime, this.frame
				));
			target.setY(this.interpolate(obj1.getY(), obj2.getY(), startTime, endTime, this.frame
				));
			target.setScaleX(this.interpolate(obj1.getScaleX(), obj2.getScaleX(), startTime, 
				endTime, this.frame));
			target.setScaleY(this.interpolate(obj1.getScaleY(), obj2.getScaleY(), startTime, 
				endTime, this.frame));
			target.setAngle(this.interpolateAngle(obj1.getAngle(), obj2.getAngle(), startTime
				, endTime, this.frame));
		}

		private void interpolateSpriterObject(SpriterObject
			 target, SpriterObject obj1, SpriterObject
			 obj2, float startTime, float endTime)
		{
			if (obj2 == null)
			{
				return;
			}
			this.interpolateAbstractObject(target, obj1, obj2, startTime, endTime);
			target.setPivotX(this.interpolate(obj1.getPivotX(), obj2.getPivotX(), startTime, 
				endTime, this.frame));
			target.setPivotY(this.interpolate(obj1.getPivotY(), obj2.getPivotY(), startTime, 
				endTime, this.frame));
			target.setAlpha(this.interpolateAngle(obj1.getAlpha(), obj2.getAlpha(), startTime
				, endTime, this.frame));
		}

		private void translateRelative(SpriterAbstractObject
			 @object, SpriterAbstractObject parent)
		{
			@object.setAngle(@object.getAngle() * this.flippedX * this.flippedY + parent.getAngle()
				);
			@object.setScaleX(@object.getScaleX() * parent.getScaleX());
			@object.setScaleY(@object.getScaleY() * parent.getScaleY());
			SpriterCalculator.translateRelative(parent, @object);
		}

		/// <summary>
		/// See
		/// <see cref="SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	">SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	</see>
		/// Can be inherited, to handle other interpolation techniques. Standard is linear interpolation.
		/// </summary>
		protected internal virtual float interpolate(float a, float b, float timeA, float
			 timeB, float currentTime)
		{
			return this.interpolator.interpolate(a, b, timeA, timeB, currentTime);
		}

		/// <summary>
		/// See
		/// <see cref="SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	">SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	</see>
		/// Can be inherited, to handle other interpolation techniques. Standard is linear interpolation.
		/// </summary>
		protected internal virtual float interpolateAngle(float a, float b, float timeA, 
			float timeB, float currentTime)
		{
			return this.interpolator.interpolateAngle(a, b, timeA, timeB, currentTime);
		}

		/// <summary>Calculates the bounding box for the current running animation.</summary>
		/// <remarks>
		/// Calculates the bounding box for the current running animation.
		/// Call this method after updating the spriter player.
		/// </remarks>
		/// <param name="bone">root to start at. Set null, to iterate through all objects.</param>
		public virtual void calcBoundingBox(SpriterBone bone
			)
		{
			if (bone == null)
			{
				this.calcBoundingBoxForAll();
			}
			else
			{
				bone.boundingBox.set(this.rect);
				foreach (SpriterObject @object in bone.getChildObjects())
				{
					SpriterPoint[] points = this.tempObjects[@object.getId()]
						.getBoundingBox();
					bone.boundingBox.left = System.Math.Min(System.Math.Min(System.Math.Min(System.Math
						.Min(points[0].x, points[1].x), points[2].x), points[3].x), bone.boundingBox.left
						);
					bone.boundingBox.right = System.Math.Max(System.Math.Max(System.Math.Max(System.Math
						.Max(points[0].x, points[1].x), points[2].x), points[3].x), bone.boundingBox.right
						);
					bone.boundingBox.top = System.Math.Max(System.Math.Max(System.Math.Max(System.Math
						.Max(points[0].y, points[1].y), points[2].y), points[3].y), bone.boundingBox.top
						);
					bone.boundingBox.bottom = System.Math.Min(System.Math.Min(System.Math.Min(System.Math
						.Min(points[0].y, points[1].y), points[2].y), points[3].y), bone.boundingBox.bottom
						);
				}
				this.rect.set(bone.boundingBox);
				foreach (SpriterBone child in bone.getChildBones(
					))
				{
					calcBoundingBox(child);
					bone.boundingBox.set(child.boundingBox);
				}
				this.rect.set(bone.boundingBox);
			}
			this.rect.calculateSize();
		}

		private void calcBoundingBoxForAll()
		{
			for (int i = 0; i < this.currenObjectsToDraw; i++)
			{
				SpriterPoint[] points = this.tempObjects[i].getBoundingBox
					();
				this.rect.left = System.Math.Min(System.Math.Min(System.Math.Min(System.Math.Min(
					points[0].x, points[1].x), points[2].x), points[3].x), this.rect.left);
				this.rect.right = System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Max
					(points[0].x, points[1].x), points[2].x), points[3].x), this.rect.right);
				this.rect.top = System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Max(points
					[0].y, points[1].y), points[2].y), points[3].y), this.rect.top);
				this.rect.bottom = System.Math.Min(System.Math.Min(System.Math.Min(System.Math.Min
					(points[0].y, points[1].y), points[2].y), points[3].y), this.rect.bottom);
			}
		}

		/// <returns>array of moddable objects.</returns>
		public virtual SpriterModObject[] getModdedObjects
			()
		{
			return moddedObjects;
		}

		/// <returns>array of moddable bones.</returns>
		public virtual SpriterModObject[] getModdedBones(
			)
		{
			return moddedBones;
		}

		/// <summary>Searches for the right index for a given object.</summary>
		/// <remarks>Searches for the right index for a given object.</remarks>
		/// <param name="object">object to search at.</param>
		/// <returns>right index for the mod object you want access to. -1 if not found.</returns>
		public virtual int getModObjectIndexForObject(SpriterObject
			 @object)
		{
			for (int i = 0; i < this.tempObjects.Length; i++)
			{
				if (this.tempObjects[i].equals(@object))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Searches for the right mod object for the given object.</summary>
		/// <remarks>Searches for the right mod object for the given object.</remarks>
		/// <param name="object">object to search at.</param>
		/// <returns>right mod object you want access to. Null if not found.</returns>
		public virtual SpriterModObject getModObjectForObject
			(SpriterObject @object)
		{
			try
			{
				return this.moddedObjects[this.getModObjectIndexForObject(@object)];
			}
			catch (System.IndexOutOfRangeException)
			{
				return null;
			}
		}

		/// <summary>Searches for the right mod object for the given name.</summary>
		/// <remarks>Searches for the right mod object for the given name.</remarks>
		/// <param name="name">name to search for.</param>
		/// <returns>right mod object you want access to. Null if not found.</returns>
		public virtual SpriterModObject getModObjectByName
			(string name)
		{
			try
			{
				return getModObjectForObject(this.getObjectByName(name));
			}
			catch (System.Exception)
			{
				System.Console.Error.WriteLine("Could not find object \"" + name + "\"");
				return null;
			}
		}

		/// <summary>Searches for the right index for a given bone.</summary>
		/// <remarks>Searches for the right index for a given bone.</remarks>
		/// <param name="bone">bone to search at.</param>
		/// <returns>right index for the mod bone you want access to. -1 if not found.</returns>
		public virtual int getModBoneIndexForBone(SpriterBone
			 bone)
		{
			for (int i = 0; i < this.tempObjects.Length; i++)
			{
				if (this.tempBones[i].equals(bone))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Searches for the right mod bone for the given bone.</summary>
		/// <remarks>Searches for the right mod bone for the given bone.</remarks>
		/// <param name="bone">bone to search at.</param>
		/// <returns>right mod bone you want access to. Null if not found.</returns>
		public virtual SpriterModObject getModBoneForBone
			(SpriterBone bone)
		{
			try
			{
				return this.moddedBones[this.getModBoneIndexForBone(bone)];
			}
			catch (System.IndexOutOfRangeException)
			{
				return null;
			}
		}

		/// <summary>Searches for the right mod bone for the given name.</summary>
		/// <remarks>Searches for the right mod bone for the given name.</remarks>
		/// <param name="name">name to search for.</param>
		/// <returns>right mod bone you want access to. Null if not found.</returns>
		public virtual SpriterModObject getModBoneByName(
			string name)
		{
			try
			{
				return getModBoneForBone(this.getBoneByName(name));
			}
			catch (System.Exception)
			{
				System.Console.Error.WriteLine("Could not find bone \"" + name + "\"");
				return null;
			}
		}

		/// <summary>Changes the current frame to the given one.</summary>
		/// <remarks>Changes the current frame to the given one.</remarks>
		/// <param name="frame">the frame to set</param>
		public virtual void setFrame(long frame)
		{
			this.frame = frame;
		}

		/// <returns>the current frame</returns>
		public virtual long getFrame()
		{
			return frame;
		}

		/// <param name="frameSpeed">the frameSpeed to set. Higher value meens playback speed. frameSpeed == 0 means no playback speed.
		/// 	</param>
		public virtual void setFrameSpeed(int frameSpeed)
		{
			this.frameSpeed = frameSpeed;
		}

		/// <returns>the current frameSpeed</returns>
		public virtual int getFrameSpeed()
		{
			return frameSpeed;
		}

		/// <summary>Flips this around the x-axis.</summary>
		/// <remarks>Flips this around the x-axis.</remarks>
		public virtual void flipX()
		{
			this.flippedX *= -1;
			foreach (SpriterAbstractPlayer player in this.players)
			{
				player.flippedX = this.flippedX;
			}
		}

		/// <returns>Indicates whether this is flipped around the x-axis or not. 1 means is not flipped, -1 is flipped.
		/// 	</returns>
		public virtual int getFlipX()
		{
			return this.flippedX;
		}

        public virtual void setFlipX(int flipX)
        {
            this.flippedX = flipX;
            foreach (SpriterAbstractPlayer player in this.players)
            {
                player.flippedX = this.flippedX;
            }
        }

        public virtual void setFlipY(int flipY)
        {
            this.flippedY = flipY;
            foreach (SpriterAbstractPlayer player in this.players)
            {
                player.flippedY = this.flippedY;
            }
        }

		/// <summary>Flips this around the y-axis.</summary>
		/// <remarks>Flips this around the y-axis.</remarks>
		public virtual void flipY()
		{
			this.flippedY *= -1;
			foreach (SpriterAbstractPlayer player in this.players)
			{
				player.flippedY = this.flippedY;
			}
		}

		/// <returns>Indicates whether this is flipped around the y-axis or not. 1 means is not flipped, -1 is flipped.
		/// 	</returns>
		public virtual float getFlipY()
		{
			return this.flippedY;
		}

		/// <summary>Changes the angle of this.</summary>
		/// <remarks>Changes the angle of this.</remarks>
		/// <param name="angle">in degrees to rotate all objects , angle = 0 means no rotation.
		/// 	</param>
		public virtual void setAngle(float angle)
		{
			this.angle = angle;
			this.rootParent.setAngle(this.angle);
		}

		/// <returns>The current angle in degrees.</returns>
		public virtual float getAngle()
		{
			return this.angle;
		}

		/// <returns>the scale. 1 means not scale. 0.5 means half scale.</returns>
		public virtual float getScale()
		{
			return scale;
		}

		/// <summary>Scales this to the given value.</summary>
		/// <remarks>Scales this to the given value.</remarks>
		/// <param name="scale">the scale to set, scale = 1.0 normal scale.</param>
		public virtual void setScale(float scale)
		{
			this.scale = scale;
			this.rootParent.setScaleX(this.scale * this.flippedX);
			this.rootParent.setScaleY(this.scale * this.flippedY);
			this.tempParent.setScaleX(this.scale * this.flippedX);
			this.tempParent.setScaleY(this.scale * this.flippedY);
		}

		/// <summary>Sets the center point of this.</summary>
		/// <remarks>Sets the center point of this. pivotX = 0, pivotY = 0 means the same rotation point as in the Spriter editor.
		/// 	</remarks>
		/// <param name="pivotX"></param>
		/// <param name="pivotY"></param>
		public virtual void setPivot(float pivotX, float pivotY)
		{
			this.pivotX = pivotX;
			this.pivotY = pivotY;
		}

		/// <summary>Returns the x center coordinate of this.</summary>
		/// <remarks>Returns the x center coordinate of this.</remarks>
		/// <returns>pivot x</returns>
		public virtual float getPivotX()
		{
			return this.pivotX;
		}

		/// <summary>Returns the y center coordinate of this.</summary>
		/// <remarks>Returns the y center coordinate of this.</remarks>
		/// <returns>pivot y</returns>
		public virtual float getPivotY()
		{
			return this.pivotY;
		}

		/// <returns>Returns the current DrawInstruction array</returns>
		public virtual DrawInstruction[] getDrawInstructions
			()
		{
			return this.instructions;
		}

		/// <summary>Searches for the bone index with the given name and returns the right one
		/// 	</summary>
		/// <param name="name">name of the bone.</param>
		/// <returns>index of the bone if the given name was found, otherwise it returns -1</returns>
		public virtual int getBoneIndexByName(string name)
		{
			for (int i = 0; i < this.tempBones.Length; i++)
			{
				if (name.Equals(this.tempBones[i].getName()))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Searches for the object index with the given name and returns the right one
		/// 	</summary>
		/// <param name="name">name of the object.</param>
		/// <returns>index of the object if the given name was found, otherwise it returns -1
		/// 	</returns>
		public virtual int getObjectIndexByName(string name)
		{
			for (int i = 0; i < this.tempObjects.Length; i++)
			{
				if (name.Equals(this.tempObjects[i].getName()))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Searches for the bone index with the given name and returns the right one
		/// 	</summary>
		/// <param name="name">name of the bone.</param>
		/// <returns>index of the bone if the given name was found, otherwise it returns null
		/// 	</returns>
		public virtual SpriterBone getBoneByName(string name
			)
		{
			int i = this.getBoneIndexByName(name);
			if (i != -1)
			{
				return this.getRuntimeBones()[i];
			}
			else
			{
				return null;
			}
		}

		/// <summary>Searches for the object index with the given name and returns the right one
		/// 	</summary>
		/// <param name="name">name of the object.</param>
		/// <returns>index of the object if the given name was found, otherwise it returns null
		/// 	</returns>
		public virtual SpriterObject getObjectByName(string
			 name)
		{
			int i = this.getObjectIndexByName(name);
			if (i != -1)
			{
				return this.getRuntimeObjects()[i];
			}
			else
			{
				return null;
			}
		}

		/// <returns>
		/// the current interpolator. See #SpriterInterpolator. You can implement your own one,
		/// which interpolates the animations as you wish.
		/// </returns>
		public virtual SpriterInterpolator getInterpolator
			()
		{
			return interpolator;
		}

		/// <param name="interpolator">
		/// the interpolator to set. See #SpriterInterpolator. You can implement your own one,
		/// which interpolates the animations as you wish.
		/// </param>
		public virtual void setInterpolator(SpriterInterpolator
			 interpolator)
		{
			this.interpolator = interpolator;
		}

		/// <returns>the current bones which where interpolated for the current animation. Bones are not flipped.
		/// 	</returns>
		public virtual SpriterBone[] getRuntimeBones()
		{
			return this.tempBones;
		}

		/// <returns>the current objects which where interpolated for the current animation. Objects are flipped.
		/// 	</returns>
		public virtual SpriterObject[] getRuntimeObjects(
			)
		{
			return this.tempObjects;
		}

		/// <returns>the rootParent</returns>
		public virtual SpriterAbstractObject getRootParent
			()
		{
			return rootParent;
		}

		/// <param name="rootParent">the rootParent to set</param>
		internal virtual void setRootParent(SpriterAbstractObject
			 rootParent)
		{
			this.rootParent = rootParent;
		}

		/// <param name="rootParent">the rootParent to set</param>
		internal virtual void changeRootParent(SpriterAbstractObject
			 rootParent)
		{
			this.rootParent.setParent(rootParent);
		}

		/// <returns>the current z-index. This gets relevant if you attach a #SpriterAbstractPlayer to another.
		/// 	</returns>
		public virtual int getZIndex()
		{
			return this.zIndex;
		}

		/// <summary>Meant to change the drawing order if this player is held by another #SpriterAbstractPlayer.
		/// 	</summary>
		/// <remarks>Meant to change the drawing order if this player is held by another #SpriterAbstractPlayer.
		/// 	</remarks>
		/// <param name="zIndex">Higher means that the object will be drawn later.</param>
		public virtual void setZIndex(int zIndex)
		{
			this.zIndex = zIndex;
		}

		/// <summary>Attaches a given player to this.</summary>
		/// <remarks>Attaches a given player to this.</remarks>
		/// <param name="player">indicates the player which has to be attached.</param>
		/// <param name="root">
		/// indicates the object the attached player has to follow.
		/// Set to
		/// <see cref="getRootParent()">getRootParent()</see>
		/// to attach the player to the same position as this player.
		/// </param>
		public virtual void attachPlayer(SpriterAbstractPlayer
			 player, SpriterAbstractObject root)
		{
			this.players.Add(player);
			player.changeRootParent(root);
		}

		/// <summary>Removes the given player from this one.</summary>
		/// <remarks>Removes the given player from this one.</remarks>
		/// <param name="player">indicates the player which has to be removed.</param>
		public virtual void removePlayer(SpriterAbstractPlayer
			 player)
		{
			this.players.Remove(player);
			player.changeRootParent(null);
		}

		public virtual IList<SpriterAbstractPlayer
			> getAttachedPlayers()
		{
			return this.players;
		}

		/// <summary>Indicates whether the given player is attached to this or not.</summary>
		/// <remarks>Indicates whether the given player is attached to this or not.</remarks>
		/// <param name="player">the player to ask after.</param>
		/// <returns>true if player is attached or not.</returns>
		public virtual bool containsPlayer(SpriterAbstractPlayer
			 player)
		{
			return this.players.Contains(player);
		}

		public virtual void updateAbstractObject(SpriterAbstractObject @object)
		{
			if (@object.hasParent())
			{
				SpriterAbstractObject obj = (@object is SpriterBone
                    ) ? (SpriterAbstractObject)this.lastFrame.getBones()[@object.getId()] : this.lastFrame.getObjects()[@object
					.getId()];
				SpriterCalculator.translateRelative(this.tempBones[@object
					.getParentId()], obj.getX(), obj.getY(), @object);
			}
		}

		/// <summary>Transforms the given bone with relative information to its parent</summary>
		/// <param name="bone"></param>
		public virtual SpriterRectangle getBoundingBox()
		{
			return this.rect;
		}

		public virtual int getObjectsToDraw()
		{
			return this.currenObjectsToDraw;
		}

		public virtual int getBonesToAnimate()
		{
			return this.currentBonesToAnimate;
		}

        public void SetDepth(float depth)
        {
            //this.depth -=  depth;
            
            this.depth = depth;
        }
	}
}
