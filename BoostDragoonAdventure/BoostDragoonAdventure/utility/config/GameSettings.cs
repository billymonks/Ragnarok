using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace wickedcrush.utility.config
{
    public class GameSettings
    {
        public Point resolution;
        public bool fullscreen;

        public GameSettings()
        {
            if (File.Exists("settings.xml"))
                LoadSettings();
            else
            {
                SetDefaultSettings();
            }
        }

        private void LoadSettings()
        {
            XDocument doc = XDocument.Load("settings.xml");
            resolution = new Point(int.Parse(doc.Element("settings").Element("resolution").Attribute("x").Value), int.Parse(doc.Element("settings").Element("resolution").Attribute("y").Value));
            fullscreen = bool.Parse(doc.Element("settings").Element("resolution").Attribute("fullscreen").Value);
        }

        private void SetDefaultSettings()
        {
            resolution = new Point(1280, 720);
            fullscreen = false;

            XDocument doc = new XDocument();
            XElement root = new XElement("settings");
            XElement resNode = new XElement("resolution");
            resNode.Add(new XAttribute("x", resolution.X));
            resNode.Add(new XAttribute("y", resolution.Y));
            resNode.Add(new XAttribute("fullscreen", fullscreen));
            root.Add(resNode);
            doc.Add(root);

            doc.Save("settings.xml");
        }
    }
}
