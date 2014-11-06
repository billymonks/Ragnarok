using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wickedcrush.manager.network;
using System.IO;

namespace wickedcrush.manager.map.room
{
    public struct RoomInfo
    {
        //network
        public int globalId;
        public String roomName;
        public String localId;
        public String creatorName;

        public RoomInfo(String localId)
        {
            globalId = -1;
            this.roomName = "nameless room, so sad, so sad";
            this.localId = localId;
            creatorName = "captain no-name";
        }

        public RoomInfo(int globalId, String localId, String roomName, String creatorName)
        {
            this.globalId = globalId;
            this.roomName = roomName;
            this.localId = localId;
            this.creatorName = creatorName;
        }
    }

    public class RoomManager
    {
        Dictionary<String, RoomInfo> sessionStats;
        List<RoomInfo> offlineAtlas;
        List<RoomInfo> localAtlas;
        Random random;

        

        public RoomManager()
        {
            

            sessionStats = new Dictionary<String, RoomInfo>();
            offlineAtlas = new List<RoomInfo>();
            localAtlas = new List<RoomInfo>();
            random = new Random();

            LoadLocalAtlas();
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
                RoomInfo temp = new RoomInfo(int.Parse(e.Element("globalId").Value), e.Element("localId").Value, e.Element("roomName").Value, e.Element("creatorName").Value);

                offlineAtlas.Add(temp);
            }
        }

        private void LoadLocalAtlas()
        {
            XDocument doc;
            XElement rootElement;
            String path = "Content/maps/atlas/LocalAtlas.xml";

            localAtlas.Clear();

            

            if (File.Exists(path))
            {
                doc = XDocument.Load(path);
                rootElement = new XElement(doc.Element("atlas"));

                foreach (XElement e in rootElement.Elements("room"))
                {
                    RoomInfo temp = new RoomInfo(int.Parse(e.Element("globalId").Value), e.Element("localId").Value, e.Element("roomName").Value, e.Element("creatorName").Value);

                    localAtlas.Add(temp);
                }
            }
        }

        public void AddRoomToAtlas(RoomInfo room, String atlasPath) //next level future shit
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

        public void AddRoomToLocalAtlas(RoomInfo room)
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

            localAtlas.Add(room);
        }

        public void SendOfflineAtlas(NetworkManager networkManager)
        {
            foreach (RoomInfo stats in offlineAtlas)
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
