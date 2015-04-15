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

using Com.Brashmonkey.Spriter.player;

namespace Com.Brashmonkey.Spriter.player
{
	/// <summary>This class is made to interpolate between two running animations.</summary>
	/// <remarks>
	/// This class is made to interpolate between two running animations.
	/// The idea is, to give an instance of this class two AbstractSpriterPlayer objects which hold and animate the same spriter entity.
	/// This will interpolate the runtime transformations of the bones and objects with a weight between 0 and 1.
	/// You will be also able to interpolate SpriterPlayerInterpolators with each other, since it extends  #SpriterAbstractPlayer.
	/// Note that this #SpriterAbstractPlayer needs 3 times more calculation effort than a normal #SpriterPlayer.
	/// </remarks>
	/// <author>Trixt0r</author>
	public class SpriterPlayerInterpolator : SpriterAbstractPlayer
	{
		private SpriterAbstractPlayer first;

		private SpriterAbstractPlayer second;

		private float weight;

		public bool updatePlayers = true;

		/// <summary>Returns an instance of this class, which will manage the interpolation between two #SpriterAbstractPlayer instances.
		/// 	</summary>
		/// <remarks>Returns an instance of this class, which will manage the interpolation between two #SpriterAbstractPlayer instances.
		/// 	</remarks>
		/// <param name="first">player to interpolate with the second one.</param>
		/// <param name="second">player to interpolate with the first one.</param>
		public SpriterPlayerInterpolator(SpriterAbstractPlayer
			 first, SpriterAbstractPlayer second) : base(first.loader, first.animations)
		{
			this.weight = 0.5f;
			setPlayers(first, second);
			this.generateData();
			this.update(0, 0);
		}

		/// <summary>Note: Make sure, that both instances hold the same bone and object structure.
		/// 	</summary>
		/// <remarks>
		/// Note: Make sure, that both instances hold the same bone and object structure.
		/// Otherwise you will not get the interpolation you wish.
		/// </remarks>
		/// <param name="first">SpriterPlayer instance to interpolate.</param>
		/// <param name="second">SpriterPlayer instance to interpolate.</param>
		public virtual void setPlayers(SpriterAbstractPlayer
			 first, SpriterAbstractPlayer second)
		{
			this.first = first;
			this.second = second;
			this.moddedBones = this.first.moddedBones;
			this.moddedObjects = this.first.moddedObjects;
			this.first.setRootParent(this.rootParent);
			this.second.setRootParent(this.rootParent);
		}

		/// <param name="weight">
		/// to set. 0 means the animation of the first player will get played back.
		/// 1 means the second player will get played back.
		/// </param>
		public virtual void setWeight(float weight)
		{
			this.weight = weight;
		}

		/// <returns>The current weight.</returns>
		public virtual float getWeight()
		{
			return this.weight;
		}

		/// <returns>The first player.</returns>
		public virtual SpriterAbstractPlayer getFirst()
		{
			return this.first;
		}

		/// <returns>The second player.</returns>
		public virtual SpriterAbstractPlayer getSecond()
		{
			return this.second;
		}

		protected internal override void step(float xOffset, float yOffset)
		{
			int firstLastSpeed = first.frameSpeed;
			int secondLastSpeed = second.frameSpeed;
			//int speed = this.frameSpeed;
			//if(this.interpolateSpeed)	speed = (int)this.interpolate(first.frameSpeed, second.frameSpeed, 0, 1, this.weight);
			//this.first.frameSpeed = speed;
			//this.second.frameSpeed = speed;
			this.moddedBones = (this.weight <= 0.5f) ? this.first.moddedBones : this.second.moddedBones;
			this.moddedObjects = (this.weight <= 0.5f) ? this.first.moddedObjects : this.second
				.moddedObjects;
			this.currenObjectsToDraw = System.Math.Max(first.currenObjectsToDraw, second.currenObjectsToDraw
				);
            this.currentBonesToAnimate = System.Math.Max(first.currentBonesToAnimate, second.
				currentBonesToAnimate);
			if (this.updatePlayers)
			{
				this.first.update(xOffset, yOffset);
				this.second.update(xOffset, yOffset);
			}
			Com.Brashmonkey.Spriter.animation.SpriterKeyFrame key1 = (first.transitionFixed) ? 
				first.lastFrame : first.lastTempFrame;
			Com.Brashmonkey.Spriter.animation.SpriterKeyFrame key2 = (second.transitionFixed)
				 ? second.lastFrame : second.lastTempFrame;
			this.transformBones(key1, key2, xOffset, yOffset);
			this.transformObjects(first.lastFrame, second.lastFrame, xOffset, yOffset);
			this.first.frameSpeed = firstLastSpeed;
			this.second.frameSpeed = secondLastSpeed;
		}

		protected internal override void setInstructionRef(Com.Brashmonkey.Spriter.draw.DrawInstruction
			 dI, Com.Brashmonkey.Spriter.objects.SpriterObject obj1, Com.Brashmonkey.Spriter.objects.SpriterObject
			 obj2)
		{
			dI.@ref = (this.weight <= 0.5f || obj2 == null) ? obj1.getRef() : obj2.getRef();
			dI.loader = (this.weight <= 0.5f || obj2 == null) ? obj1.getLoader() : obj2.getLoader
				();
			dI.obj = (this.weight <= 0.5f || obj2 == null) ? obj1 : obj2;
		}

		/// <summary>
		/// See
		/// <see cref="Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	">Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	</see>
		/// Can be inherited, to handle other interpolation techniques. Standard is linear interpolation.
		/// </summary>
		protected internal override float interpolate(float a, float b, float timeA, float
			 timeB, float currentTime)
		{
			return this.interpolator.interpolate(a, b, 0, 1, this.weight);
		}

		/// <summary>
		/// See
		/// <see cref="Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	">Com.Brashmonkey.Spriter.SpriterCalculator.calculateInterpolation(float, float, float, float, float)
		/// 	</see>
		/// Can be inherited, to handle other interpolation techniques. Standard is linear interpolation.
		/// </summary>
		protected internal override float interpolateAngle(float a, float b, float timeA, 
			float timeB, float currentTime)
		{
			return this.interpolator.interpolateAngle(a, b, 0, 1, this.weight);
		}
	}
}
