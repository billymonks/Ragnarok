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
using Com.Brashmonkey.Spriter.file;
using System.Collections.Generic;

namespace Com.Brashmonkey.Spriter.player
{
	/// <summary>A SpriterPlayer is the core of a spriter animation.</summary>
	/// <remarks>
	/// A SpriterPlayer is the core of a spriter animation.
	/// Here you can get as many information as you need.
	/// SpriterPlayer plays the given data with the method
	/// <see cref="SpriterAbstractPlayer.update(float, float)">SpriterAbstractPlayer.update(float, float)
	/// 	</see>
	/// . You have to call this method by your own in your main game loop.
	/// SpriterPlayer updates the frames by its own. See
	/// <see cref="SpriterAbstractPlayer.setFrameSpeed(int)">SpriterAbstractPlayer.setFrameSpeed(int)
	/// 	</see>
	/// for setting the playback speed.
	/// The animations can be drawn by
	/// <see cref="#draw()">#draw()</see>
	/// which draws all objects with your own implemented Drawer.
	/// Accessing bones and animations via names is also possible. See
	/// <see cref="getAnimationIndexByName(string)">getAnimationIndexByName(string)</see>
	/// and
	/// <see cref="SpriterAbstractPlayer.getBoneIndexByName(string)">SpriterAbstractPlayer.getBoneIndexByName(string)
	/// 	</see>
	/// .
	/// You can modify the whole animation or only bones at runtime with some fancy methods provided by this class.
	/// Have a look at
	/// <see cref="SpriterAbstractPlayer.setAngle(float)">SpriterAbstractPlayer.setAngle(float)
	/// 	</see>
	/// ,
	/// <see cref="SpriterAbstractPlayer.flipX()">SpriterAbstractPlayer.flipX()</see>
	/// ,
	/// <see cref="SpriterAbstractPlayer.flipY()">SpriterAbstractPlayer.flipY()</see>
	/// ,
	/// <see cref="SpriterAbstractPlayer.setScale(float)">SpriterAbstractPlayer.setScale(float)
	/// 	</see>
	/// for animation moddification.
	/// And see
	/// <see cref="#setBoneAngle(int,float)">#setBoneAngle(int,float)</see>
	/// ,
	/// <see cref="#setBoneScaleX(int,float)">#setBoneScaleX(int,float)</see>
	/// ,
	/// <see cref="#setBoneScaleY(int,float)">#setBoneScaleY(int,float)</see>
	/// .
	/// All stuff you set you can also receive by the corresponding getters ;) .
	/// </remarks>
	/// <author>Trixt0r</author>
	public class SpriterPlayer : SpriterAbstractPlayer
	{
		private static Dictionary<Entity
			, SpriterPlayer> loaded = new Dictionary
			<Entity, SpriterPlayer
			>();

		protected internal Entity entity;

		private Com.Brashmonkey.Spriter.animation.SpriterAnimation animation;

		private int transitionSpeed = 30;

		private int animationIndex = 0;

		private int currentKey = 0;

		internal Com.Brashmonkey.Spriter.animation.SpriterKeyFrame lastRealFrame;

		internal Com.Brashmonkey.Spriter.animation.SpriterKeyFrame firstKeyFrame;

		internal Com.Brashmonkey.Spriter.animation.SpriterKeyFrame secondKeyFrame;

		internal bool transitionTempFixed = true;

		private int fixCounter = 0;

		private int fixMaxSteps = 100;

		/// <summary>Constructs a new SpriterPlayer object which animates the given Spriter entity.
		/// 	</summary>
		/// <remarks>Constructs a new SpriterPlayer object which animates the given Spriter entity.
		/// 	</remarks>
		/// <param name="data">
		/// 
		/// <see cref="SpriterData">SpriterData
		/// 	</see>
		/// which provides a method to load all needed data to animate. See
		/// <see cref="Spriter#getSpriter(String,com.spriter.file.FileLoader)">Spriter#getSpriter(String,com.spriter.file.FileLoader)
		/// 	</see>
		/// for mor information.
		/// </param>
		/// <param name="entityIndex">The entity which should be handled by this player.</param>
		/// <param name="loader">The loader which has loaded all necessary sprites for the scml file.
		/// 	</param>
		public SpriterPlayer(SpriterData data, Entity
			 entity, FileLoader loader) : base(loader, null)
		{
			this.entity = entity;
			this.frame = 0;
			if (!alreadyLoaded(entity))
			{
				this.animations = SpriterKeyFrameProvider.generateKeyFramePool(data, entity);
				loaded.Add(entity, this);
			}
			else
			{
				this.animations = loaded[entity].animations;
			}
			this.generateData();
			this.animation = this.animations[0];
			this.firstKeyFrame = this.animation.frames[0];
			this.update(0, 0);
		}

		/// <summary>Constructs a new SpriterPlayer object which animates the given Spriter entity.
		/// 	</summary>
		/// <remarks>Constructs a new SpriterPlayer object which animates the given Spriter entity.
		/// 	</remarks>
		/// <param name="data">
		/// 
		/// <see cref="SpriterData">SpriterData
		/// 	</see>
		/// which provides a method to load all needed data to animate. See
		/// <see cref="Spriter#getSpriter(String,com.spriter.file.FileLoader)">Spriter#getSpriter(String,com.spriter.file.FileLoader)
		/// 	</see>
		/// for mor information.
		/// </param>
		/// <param name="entityIndex">The index of the entity which should be handled by this player.
		/// 	</param>
		/// <param name="loader">The loader which has loaded all necessary sprites for the scml file.
		/// 	</param>
		public SpriterPlayer(SpriterData data, int entityIndex, FileLoader loader) : this(data, data.getEntity
			()[entityIndex], loader)
		{
		}

		/// <summary>Constructs a new SpriterPlayer object which animates the given Spriter entity.
		/// 	</summary>
		/// <remarks>Constructs a new SpriterPlayer object which animates the given Spriter entity.
		/// 	</remarks>
		/// <param name="data">
		/// 
		/// <see cref="Com.Brashmonkey.Spriter.Spriter">Com.Brashmonkey.Spriter.Spriter</see>
		/// which provides a method to load all needed data to animate. See
		/// <see cref="Spriter#getSpriter(String,com.spriter.file.FileLoader)">Spriter#getSpriter(String,com.spriter.file.FileLoader)
		/// 	</see>
		/// for mor information.
		/// </param>
		/// <param name="entityIndex">The index of the entity which should be handled by this player.
		/// 	</param>
		/// <param name="loader">The loader which has loaded all necessary sprites for the scml file.
		/// 	</param>
		public SpriterPlayer(Com.Brashmonkey.Spriter.Spriter spriter, int entityIndex, FileLoader loader) : this(spriter.getSpriterData(), spriter.getSpriterData().getEntity
			()[entityIndex], loader)
		{
		}

		protected internal override void step(float xOffset, float yOffset)
		{
			//Fetch information
			IList<Com.Brashmonkey.Spriter.animation.SpriterKeyFrame
				> frameList = this.animation.frames;
			if (this.transitionFixed && this.transitionTempFixed)
			{
				firstKeyFrame = frameList[this.currentKey];
				secondKeyFrame = frameList[(this.currentKey + 1) % frameList.Count];
				//Update
				this.frame += this.frameSpeed;
				if (this.frame >= this.animation.length)
				{
					this.frame = 0;
					this.currentKey = 0;
					firstKeyFrame = frameList[this.currentKey];
					secondKeyFrame = frameList[(this.currentKey + 1) % frameList.Count];
				}
				if (this.frame > secondKeyFrame.getTime() && this.frameSpeed >= 0)
				{
					this.currentKey = (this.currentKey + 1) % frameList.Count;
				}
				else
				{
					if (this.frame < firstKeyFrame.getTime())
					{
						if (this.frame < 0)
						{
							this.frame = this.animation.length;
							this.currentKey = frameList.Count - 1;
							firstKeyFrame = frameList[frameList.Count - 1];
							secondKeyFrame = frameList[0];
						}
						else
						{
							this.currentKey = ((this.currentKey - 1) + frameList.Count) % frameList.Count;
						}
					}
				}
			}
			else
			{
				firstKeyFrame = frameList[0];
				secondKeyFrame = this.lastRealFrame;
				float temp = (float)(this.fixCounter) / (float)this.fixMaxSteps;
				this.frame = this.lastRealFrame.getTime() + (long)(this.fixMaxSteps * temp);
				this.fixCounter = System.Math.Min(this.fixCounter + this.transitionSpeed, this.fixMaxSteps
					);
				//Update
				if (this.fixCounter == this.fixMaxSteps)
				{
					this.frame = 0;
					this.fixCounter = 0;
					if (this.lastRealFrame.Equals(this.lastFrame))
					{
						this.transitionFixed = true;
					}
					else
					{
						this.transitionTempFixed = true;
					}
					firstKeyFrame.setTime(0);
				}
			}
			//Interpolate
			this.currenObjectsToDraw = firstKeyFrame.getObjects().Length;
			this.currentBonesToAnimate = firstKeyFrame.getBones().Length;
			this.transformBones(firstKeyFrame, secondKeyFrame, xOffset, yOffset);
			this.transformObjects(firstKeyFrame, secondKeyFrame, xOffset, yOffset);
		}

		/// <summary>Sets the animationIndex for this to the given animationIndex.</summary>
		/// <remarks>
		/// Sets the animationIndex for this to the given animationIndex.
		/// This method can make sure that the switching between to animations is smooth.
		/// By setting transitionSpeed and transitionSteps to appropriate values, you can have nice transitions between two animations.
		/// Setting transitionSpeed to 1 and transitionSteps to 20 means, that this player will need 20 steps to translate the current animation to the given one.
		/// </remarks>
		/// <param name="animationIndex">
		/// Index of animation to set. Get the index with
		/// <see cref="getAnimationIndexByName(string)">getAnimationIndexByName(string)</see>
		/// .
		/// </param>
		/// <param name="transitionSpeed">Speed for the switch between the current animation and the one which has been set.
		/// 	</param>
		/// <param name="transitionSteps">Steps needed for the transition</param>
		/// <exception cref="System.Exception"></exception>
		public virtual void setAnimatioIndex(int animationIndex, int transitionSpeed, int
			 transitionSteps)
		{
			if (animationIndex >= this.entity.getAnimation().Count || animationIndex < 0)
			{
				throw new System.Exception("The given animation index does not exist: " + animationIndex
					 + "\n" + "Index range goes from 0 to " + (this.entity.getAnimation().Count - 1)
					);
			}
			if (this.animationIndex != animationIndex)
			{
				if (this.transitionFixed)
				{
					this.lastRealFrame = this.lastFrame;
					this.transitionFixed = false;
					this.transitionTempFixed = true;
				}
				else
				{
					this.lastRealFrame = this.lastTempFrame;
					this.transitionTempFixed = false;
					this.transitionFixed = true;
				}
				this.transitionSpeed = transitionSpeed;
				this.fixMaxSteps = transitionSteps;
				this.lastRealFrame.setTime(this.frame + 1);
				this.animation = this.animations[animationIndex];
				this.animation.frames[0].setTime(this.frame + 1 + this.fixMaxSteps);
				this.currentKey = 0;
				this.fixCounter = 0;
				this.animationIndex = animationIndex;
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void setAnimationIndex(int animationIndex)
		{
			this.setAnimatioIndex(animationIndex, 1, 1);
		}

		public virtual void setAnimation(string animationName, int transitionSpeed, int transitionSteps
			)
		{
			int index = getAnimationIndexByName(animationName);
			if (index >= this.entity.getAnimation().Count || index < 0)
			{
				throw new System.Exception("The animation \"" + animationName + "\" does not exist!"
					);
			}
			this.setAnimatioIndex(index, transitionSpeed, transitionSteps);
		}

		/// <summary>Searches for the animation index with the given name and returns the right one
		/// 	</summary>
		/// <param name="name">name of the animation.</param>
		/// <returns>index of the animation if the given name was found, otherwise it returns -1
		/// 	</returns>
		public virtual int getAnimationIndexByName(string name)
		{
			Animation anim = this.getAnimationByName(name);
			if (this.getAnimationByName(name) == null)
			{
				return -1;
			}
			else
			{
				return anim.getId();
			}
		}

		/// <summary>Searches for the animation with the given name and returns the right one
		/// 	</summary>
		/// <param name="name">of the animation.</param>
		/// <returns>nimation if the given name was found, otherwise it returns null.</returns>
		public virtual Animation getAnimationByName(string name
			)
		{
			IList<Animation> anims = this
				.entity.getAnimation();
			foreach (Animation anim in anims)
			{
				if (anim.getName().Equals(name))
				{
					return anim;
				}
			}
			return null;
		}

		/// <returns>current animation index, which has same numbering as in the scml file.</returns>
		public virtual int getAnimationIndex()
		{
			return this.animationIndex;
		}

		/// <returns>the entity, which was read from the scml file you loaded before.</returns>
		public virtual Entity getEntity()
		{
			return entity;
		}

		/// <param name="entity">the entity to set</param>
		public virtual void setEntity(Entity entity)
		{
			this.entity = entity;
		}

		/// <returns>the current animation with all its raw data which was read from the scml file.
		/// 	</returns>
		public virtual Com.Brashmonkey.Spriter.animation.SpriterAnimation getAnimation()
		{
			return animation;
		}

		private static bool alreadyLoaded(Entity entity)
		{
			return loaded.ContainsKey(entity);
		}
	}
}
