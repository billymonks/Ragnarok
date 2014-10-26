using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;
using System.Xml.Linq;

namespace wickedcrush.manager.network
{
    public class NetworkManager
    {
        public bool enabled = true;

        PeerListener listener;

        Dictionary<String, XDocument> mapsToSend = new Dictionary<String, XDocument>();
        Dictionary<String, String> charactersToSend = new Dictionary<String, String>();

        public NetworkManager(Game game)
        {
            listener = new PeerListener(game);

            if (enabled)
            {
                listener.Connect();
            }
        }

        public void Update()
        {
            if (!enabled)
            {
                return;
            }

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
            foreach (KeyValuePair<String, XDocument> pair in mapsToSend)
            {
                listener.SendMap(pair.Key, pair.Value);
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

        public void SendMap(String name, XDocument map)
        {
            if (mapsToSend.ContainsKey(name))
                return;

            mapsToSend.Add(name, map);
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
