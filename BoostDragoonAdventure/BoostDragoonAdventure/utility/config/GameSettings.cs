using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace wickedcrush.utility.config
{
    public enum ControlMode
    {
        KeyboardOnly = 1,
        MouseAndKeyboard = 2,
        Gamepad = 3
    }
    public class GameSettings
    {
        public Point resolution;
        public bool fullscreen;
        public ControlMode controlMode = ControlMode.MouseAndKeyboard;
        public float mouseSensitivity = 1.5f;
        public bool hqLight = false;

        public GameSettings()
        {
            String path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Wicked Crush\\settings.xml");
            if (File.Exists(path))
                LoadSettings();
            else
            {
                SetDefaultSettings();
            }
        }

        private void LoadSettings()
        {
            XDocument doc = XDocument.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Wicked Crush\\settings.xml"));
            
            if (doc.Element("settings").Elements("resolution").Count<XElement>() > 0)
            {
                resolution = new Point(int.Parse(doc.Element("settings").Element("resolution").Attribute("x").Value), int.Parse(doc.Element("settings").Element("resolution").Attribute("y").Value));
                fullscreen = bool.Parse(doc.Element("settings").Element("resolution").Attribute("fullscreen").Value);
            }
            else
            {
                resolution = new Point(1280, 720);
                fullscreen = false;
            }

            if (doc.Element("settings").Elements("light").Count<XElement>() > 0)
            {
                hqLight = bool.Parse(doc.Element("settings").Element("light").Value);
            }
            else
            {
                hqLight = false;
            }
            
            if (doc.Element("settings").Elements("controlMode").Count<XElement>() > 0)
            {
                controlMode = (ControlMode)int.Parse(doc.Element("settings").Element("controlMode").Value);
            }
            else
            {
                controlMode = ControlMode.MouseAndKeyboard;
            }

            if (doc.Element("settings").Elements("mouseSensitivity").Count<XElement>() > 0)
            {
                mouseSensitivity = float.Parse(doc.Element("settings").Element("mouseSensitivity").Value);
            }
            else
            {
                mouseSensitivity = 1.5f;
            }

            SaveSettings();
        }

        private void SetDefaultSettings()
        {
            resolution = new Point(1280, 720);
            fullscreen = false;
            controlMode = ControlMode.MouseAndKeyboard;
            hqLight = true;

            SaveSettings();
        }

        private void SaveSettings()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("settings");
            XElement resNode = new XElement("resolution");
            XElement lightNode = new XElement("light");
            lightNode.SetValue(hqLight);

            resNode.Add(new XAttribute("x", resolution.X));
            resNode.Add(new XAttribute("y", resolution.Y));
            resNode.Add(new XAttribute("fullscreen", fullscreen));
            root.Add(new XElement("controlMode", (int)controlMode));
            root.Add(new XElement("mouseSensitivity", (float)mouseSensitivity));
            root.Add(resNode);
            root.Add(lightNode);
            doc.Add(root);

            if (!System.IO.Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Wicked Crush")))
                System.IO.Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Wicked Crush"));
            doc.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Wicked Crush\\settings.xml"));
        }
    }
}
