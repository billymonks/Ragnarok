using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Brashmonkey.Spriter.file;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.display.spriter
{
    public class SpriterLoader : FileLoader
    {
        private GameBase game;

        public SpriterLoader(GameBase game)
        {
            this.game = game;
        }

        public override void load(Reference reference, string path)
        {
            try
            {
                FileStream filestream = new FileStream(path, FileMode.Open);
                Texture2D sprite = Texture2D.FromStream(game.GraphicsDevice, filestream);
                files.Add(reference, sprite);
                filestream.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void dispose()
        {
            foreach (KeyValuePair<Reference, object> pair in files)
                ((Texture2D)pair.Value).Dispose();
        }
    }
}
