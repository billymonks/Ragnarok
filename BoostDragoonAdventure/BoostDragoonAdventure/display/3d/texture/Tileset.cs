using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.IO;

namespace wickedcrush.display._3d.texture
{
    public class Tileset
    {
        public String name, tex, normal;
        public Point size;
        public Rectangle[] tiles; // length = 3, represent width and height from top left to bottom right

        public Tileset(String path)
        {
            LoadXml("Content/img/xml/"+path+".xml");
        }

        public Rectangle GetTextureCoordinates(int i, int j)
        {
            return new Rectangle(tiles[i].X, tiles[j].Y, tiles[i].Width, tiles[j].Height);
        }

        private void LoadXml(String path)
        {
            XDocument doc;
            XElement rootElement;

            if (File.Exists(path))
            {
                doc = XDocument.Load(path);
                rootElement = new XElement(doc.Element("tileset"));

                name = rootElement.Attribute("name").Value;
                tex = "img/tex/" + rootElement.Element("tex").Value;
                normal = "img/tex/" + rootElement.Element("normal").Value;

                size = new Point(int.Parse(rootElement.Attribute("x").Value), 
                    int.Parse(rootElement.Attribute("y").Value));

                List<Rectangle> tempList = new List<Rectangle>();

                foreach(XElement e in rootElement.Element("tilesize").Elements("tilepos"))
                {
                    tempList.Add(new Rectangle(int.Parse(e.Attribute("x").Value), int.Parse(e.Attribute("y").Value), 
                        int.Parse(e.Attribute("width").Value), int.Parse(e.Attribute("height").Value)));
                }

                tiles = tempList.ToArray();

            }

        }
    }
}
