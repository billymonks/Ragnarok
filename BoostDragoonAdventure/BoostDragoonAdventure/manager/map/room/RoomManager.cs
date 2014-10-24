using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wickedcrush.manager.network;

namespace wickedcrush.manager.map.room
{
    public struct RoomStats
    {
        //network
        public int roomId;

        public String name;
        public String filename;
        public int attempts, completions, damage;
        public float difficulty;

        public String creator;

        public RoomStats(String filename)
        {
            roomId = -1;
            this.name = "nameless room, so sad, so sad";
            this.filename = filename;
            attempts = 0;
            completions = 0;
            damage = 0;
            difficulty = 0;
            creator = "captain no-name";
        }

        public RoomStats(String filename, String name)
        {
            roomId = -1;
            this.name = name;
            this.filename = filename;
            attempts = 0;
            completions = 0;
            damage = 0;
            difficulty = 0;
            creator = "captain no-name";
        }
    }

    public class RoomManager
    {
        Dictionary<String, RoomStats> sessionStats;
        List<RoomStats> offlineAtlas;
        Random random;

        public RoomManager()
        {
            

            sessionStats = new Dictionary<String, RoomStats>();
            offlineAtlas = new List<RoomStats>();
            random = new Random();

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
                RoomStats temp = new RoomStats(e.Element("filename").Value, e.Element("filename").Value);

                temp.attempts = int.Parse(e.Element("attempts").Value);
                temp.completions = int.Parse(e.Element("completions").Value);
                temp.damage = int.Parse(e.Element("damage").Value);
                temp.difficulty = 0f;

                offlineAtlas.Add(temp);
            }
        }

        public void SendOfflineAtlas(NetworkManager networkManager)
        {
            foreach (RoomStats stats in offlineAtlas)
            {
                networkManager.SendMap(stats.name, XDocument.Load(stats.filename));
            }
        }

        public String getRandomRoom()
        {
            return offlineAtlas[random.Next(offlineAtlas.Count - 1)].filename;
        }
    }
}
