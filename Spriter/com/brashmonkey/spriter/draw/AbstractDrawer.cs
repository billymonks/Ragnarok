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

namespace Com.Brashmonkey.Spriter.draw
{
	/// <summary>An AbstractDrawer is an object which is able to draw an animated entity.
	/// 	</summary>
	/// <remarks>
	/// An AbstractDrawer is an object which is able to draw an animated entity.
	/// To use a drawer, you have to implement
	/// <see cref="AbstractDrawer{L}.draw(DrawInstruction)">AbstractDrawer&lt;L&gt;.draw(DrawInstruction)
	/// 	</see>
	/// which fits to your environment.
	/// </remarks>
	/// <author>Trixt0r</author>
	/// <?></?>
	public abstract class AbstractDrawer
	{
		public Com.Brashmonkey.Spriter.file.FileLoader loader;

		public static float BONE_LENGTH = 200;

		public static float BONE_WIDTH = 10;

		public bool debugBones = true;

		public bool debubBoxes = true;

		public AbstractDrawer(Com.Brashmonkey.Spriter.file.FileLoader loader)
		{
			this.loader = loader;
		}

		/// <summary>Draws a sprite with the given instruction.</summary>
		/// <remarks>Draws a sprite with the given instruction.</remarks>
		/// <param name="instruction">Instruction to draw with.</param>
		public abstract void draw(Com.Brashmonkey.Spriter.draw.DrawInstruction instruction
			);

		/// <summary>
		/// Draws the playing animation in <code>player</code> with all bones (if
		/// <see cref="AbstractDrawer{L}.drawBones">AbstractDrawer&lt;L&gt;.drawBones</see>
		/// is true) and bounding boxes (if
		/// <see cref="AbstractDrawer{L}.drawBoxes">AbstractDrawer&lt;L&gt;.drawBoxes</see>
		/// is true) but without the corresponding sprites.
		/// </summary>
		/// <param name="player">
		/// 
		/// <see>[Com.Brashmonkey.Spriter.player.]SpriterAbstractPlayer AbstractPlayer</see>
		/// to draw
		/// </param>
		public virtual void debugDraw(Com.Brashmonkey.Spriter.player.SpriterAbstractPlayer
			 player)
		{
			if (debubBoxes)
			{
				this.drawBoxes(player);
			}
			if (debugBones)
			{
				this.drawBones(player);
			}
		}

		protected internal virtual void drawBones(Com.Brashmonkey.Spriter.player.SpriterAbstractPlayer
			 player)
		{
			for (int i = 0; i < player.getBonesToAnimate(); i++)
			{
				Com.Brashmonkey.Spriter.objects.SpriterBone bone = player.getRuntimeBones()[i];
				if (bone.active)
				{
					this.setDrawColor(1, 0, 0, 1);
				}
				else
				{
					this.setDrawColor(0, 1, 1, 1);
				}
                float xx = bone.getX() + (float)System.Math.Cos(DegreeToRadian(bone.getAngle
                    ())) * 5;
                float yy = bone.getY() + (float)System.Math.Sin(DegreeToRadian(bone.getAngle
                    ())) * 5;
                float x2 = (float)System.Math.Cos(DegreeToRadian(bone.getAngle() + 90)) *
                    (SpriterCalculator.BONE_WIDTH / 2) * bone.getScaleY();
                float y2 = (float)System.Math.Sin(DegreeToRadian(bone.getAngle() + 90)) *
                    (SpriterCalculator.BONE_WIDTH / 2) * bone.getScaleY();
                float targetX = bone.getX() + (float)System.Math.Cos(DegreeToRadian(bone.getAngle
                    ())) * SpriterCalculator.BONE_LENGTH * bone.getScaleX();
                float targetY = bone.getY() + (float)System.Math.Sin(DegreeToRadian(bone.getAngle
                    ())) * SpriterCalculator.BONE_LENGTH * bone.getScaleX();
                float upperPointX = xx + x2;
                float upperPointY = yy + y2;
                this.drawLine(bone.getX(), bone.getY(), upperPointX, upperPointY);
                this.drawLine(upperPointX, upperPointY, targetX, targetY);
                float lowerPointX = xx - x2;
                float lowerPointY = yy - y2;
                this.drawLine(bone.getX(), bone.getY(), lowerPointX, lowerPointY);
                this.drawLine(lowerPointX, lowerPointY, targetX, targetY);
                this.drawLine(bone.getX(), bone.getY(), targetX, targetY);
			}
		}

		protected internal virtual void drawBoxes(Com.Brashmonkey.Spriter.player.SpriterAbstractPlayer
			 player)
		{
			this.setDrawColor(.25f, 1f, .25f, 1f);
			this.drawRectangle(player.getBoundingBox().left, player.getBoundingBox().bottom, 
				player.getBoundingBox().width, player.getBoundingBox().height);
			for (int j = 0; j < player.getObjectsToDraw(); j++)
			{
				Com.Brashmonkey.Spriter.SpriterPoint[] points = player.getRuntimeObjects()[j].getBoundingBox
					();
				this.drawLine(points[0].x, points[0].y, points[1].x, points[1].y);
				this.drawLine(points[1].x, points[1].y, points[3].x, points[3].y);
				this.drawLine(points[3].x, points[3].y, points[2].x, points[2].y);
				this.drawLine(points[2].x, points[2].y, points[0].x, points[0].y);
			}
		}

		protected internal abstract void setDrawColor(float r, float g, float b, float a);

		protected internal abstract void drawLine(float x1, float y1, float x2, float y2);

		protected internal abstract void drawRectangle(float x, float y, float width, float
			 height);

		/// <summary>Draws the given player with its sprites.</summary>
		/// <remarks>Draws the given player with its sprites.</remarks>
		/// <param name="player">Player to draw.</param>
		public virtual void draw(Com.Brashmonkey.Spriter.player.SpriterAbstractPlayer player
			)
		{
			Com.Brashmonkey.Spriter.draw.DrawInstruction[] instructions = player.getDrawInstructions
				();
			for (int i = 0; i < player.getObjectsToDraw(); i++)
			{
                instructions[i].setDepth(player.depth - ((float)i) * 0.00001f);
				if (instructions[i].obj.isVisible())
				{
					this.draw(instructions[i]);
				}
				foreach (Com.Brashmonkey.Spriter.player.SpriterAbstractPlayer pl in player.getAttachedPlayers
					())
				{
					if (player.getZIndex() == i)
					{
						draw(pl);
						pl.drawn = true;
					}
				}
			}
			foreach (Com.Brashmonkey.Spriter.player.SpriterAbstractPlayer pl_1 in player.getAttachedPlayers
				())
			{
				if (!player.drawn)
				{
					draw(pl_1);
				}
				player.drawn = false;
			}
		}

        protected float DegreeToRadian(float angle)
        {
            return SpriterCalculator.DegreeToRadian(angle);
        }
	}
}
