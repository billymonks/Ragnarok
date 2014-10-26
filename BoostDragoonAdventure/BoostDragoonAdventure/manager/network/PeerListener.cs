using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;
using System.Xml.Linq;

namespace wickedcrush.manager.network
{
    public enum OpCode
    {
        MapFromServer = 1,
        MapToServer = 2,
        AssignMapId = 3,
        AssignCharId = 4
    }

    public class PeerListener : IPhotonPeerListener
    {
        private int retries = 0, maxRetries = 5;
        
        //private string serverAddress = "54.172.128.30:4530", serverName = "WorldServer";
        private string serverAddress = "localhost:4530", serverName = "WorldServer";

        public bool connected = false;

        public PhotonPeer peer;

        private Game _g;

        public PeerListener(Game g)
        {
            peer = new PhotonPeer(this, ConnectionProtocol.Tcp);
            this._g = g;
        }

        public void Update()
        {
            if (peer.PeerState.Equals(PeerStateValue.Connected))
                connected = true;
            else
                connected = false;

            peer.Service();
        }

        public void  DebugReturn(DebugLevel level, string message)
        {
            Console.WriteLine(level + ": " + message);
        }

        public void  OnEvent(EventData eventData)
        {
            Console.WriteLine("Event: " + eventData.Code);
            if (eventData.Code == 1)
            {
                Console.WriteLine("Chat: " + eventData.Parameters[1]);
            }
        }

        public void  OnOperationResponse(OperationResponse operationResponse)
        {
            Console.WriteLine("Response: " + operationResponse.OperationCode);
            OperationResponse(operationResponse);
        }

        public void OperationResponse(OperationResponse operationResponse)
        {
            // handle response by code (action we called)
            /*switch (operationResponse.OperationCode)
            {
                // out custom "hello world" operation's code is 1
                case 1:
                    // OK
                    if (operationResponse.ReturnCode == 0)
                    {
                        // show the complete content of the response
                        Console.WriteLine(operationResponse.ToStringFull());
                    }
                    else
                    {
                        // show the error message
                        Console.WriteLine(operationResponse.DebugMessage);
                    }
                    break;
            }*/

            if (operationResponse.ReturnCode == 1)
            {
                Console.WriteLine(operationResponse.ToStringFull());
                return;
            }

            switch (operationResponse.OperationCode)
            {
                case (byte)OpCode.AssignCharId: //AssignCharId
                    _g.playerManager.AssignGlobalId((int)operationResponse.Parameters[102], (string)operationResponse.Parameters[101]);

                    break;

            }

            Console.WriteLine(operationResponse.ToStringFull());
        }

        public void  OnStatusChanged(StatusCode statusCode)
        {
            Console.WriteLine(statusCode.ToString());
        }

        public void Connect()
        {
            do
            {
                connected = peer.Connect(serverAddress, serverName);
                retries++;
            } while (!connected && retries < maxRetries);
        }

        public void Disconnect()
        {
            peer.Disconnect();
            connected = false;
        }

        public void SendMap(String name, XDocument map)
        {
            var parameters = new Dictionary<byte, object>();
            parameters[(byte)100] = name;
            parameters[(byte)101] = map.ToString();
            peer.OpCustom((byte)OpCode.MapToServer, parameters, true);
            Console.WriteLine("Map sent: " + name);
        }

        public void AssignCharacter(String name, String localId)
        {
            var parameters = new Dictionary<byte, object>();
            parameters[(byte)100] = name;
            parameters[(byte)101] = localId;
            peer.OpCustom((byte)OpCode.AssignCharId, parameters, true);
            Console.WriteLine("Character sent: " + name);
        }
    }
}
