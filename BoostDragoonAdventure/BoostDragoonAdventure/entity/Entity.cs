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
        public Vector2 pos, size, center;
        protected String name;
        #endregion

        #region Initialization
        public Entity(Vector2 pos, Vector2 size, Vector2 center)
        {
            Initialize(pos, size, center);
        }
        
        protected void Initialize(Vector2 pos, Vector2 size, Vector2 center)
        {
            this.pos = pos;
            this.size = size;
            this.center = center;

            this.name = "Entity";
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            
        }
        #endregion

        #region CollisionChecks
        
        #endregion

        public virtual void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            //spriteBatch.Draw(whiteTexture, body, c);
            spriteBatch.Draw(tex, new Rectangle((int)pos.X - 1, (int)pos.Y - 1, 2, 2), c); 
            spriteBatch.DrawString(f, name, pos, Color.Black);
        }

        
    }
        
}
