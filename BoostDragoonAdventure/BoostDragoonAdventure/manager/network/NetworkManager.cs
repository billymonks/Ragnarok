using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;

namespace wickedcrush.manager.network
{
    public class NetworkManager
    {
        PeerListener listener;

        public NetworkManager()
        {
            listener = new PeerListener();

            listener.Connect();
        }

        public void RequestMap(int difficulty, int tier)
        {

        }
    }
}
