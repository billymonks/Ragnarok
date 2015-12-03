using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Com.Brashmonkey.Spriter.xml
{
    public class XmlReader
    {
        private XmlDocument xml;
        public XmlReader()
        {
            xml = new XmlDocument();
        }

        public void parse(FileStream stream)
        {
            xml.Load(stream);
        }

        public void parse(String path)
        {
            this.parse(new FileStream(path, FileMode.Open));
        }

        public XmlNode getNode(string name)
        {
            return this.xml.GetElementsByTagName(name).Item(0);
        }

        public static XmlNode getChildByName(XmlNode node, string name)
        {
            List<XmlNode> children = getChildrenByName(node, name);
            if (children.Count == 0) return null;
            else return getChildrenByName(node, name).First();
        }

        public static List<XmlNode> getChildrenByName(XmlNode node, string name)
        {
            List<XmlNode> children = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.Equals(name))
                {
                    children.Add(child);
                }
            }

            return children;
        }

        public static string getAttribute(XmlNode node, string name)
        {
            return node.Attributes[name].Value;
        }

        public static bool getBool(XmlNode node, string name)
        {
            //if (node == null)
                //return false;

            try
            {
                return bool.Parse(getAttribute(node, name));
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static int getInt(XmlNode node, string name, int defaultValue)
        {
            try
            {
                return int.Parse(getAttribute(node, name));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static float getFloat(XmlNode node, string name, float defaultValue)
        {
            try
            {
                return float.Parse(getAttribute(node, name));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static int getInt(XmlNode node, string name)
        {
            return getInt(node, name, 0);
        }

        public static float getFloat(XmlNode node, string name)
        {
            return getFloat(node, name, 0f);
        }

    }
}
