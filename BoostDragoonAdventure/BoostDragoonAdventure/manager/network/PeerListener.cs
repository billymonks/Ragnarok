using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;

namespace wickedcrush.manager.network
{
    

    public class PeerListener : IPhotonPeerListener
    {
        private int retries = 0, maxRetries = 5;
        
        private string serverAddress = "54.172.128.30:4530", serverName = "WorldServer";

        bool connected = false;

        PhotonPeer peer;

        public PeerListener()
        {
            peer = new PhotonPeer(this, ConnectionProtocol.Tcp);
        }

        

        public void  DebugReturn(DebugLevel level, string message)
        {
 	        throw new NotImplementedException();
        }

        public void  OnEvent(EventData eventData)
        {
 	        throw new NotImplementedException();
        }

        public void  OnOperationResponse(OperationResponse operationResponse)
        {
 	        throw new NotImplementedException();
        }

        public void  OnStatusChanged(StatusCode statusCode)
        {
 	        throw new NotImplementedException();
        }

        public void Connect()
        {
            do
            {
                connected = peer.Connect(serverAddress, serverName);
                retries++;
            } while (!connected && retries < maxRetries);
        }
    }
}
