using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wickedcrush.manager.network;
using System.IO;

namespace wickedcrush.manager.map.room
{
    public struct RoomStats
    {
        //network
        public int globalId;
        public String roomName;
        public String localId;
        public String creatorName;

        public RoomStats(String localId)
        {
            globalId = -1;
            this.roomName = "nameless room, so sad, so sad";
            this.localId = localId;
            creatorName = "captain no-name";
        }

        public RoomStats(int globalId, String localId, String roomName, String creatorName)
        {
            this.globalId = globalId;
            this.roomName = roomName;
            this.localId = localId;
            this.creatorName = creatorName;
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

            CreateLocalAtlas();
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
                RoomStats temp = new RoomStats(int.Parse(e.Element("globalId").Value), e.Element("localId").Value, e.Element("roomName").Value, e.Element("creatorName").Value);

                offlineAtlas.Add(temp);
            }
        }

        private void CreateLocalAtlas()
        {
            if (File.Exists("Content/maps/atlas/LocalAtlas.xml"))
                return;

            XDocument doc = new XDocument();
            XElement rootElement = new XElement("atlas");
            doc.Add(rootElement);
            doc.Save("Content/maps/atlas/LocalAtlas.xml");
        }

        public void AddRoomToAtlas(RoomStats room, String atlasPath) //next level future shit
        {
            String path = atlasPath;
            XDocument doc = XDocument.Load(path);
            XElement rootElement = new XElement(doc.Element("atlas"));

            XElement roomElement = new XElement("room");

            XElement globalIdElement = new XElement("globalId");
            globalIdElement.SetValue(room.globalId);

            XElement localIdElement = new XElement("localId");
            localIdElement.SetValue(room.localId);

            XElement roomNameElement = new XElement("roomName");
            roomNameElement.SetValue(room.roomName);

            XElement creatorNameElement = new XElement("creatorName");
            creatorNameElement.SetValue(room.creatorName);

            roomElement.Add(globalIdElement);
            roomElement.Add(localIdElement);
            roomElement.Add(roomNameElement);
            roomElement.Add(creatorNameElement);

            rootElement.Add(roomElement);
            doc = new XDocument();
            doc.Add(rootElement);
            doc.Save(path);
        }

        public void AddRoomToLocalAtlas(RoomStats room)
        {
            String path = "Content/maps/atlas/LocalAtlas.xml";
            XDocument doc = XDocument.Load(path);
            XElement rootElement = new XElement(doc.Element("atlas"));

            XElement roomElement = new XElement("room");

            XElement globalIdElement = new XElement("globalId");
            globalIdElement.SetValue(room.globalId);

            XElement localIdElement = new XElement("localId");
            localIdElement.SetValue(room.localId);

            XElement roomNameElement = new XElement("roomName");
            roomNameElement.SetValue(room.roomName);

            XElement creatorNameElement = new XElement("creatorName");
            creatorNameElement.SetValue(room.creatorName);

            roomElement.Add(globalIdElement);
            roomElement.Add(localIdElement);
            roomElement.Add(roomNameElement);
            roomElement.Add(creatorNameElement);

            rootElement.Add(roomElement);
            doc = new XDocument();
            doc.Add(rootElement);
            doc.Save(path);
        }

        public void SendOfflineAtlas(NetworkManager networkManager)
        {
            foreach (RoomStats stats in offlineAtlas)
            {
                networkManager.SendMap(stats.roomName, XDocument.Load(stats.localId), stats.localId, 11);
                return;
            }
        }

        public String getRandomRoom()
        {
            return offlineAtlas[random.Next(offlineAtlas.Count - 1)].localId;
        }
    }
}
