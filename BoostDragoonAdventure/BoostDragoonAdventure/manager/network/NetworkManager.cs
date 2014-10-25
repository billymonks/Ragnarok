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

        public NetworkManager(Game game)
        {
            listener = new PeerListener();

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
        }

        private bool OkToSend()
        {
            return (listener.connected);
        }

        private bool MapsReady()
        {
            return (mapsToSend.Count > 0);
        }

        private void SendQueuedMaps()
        {
            foreach (KeyValuePair<String, XDocument> pair in mapsToSend)
            {
                listener.SendMap(pair.Key, pair.Value);
            }

            mapsToSend.Clear();
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

        public void RequestUserId(String localId, String name)
        {

        }
    }
}
