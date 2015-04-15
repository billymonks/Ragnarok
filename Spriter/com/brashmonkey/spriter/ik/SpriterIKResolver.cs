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
using Com.Brashmonkey.Spriter.player;
using System.Collections.Generic;

namespace Com.Brashmonkey.Spriter.ik
{
	public abstract class SpriterIKResolver
	{
		/// <summary>Resolves the inverse kinematics constraint with a specific algtorithm</summary>
		/// <param name="x">the target x value</param>
		/// <param name="y">the target y value</param>
		/// <param name="chainLength">number of parents which are affected</param>
		/// <param name="effector">the actual effector where the resolved information has to be stored in.
		/// 	</param>
		protected abstract void resolve(float x, float y, int chainLength, SpriterAbstractObject effector, SpriterAbstractPlayer player);

		protected internal bool resovling;

		protected internal System.Collections.Generic.Dictionary<SpriterIKObject, SpriterAbstractObject> ikMap;

		protected internal float tolerance;

		public SpriterIKResolver()
		{
			this.resovling = true;
			this.tolerance = 5f;
			this.ikMap = new System.Collections.Generic.Dictionary<SpriterIKObject, SpriterAbstractObject>();
		}

		/// <summary>
		/// Resolves the inverse kinematics constraints with the implemented algorithm in
		/// <see cref="resolve(float, float, int, SpriterAbstractObject, SpriterAbstractPlayer)
		/// 	">resolve(float, float, int, SpriterAbstractObject, SpriterAbstractPlayer)
		/// 	</see>
		/// .
		/// </summary>
		/// <param name="player">player to apply the resolving.</param>
		public virtual void resolve(SpriterAbstractPlayer player)
		{
            foreach(SpriterIKObject key in this.ikMap.Keys)
            {
                for (int j = 0; j < key.iterations; j++)
                {
                    SpriterAbstractObject obj = this.ikMap[key];
                    if (obj is Com.Brashmonkey.Spriter.objects.SpriterBone) obj = player.getRuntimeBones()[obj.getId()];
                    else obj = player.getRuntimeObjects()[obj.getId()];
                    this.resolve(obj.getX(), obj.getY(), key.chainLength, obj, player);
				}
            }
		}

		/// <returns>the resovling</returns>
		public virtual bool isResovling()
		{
			return resovling;
		}

		/// <param name="resovling">the resovling to set</param>
		public virtual void setResovling(bool resovling)
		{
			this.resovling = resovling;
		}

		/// <summary>Adds the given object to the internal SpriterIKObject - SpriterBone map, which works like a HashMap.
		/// 	</summary>
		/// <remarks>
		/// Adds the given object to the internal SpriterIKObject - SpriterBone map, which works like a HashMap.
		/// This means, the values of the given object affect the mapped bone.
		/// </remarks>
		/// <param name="object"></param>
		/// <param name="bone"></param>
		public virtual void mapIKObject(SpriterIKObject @object
			, SpriterAbstractObject abstractObject)
		{
			this.ikMap.Add(@object, abstractObject);
		}

		/// <summary>Removes the given object from the internal map.</summary>
		/// <remarks>Removes the given object from the internal map.</remarks>
		/// <param name="object"></param>
		public virtual void unmapIKObject(SpriterIKObject @object)
		{
            this.ikMap.Remove(@object);
		}

		public virtual float getTolerance()
		{
			return tolerance;
		}

		public virtual void setTolerance(float tolerance)
		{
			this.tolerance = tolerance;
		}

		/// <summary>Changes the state of each effector to unactive.</summary>
		/// <remarks>Changes the state of each effector to unactive. The effect results in non animated bodyparts.
		/// 	</remarks>
		/// <param name="parents">indicates whether parents of the effectors have to be deactivated or not.
		/// 	</param>
		public virtual void deactivateEffectors(SpriterAbstractPlayer player, bool parents)
		{
            foreach (SpriterIKObject key in this.ikMap.Keys)
            {
                SpriterAbstractObject obj = this.ikMap[key];
                if (obj is SpriterBone) obj = player.getRuntimeBones()[obj.getId()];
                else obj = player.getRuntimeObjects()[obj.getId()];
                obj.active = false;
                if (!parents)
                {
                    continue;
                }
                SpriterBone par = (SpriterBone)obj.getParent();
                for (int j = 0; j < key.chainLength && par != null; j++)
                {
                    player.getRuntimeBones()[par.getId()].active = false;
                    par = (SpriterBone)par.getParent();
                }
            }
		}

		public virtual void activateEffectors(SpriterAbstractPlayer player)
		{
            foreach (SpriterIKObject key in this.ikMap.Keys)
            {
                SpriterAbstractObject obj = this.ikMap[key];
                if (obj is SpriterBone) obj = player.getRuntimeBones()[obj.getId()];
                else obj = player.getRuntimeObjects()[obj.getId()];
                obj.active = true;
                SpriterBone par = (SpriterBone)obj.getParent();
				for (int j = 0; j < key.chainLength && par != null; j++)
				{
					player.getRuntimeBones()[par.getId()].active = true;
					par = (SpriterBone)par.getParent();
				}
			}
		}

		public virtual void activateAll(SpriterAbstractPlayer
			 player)
		{
			foreach (SpriterBone bone in player.getRuntimeBones())
			{
				bone.active = true;
			}
			foreach (SpriterObject obj in player.getRuntimeObjects())
			{
				obj.active = true;
			}
		}

		protected internal virtual void updateObject(SpriterAbstractPlayer
			 player, SpriterAbstractObject @object)
		{
			player.updateAbstractObject(@object);
		}

		protected internal virtual void updateRecursively(SpriterAbstractPlayer
			 player, SpriterAbstractObject @object)
		{
			this.updateObject(player, @object);
			if (@object is SpriterBone)
			{
				foreach (SpriterBone child in ((SpriterBone
					)@object).getChildBones())
				{
					this.updateRecursively(player, player.getRuntimeBones()[child.getId()]);
				}
				foreach (SpriterObject child_1 in ((SpriterBone
					)@object).getChildObjects())
				{
					this.updateRecursively(player, player.getRuntimeObjects()[child_1.getId()]);
				}
			}
		}
	}
}
