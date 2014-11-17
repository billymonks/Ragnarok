using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;
using System.Xml.Linq;
using wickedcrush.utility;
using Microsoft.Xna.Framework;

namespace wickedcrush.manager.network
{
    public struct AuthoredMap
    {
        public String mapName;
        public XDocument map;
        public String localId;
        public int globalCharId;

        public AuthoredMap(String mapName, XDocument map, String localId, int globalCharId)
        {
            this.mapName = mapName;
            this.map = map;
            this.localId = localId;
            this.globalCharId = globalCharId;
        }
    }
    public class NetworkManager
    {
        public bool enabled = true;

        PeerListener listener;

        Dictionary<String, AuthoredMap> mapsToSend = new Dictionary<String, AuthoredMap>();
        Dictionary<String, String> charactersToSend = new Dictionary<String, String>();

        public Dictionary<String, Timer> timers;

        public bool mapsRequested = false;

        public NetworkManager(Game game)
        {
            listener = new PeerListener(game);
            timers = new Dictionary<String, Timer>();

            if (enabled)
            {
                listener.Connect();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!enabled)
            {
                return;
            }

            UpdateTimers(gameTime);

            listener.Update();

            if (MapsReady())
            {
                if (listener.connected)
                    SendQueuedMaps();
            }

            if (CharactersReady())
            {
                if (listener.connected)
                    SendQueuedCharacters();
            }

            if (!mapsRequested && listener.connected)
            {
                RequestOnlineMaps(1);
                mapsRequested = true;
            }
        }

        private void UpdateTimers(GameTime gameTime)
        {
            foreach (KeyValuePair<String, Timer> pair in timers)
            {
                pair.Value.Update(gameTime);
            }
        }

        public void Disconnect()
        {
            listener.Disconnect();
            enabled = false;
        }

        public void RequestOnlineMaps(int count)
        {
            listener.RequestOnlineRooms(count);
        }

        private bool OkToSend()
        {
            return (listener.connected);
        }

        private bool MapsReady()
        {
            return (mapsToSend.Count > 0);
        }

        private bool CharactersReady()
        {
            return (charactersToSend.Count > 0);
        }

        private void SendQueuedMaps()
        {
            foreach (KeyValuePair<String, AuthoredMap> pair in mapsToSend)
            {
                listener.SendRoom(pair.Value);
            }

            mapsToSend.Clear();
        }

        private void SendQueuedCharacters()
        {
            foreach (KeyValuePair<String, String> pair in charactersToSend)
            {
                listener.AssignCharacter(pair.Value, pair.Key);
            }

            charactersToSend.Clear();
        }

        public void SendMap(String mapName, XDocument map, String localId, int globalCharId)
        {
            if (mapsToSend.ContainsKey(localId))
                return;

            mapsToSend.Add(localId, new AuthoredMap(mapName, map, localId, globalCharId));
        }

        public void RequestMap(int difficulty, int tier)
        {

        }

        public void AssignCharacter(String name, String localId)
        {
            if (charactersToSend.ContainsKey(localId))
                return;

            charactersToSend.Add(localId, name);
        }
    }
}
