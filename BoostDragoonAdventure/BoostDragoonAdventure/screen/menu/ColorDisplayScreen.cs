using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using wickedcrush.player;
using wickedcrush.entity;
using Microsoft.Xna.Framework;
using wickedcrush.controls;
using Com.Brashmonkey.Spriter.player;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen.menu
{
    public class ColorDisplayScreen : GameScreen
    {
        protected GameplayManager _gm;
        protected Player p;

        Color color;

        protected float marginSize;

        public ColorDisplayScreen(Color color, GameBase game, GameplayManager gm, Player p)
        {
            this.game = game;
            _gm = gm;
            this.p = p;

            this.color = color;

            exclusiveUpdate = false;
            exclusiveDraw = false;

            Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);
            marginSize = (_gm.camera.fov - 1.3333333f) * 540f;
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, game.spriteScale);
            game.spriteBatch.Draw(game.whiteTexture, new Rectangle((int)(-marginSize), 0, (int)(1440f + marginSize * 2f), 1080), color);
            game.spriteBatch.End();
        }
    }
}
