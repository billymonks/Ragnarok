﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace wickedcrush.manager.map.room
{
    public struct RoomStats
    {
        public String filename;
        public int attempts, completions, damage;
        public float difficulty;

        public RoomStats(String filename)
        {
            this.filename = filename;
            attempts = 0;
            completions = 0;
            damage = 0;
            difficulty = 0;
        }
    }

    public class RoomManager
    {
        Dictionary<String, RoomStats> sessionStats;
        List<RoomStats> offlineAtlas;

        public RoomManager()
        {
            sessionStats = new Dictionary<String, RoomStats>();
            offlineAtlas = new List<RoomStats>();

            LoadOfflineAtlas();
        }

        public void LoadOfflineAtlas()
        {
            offlineAtlas.Clear();

            String path = "Content/maps/atlas/OfflineAtlas.xml";
            XDocument doc = XDocument.Load(path);
            XElement rootElement = new XElement(doc.Element("atlas"));

            foreach (XElement e in rootElement.Elements("room"))
            {
                RoomStats temp = new RoomStats(e.Element("filename").Value);

                temp.attempts = int.Parse(e.Element("attempts").Value);
                temp.completions = int.Parse(e.Element("completions").Value);
                temp.damage = int.Parse(e.Element("damage").Value);
                temp.difficulty = 0f;

                offlineAtlas.Add(temp);
            }
        }
    }
}