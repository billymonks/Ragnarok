using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wickedcrush.manager.network;
using System.IO;

namespace wickedcrush.manager.gameplay.room
{
    public static class RoomInfoList
    {
        public static List<RoomInfo> Deserialize(string data)
        {
            List<RoomInfo> roomInfoList = new List<RoomInfo>();

            XDocument doc = XDocument.Parse(data);

            XElement rootElement = new XElement(doc.Element("atlas"));

            foreach (XElement e in rootElement.Elements("room"))
            {
                RoomInfo temp = new RoomInfo(int.Parse(e.Attribute("globalId").Value), e.Attribute("localId").Value, e.Attribute("roomName").Value, e.Attribute("creatorName").Value);

                roomInfoList.Add(temp);
            }

            return roomInfoList;
        }

        public static string Serialize(List<RoomInfo> roomInfoList)
        {
            XDocument doc = new XDocument();

            XElement rootElement = new XElement("atlas");

            foreach (RoomInfo roomInfo in roomInfoList)
            {
                XElement map = new XElement("room");
                map.Add(new XAttribute("localId", roomInfo.localId));
                map.Add(new XAttribute("globalId", roomInfo.globalId));
                map.Add(new XAttribute("roomName", roomInfo.roomName));
                map.Add(new XAttribute("creatorName", roomInfo.creatorName));

                rootElement.Add(map);
            }

            doc.Add(rootElement);

            return doc.ToString();
        }
    }

    public class RoomInfo
    {
        //network
        public int globalId;
        public String roomName;
        public String localId;
        public String creatorName;

        public bool readyToLoad = false;
        public bool readyToAuthor = false;

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
        public List<RoomInfo> offlineAtlas;
        public List<RoomInfo> onlineAtlas;
        public Dictionary<String, RoomInfo> localAtlas;
        Random random;

        

        public RoomManager()
        {
            

            sessionStats = new Dictionary<String, RoomInfo>();
            offlineAtlas = new List<RoomInfo>();
            onlineAtlas = new List<RoomInfo>();
            localAtlas = new Dictionary<String, RoomInfo>();
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

        public void LoadOnlineAtlas(List<RoomInfo> atlas)
        {
            onlineAtlas.Clear();

            foreach (RoomInfo room in atlas)
                onlineAtlas.Add(room);
        }

        public void LoadLocalAtlas()
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

                    localAtlas.Add(e.Element("localId").Value, temp);
                }
            }
        }

        private void SaveLocalAtlas()
        {
            XDocument doc;
            XElement rootElement;
            String path = "Content/maps/atlas/LocalAtlas.xml";

            doc = new XDocument();
            rootElement = new XElement("atlas");

            foreach (KeyValuePair<String, RoomInfo> pair in localAtlas)
            {
                XElement room = new XElement("room");

                XElement globalId = new XElement("globalId");
                globalId.SetValue(pair.Value.globalId);
                room.Add(globalId);

                XElement localId = new XElement("localId");
                localId.SetValue(pair.Value.localId);
                room.Add(localId);

                XElement roomName = new XElement("roomName");
                roomName.SetValue(pair.Value.roomName);
                room.Add(roomName);

                XElement creatorName = new XElement("creatorName");
                creatorName.SetValue(pair.Value.creatorName);
                room.Add(creatorName);

                rootElement.Add(room);
            }

            doc.Add(rootElement);

            doc.Save(path);
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
            LoadLocalAtlas();

            if (localAtlas.ContainsKey(room.localId))
            {
                localAtlas.Remove(room.localId);
            }

            localAtlas.Add(room.localId, room);

            SaveLocalAtlas();
        }

        public RoomInfo GetRoomFromLocalAtlas(String localId)
        {
            LoadLocalAtlas();
            if (!localAtlas.ContainsKey(localId))
                throw new Exception("Whahahhahahah");

            return localAtlas[localId];
        }

        

        public void SendOfflineAtlas(NetworkManager networkManager)
        {
            foreach (RoomInfo stats in offlineAtlas)
            {
                networkManager.SendMap(stats.roomName, XDocument.Load(stats.localId), stats.localId, 11);
                return;
            }
        }

        public void AssignGlobalId(String localId, int globalId)
        {
            if (!localAtlas.ContainsKey(localId))
            {
                Console.WriteLine("LocalId: " + localId + " does not exist, cannot assign! What have you done?!");
                return;
            }

            RoomInfo temp = localAtlas[localId];
            temp.globalId = globalId;
            localAtlas[localId] = temp;

            SaveLocalAtlas();
        }

        public String getGameplayRoom(RoomInfo room)
        {
            return "Content/maps/small/" + room.localId + ".xml";
        }

        public String getRandomOfflineRoom()
        {
            return offlineAtlas[random.Next(offlineAtlas.Count - 1)].localId;
        }

        public String getRandomOnlineRoom()
        {
            RoomInfo info = onlineAtlas[random.Next(0, onlineAtlas.Count)];
            return "Content/maps/temp/" + info.globalId + "_" + info.localId + ".xml";
        }
    }
}
