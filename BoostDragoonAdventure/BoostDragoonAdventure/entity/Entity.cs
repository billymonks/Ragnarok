using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.map;

namespace wickedcrush.entity
{
    public class Entity
    {
        #region Variables
        public Vector2 pos, velocity, accel, size, center;
        public Rectangle body;
        public Boolean solid; //can walk through?
        protected String name;

        public float friction = 0f; // 0 = no slide, 1 = no friction, more than 1 = acceleration

        protected List<LayerType> layerCollisionList;
        protected List<LayerType> predictedLayerCollisionList;

        protected List<Entity> entityCollisionList;

        protected List<Rectangle> rectangleCollisionList;
        #endregion

        #region Initialization
        public Entity(Vector2 pos, Vector2 size, Vector2 center, Boolean solid)
        {
            Initialize(pos, size, center, solid);
        }
        
        protected void Initialize(Vector2 pos, Vector2 size, Vector2 center, Boolean solid)
        {
            InitializeBody(pos, size, center);
            this.pos = pos;
            this.size = size;
            this.center = center;
            this.velocity = new Vector2(0f, 0f);
            this.accel = new Vector2(0f, 0f);
            this.solid = solid;

            this.name = "Entity";

            this.layerCollisionList = new List<LayerType>();
            this.predictedLayerCollisionList = new List<LayerType>();

            this.entityCollisionList = new List<Entity>();
            this.rectangleCollisionList = new List<Rectangle>();
        }

        protected void InitializeBody(Vector2 pos, Vector2 size, Vector2 center)
        {
            body = new Rectangle(
                (int)(pos.X - center.X),
                (int)(pos.Y - center.Y),
                (int)size.X,
                (int)size.Y);
        }
        #endregion

        #region Update
        public virtual void CheckCollisions(GameTime gameTime, Map m, List<Entity> entityList)
        {
            rectangleCollisionList.Clear();

            WallCollisions(m);
            foreach (Entity e in entityList)
            {
                if (!this.Equals(e))
                {
                    EntityCollision(e);
                }
            }
            foreach (Rectangle r in rectangleCollisionList)
                moveSelfOutOfRectangle(r);
        }

        protected Boolean EntityCollision(Entity e)
        {
            Boolean collisions = false;
            int turn = 1;
            if (CheckPredictedBodyCollisionX(e))
            {
                collisions = true;

                if (turn > 0)
                {
                    if (roundTowardZero(velocity.X) > 0)
                        velocity.X--;
                    else if (roundTowardZero(velocity.X) == 0)
                    {
                        accel.X = 0f;
                        if (roundTowardZero(e.velocity.X) == 0)
                        {
                            e.accel.X = 0f;
                        }
                    }
                    else
                        velocity.X++;
                }
                else
                {

                    if (roundTowardZero(e.velocity.X) > 0)
                        e.velocity.X--;
                    else if (roundTowardZero(e.velocity.X) == 0)
                    {
                        e.accel.X = 0f;
                        if (roundTowardZero(velocity.X) == 0)
                        {
                            accel.X = 0f;
                        }
                    }
                    else
                        e.velocity.X++;
                }
                turn *= -1;
            }
            if (CheckPredictedBodyCollisionY(e))
            {
                collisions = true;

                if (turn > 0)
                {
                    if (roundTowardZero(velocity.Y) > 0)
                        velocity.Y--;
                    else if (roundTowardZero(velocity.Y) == 0)
                    {
                        accel.Y = 0f;
                        if (roundTowardZero(e.velocity.Y) == 0)
                        {
                            e.accel.Y = 0f;
                            //break;
                        }
                    }
                    else
                        velocity.Y++;
                }
                else
                {
                    if (roundTowardZero(e.velocity.Y) > 0)
                        e.velocity.Y--;
                    else if (roundTowardZero(e.velocity.Y) == 0)
                    {
                        e.accel.Y = 0f;
                        if (roundTowardZero(velocity.Y) == 0)
                        {
                            accel.Y = 0f;
                            //break;
                        }
                    }
                    else
                        e.velocity.X++;
                }
                turn *= -1;
            }

            if (CheckPredictedBodyCollision(e))
            {
                collisions = true;

                velocity.X = 0f;
                velocity.Y = 0f;

                e.velocity.X = 0f;
                e.velocity.Y = 0f;

                e.accel.X = 0f;
                e.accel.Y = 0f;

            }

            if(collisions)
                rectangleCollisionList.Add(e.body);

            return collisions;
        }

        protected Boolean WallCollisions(Map m)
        {
            Boolean collisions = false;

            layerCollisionList.Clear();
            predictedLayerCollisionList.Clear();

            while (predictedLayerCollisionX(m, LayerType.WALL))
            {
                collisions = true;
                if (roundTowardZero(velocity.X) > 0)
                    velocity.X--;
                else if (roundTowardZero(velocity.X) == 0)
                {
                    accel.X = 0f;
                    break;
                }
                else
                    velocity.X++;

            }
            while (predictedLayerCollisionY(m, LayerType.WALL))
            {
                collisions = true;
                if (roundTowardZero(velocity.Y) > 0)
                    velocity.Y--;
                else if (roundTowardZero(velocity.Y) == 0)
                {
                    accel.Y = 0f;
                    break;
                }
                else
                    velocity.Y++;

            }
            if (predictedLayerCollision(m, LayerType.WALL))
            {
                collisions = true;

                velocity.X = 0f;
                velocity.Y = 0f;

                accel.X = 0f;
                accel.Y = 0f;
            }

            return collisions;
        }

        public virtual void Update(GameTime gameTime)
        {
            velocity += accel * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f) * 60.0f;
            pos += velocity * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f) * 60.0f;

            velocity *= friction;

            UpdateBody(pos);
        }

        public void UpdateBody(Vector2 pos)
        {
            body.X = (int)roundTowardZero(pos.X - center.X);
            body.Y = (int)roundTowardZero(pos.Y - center.Y);
        }
        #endregion

        #region CollisionChecks
        protected Boolean CheckBodyCollision(Entity otherObject)
        {
            return CheckRectangleCollision(otherObject.body);
        }

        protected Boolean CheckPredictedBodyCollision(Entity otherObject)
        {
            return CheckPredictedRectangleCollision(otherObject.body, otherObject.velocity);
        }

        protected Boolean CheckPredictedBodyCollisionX(Entity otherObject)
        {
            return CheckPredictedRectangleCollisionX(otherObject.body, otherObject.velocity);
        }

        protected Boolean CheckPredictedBodyCollisionY(Entity otherObject)
        {
            return CheckPredictedRectangleCollisionY(otherObject.body, otherObject.velocity);
        }

        protected Boolean CheckRectangleCollision(Rectangle r)
        {
            return (this.body.Intersects(r) || this.body.Contains(r));
        }

        protected Boolean CheckPredictedRectangleCollisionX(Rectangle r, Vector2 v)
        {
            Rectangle tempBody = new Rectangle(this.body.X + (int)roundTowardZero(velocity.X), this.body.Y, this.body.Width, this.body.Height);
            Rectangle tempR = new Rectangle(r.X + (int)v.X, r.Y, r.Width, r.Height);
            return (tempBody.Intersects(r) || tempBody.Contains(r));
        }
        protected Boolean CheckPredictedRectangleCollisionY(Rectangle r, Vector2 v)
        {
            Rectangle tempBody = new Rectangle(this.body.X, this.body.Y + (int)roundTowardZero(velocity.Y), this.body.Width, this.body.Height);
            Rectangle tempR = new Rectangle(r.X, r.Y + (int)v.Y, r.Width, r.Height);
            return (tempBody.Intersects(r) || tempBody.Contains(r));
        }
        protected Boolean CheckPredictedRectangleCollision(Rectangle r, Vector2 v)
        {
            Rectangle tempBody = new Rectangle(this.body.X + (int)roundTowardZero(velocity.X), this.body.Y + (int)roundTowardZero(velocity.Y), this.body.Width, this.body.Height);
            Rectangle tempR = new Rectangle(r.X + (int)roundTowardZero(v.X), r.Y + (int)roundTowardZero(v.Y), r.Width, r.Height);
            return (tempBody.Intersects(r) || tempBody.Contains(r));
        }

        protected Boolean CheckHotspotCollision(Rectangle r)
        {
            return r.Contains(new Point((int)roundTowardZero(center.X), (int)roundTowardZero(center.Y)));
        }

        protected Boolean layerCollision(Map m, LayerType l)
        {
            return m.layerCollision(body, l);
        }

        protected Boolean predictedLayerCollision(Map m, LayerType l)
        {
            return m.predictedLayerCollision(body, l, velocity);
        }

        protected Boolean predictedLayerCollisionX(Map m, LayerType l)
        {
            return m.predictedLayerCollision(body, l, new Vector2(velocity.X, 0f));
        }

        protected Boolean predictedLayerCollisionY(Map m, LayerType l)
        {
            return m.predictedLayerCollision(body, l, new Vector2(0f, velocity.Y));
        }

        #endregion

        public void DrawBody(GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            Texture2D whiteTexture = new Texture2D(gd, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            whiteTexture.SetData(data);

            spriteBatch.Draw(whiteTexture, body, c);
            spriteBatch.DrawString(f, name, pos, Color.Black);
        }

        public void moveSelfOutOfRectangle(Rectangle r)
        {
            if (!body.Intersects(r)) { return; }

            int smallest = smallestNumber(new int[] { bottomToTop(r), topToBottom(r), leftToRight(r), rightToLeft(r)});

            if (smallest == bottomToTop(r))
                body.Y -= (smallest+1);
            if (smallest == topToBottom(r))
                body.Y += smallest+1;
            if (smallest == leftToRight(r))
                body.X += smallest+1;
            if (smallest == rightToLeft(r))
                body.X -= (smallest+1);
        }

        private int bottomToTop(Rectangle r) // smallest? move up (-)
        {
            return Math.Abs(body.Bottom - r.Top);
        }

        private int topToBottom(Rectangle r) // smallest? move down (+)
        {
            return Math.Abs(body.Top - r.Bottom);
        }

        private int rightToLeft(Rectangle r) // smallest? move left (-)
        {
            return Math.Abs(body.Right - r.Left);
        }

        private int leftToRight(Rectangle r) // smallest? move right (+)
        {
            return Math.Abs(body.Left - r.Right);
        }

        private int smallestNumber(int[] list)
        {
            int num = list[0];
            for (int i = 1; i < list.Length; i++)
            {
                if (list[i] < num)
                    num = list[i];
            }
            return num;
        }

        protected float roundTowardZero(float f)
        {
            if (f < 0)
            {
                return (float)Math.Ceiling(f);
            }
            else
            {
                return (float)Math.Floor(f);
            }
        }
    }
        
}
