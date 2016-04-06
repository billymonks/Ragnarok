using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace wickedcrush.display._3d.atlas
{
    public struct AtlasNode
    {
        public String path;
        public Rectangle rectangle;

        public AtlasNode(String path, Rectangle rectangle)
        {
            this.path = path;
            this.rectangle = rectangle;
        }
    }
    public class TextureAtlas
    {
        public Texture2D texture;
        public Dictionary<String, Rectangle> atlas = new Dictionary<String, Rectangle>();
        public Rectangle bounding = new Rectangle(0, 0, 64, 64);

        public TextureAtlas(Dictionary<String, Texture2D> textures, GraphicsDevice gd)
        {
            RenderTarget2D renderTarget;
            Texture2D compiledTexture;
            SpriteBatch batch = new SpriteBatch(gd);
            List<AtlasNode> atlasList = new List<AtlasNode>();

            int width, height;
            Color[] cData;

            foreach (KeyValuePair<String, Texture2D> tex in textures)
            {
                atlasList.Add(new AtlasNode(tex.Key, new Rectangle(0, 0, tex.Value.Width, tex.Value.Height)));
            }

            CalculateAtlas(atlasList);

            renderTarget = new RenderTarget2D(
                gd,
                bounding.Width,
                bounding.Height,
                true,
                gd.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            gd.SetRenderTarget(renderTarget);
            gd.Clear(Color.Transparent);

            batch.Begin();

            foreach (KeyValuePair<String, Rectangle> pair in atlas)
            {
                batch.Draw(textures[pair.Key], pair.Value, Color.White);
            }

            batch.End();

            gd.SetRenderTarget(null);

            compiledTexture = new Texture2D(gd,
                renderTarget.Width, renderTarget.Height, true,
                renderTarget.Format);

            for (int i = 0; i < renderTarget.LevelCount; i++)
            {
                width = (int)Math.Max((renderTarget.Width / Math.Pow(2, i)), 1);
                height = (int)Math.Max((renderTarget.Width / Math.Pow(2, i)), 1);
                cData = new Color[width * height];

                renderTarget.GetData<Color>(i, null, cData, 0, cData.Length);
                compiledTexture.SetData<Color>(i, null, cData, 0, cData.Length);
            }


            texture = compiledTexture;

            //DateTime date = DateTime.Now; //Get the date for the file name
            //Stream stream = File.Create("texture_atlas" + date.ToString("MM-dd-yy H;mm;ss") + ".png"); 

            //renderTarget.SaveAsPng(stream, bounding.Width, bounding.Height);
        }

        public Vector2 GetConvertedCoordinate(String path, Vector2 oldCoordinate)
        {
            Rectangle r = atlas[path];

            return new Vector2((((float)r.X + oldCoordinate.X * (float)r.Width) / (float)bounding.Width),
                (((float)r.Y + oldCoordinate.Y * (float)r.Height) / (float)bounding.Height));
        }

        public Vector2 GetConvertedTileCoord(String path, Vector2 oldCoordinate)
        {
            Rectangle r = atlas[path];

            return new Vector2((((float)r.X + oldCoordinate.X) / (float)bounding.Width),
                (((float)r.Y + oldCoordinate.Y) / (float)bounding.Height));
        }

        private void CalculateAtlas(List<AtlasNode> atlasList)
        {
            LinkedList<Point> texturePlacements = new LinkedList<Point>( new List<Point>() {new Point(0, 0)} );
            
            atlasList.Sort((a1, a2) => a2.rectangle.Height.CompareTo(a1.rectangle.Height));

            

            foreach (AtlasNode a in atlasList)
            {
                while (!PlaceAtlasNode(a, texturePlacements))
                {
                    IncreaseBoundingSize();
                }
            }
        }

        private bool PlaceAtlasNode(AtlasNode node, LinkedList<Point> texturePlacements)
        {
            LinkedListNode<Point> curr = texturePlacements.First;

            while (curr != null)
            {
                node.rectangle.X = curr.Value.X;
                node.rectangle.Y = curr.Value.Y;

                if (RectangleFits(node.rectangle))
                {
                    atlas.Add(node.path, node.rectangle);
                    texturePlacements.AddAfter(curr, new Point(node.rectangle.Left, node.rectangle.Bottom));
                    texturePlacements.AddAfter(curr, new Point(node.rectangle.Right, node.rectangle.Top));
                    texturePlacements.Remove(curr);
                    return true;
                }

                curr = curr.Next;
            }

            return false;
        }

        private bool RectangleFits(Rectangle r)
        {
            foreach (KeyValuePair<String, Rectangle> pair in atlas)
            {
                if (pair.Value.Intersects(r))
                    return false;
            }

            if (!bounding.Contains(r))
                return false;

            return true;
        }

        private void IncreaseBoundingSize()
        {
            bounding.Width = bounding.Width * 2;
            bounding.Height = bounding.Height * 2;
        }
    }
}
