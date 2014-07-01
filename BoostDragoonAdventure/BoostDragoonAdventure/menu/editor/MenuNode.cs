using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display.sprite;

namespace wickedcrush.menu.editor
{
    public abstract class MenuNode
    {
        public TextSprite text;
        public TextureSprite image;
        public MenuNode parent, next, prev;
        public EditorMenu root;
        
        private Rectangle hitBox;

        public MenuNode(TextSprite text, TextureSprite image, Nullable<Rectangle> hitBox, EditorMenu root)
        {
            this.text = text;
            this.image = image;
            if (hitBox != null)
                this.hitBox = hitBox.Value;
            this.root = root;
        }

        public void Update(GameTime gameTime)
        {
            text.Update(gameTime);
            image.Update(gameTime);
        }
    }
}
