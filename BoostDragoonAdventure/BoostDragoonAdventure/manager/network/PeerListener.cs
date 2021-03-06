﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;
using System.Xml.Linq;
using wickedcrush.manager.gameplay.room;
using System.IO;

namespace wickedcrush.manager.network
{
    public enum OpCode
    {
        MapFromServer = (byte)1,
        MapToServer = (byte)2,
        AssignMapId = (byte)3,
        AssignCharId = (byte)4
    }

    public class PeerListener : IPhotonPeerListener
    {
        private int retries = 0, maxRetries = 5;
        
        //private string serverAddress = "54.172.128.30:4530", serverName = "WorldServer";
        private string serverAddress = "localhost:4530", serverName = "WorldServer";

        public bool connected = false;

        public PhotonPeer peer;

        private GameBase _g;

        public PeerListener(GameBase g)
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

            if (operationResponse.ReturnCode == 1)
            {
                Console.WriteLine(operationResponse.ToStringFull());
                return;
            }

            switch (operationResponse.OperationCode)
            {
                case (byte)OpCode.AssignCharId: //AssignCharId
                    _g.playerManager.AssignGlobalId((string)operationResponse.Parameters[101], (int)operationResponse.Parameters[102]);

                    break;

                case (byte)OpCode.MapToServer: //maybe use this to mark jobs as completed 
                    //assign global id to room matching local id
                    _g.roomManager.AssignGlobalId((string)operationResponse.Parameters[101], (int)operationResponse.Parameters[102]);
                    break;

                case (byte)OpCode.MapFromServer:
                    if (!Directory.Exists("Content/maps/temp/"))
                        Directory.CreateDirectory("Content/maps/temp");

                    List<RoomInfo> atlas = RoomInfoList.Deserialize((string)operationResponse.Parameters[100]);
                    _g.roomManager.LoadOnlineAtlas(atlas);

                    for (int i = 0; i < atlas.Count; i++)
                    {
                        File.WriteAllText("Content/maps/temp/" + atlas[i].globalId.ToString() + "_" + atlas[i].localId + ".xml", operationResponse.Parameters[(byte)(101 + i)].ToString());
                    }

                    break;

            }

            Console.WriteLine(operationResponse.ToString());
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

        public void RequestOnlineRooms(int count)
        {
            var parameters = new Dictionary<byte, object>();
            parameters[(byte)100] = count;
            peer.OpCustom((byte)OpCode.MapFromServer, parameters, true);
            Console.WriteLine("Requesting " + count + " room(s).");
        }

        public void SendRoom(AuthoredMap authoredMap)
        {
            var parameters = new Dictionary<byte, object>();
            parameters[(byte)100] = authoredMap.mapName; //map name
            parameters[(byte)101] = authoredMap.map.ToString(); //map data
            parameters[(byte)102] = authoredMap.localId;
            parameters[(byte)103] = authoredMap.globalCharId;
            peer.OpCustom((byte)OpCode.MapToServer, parameters, true);
            Console.WriteLine("Map sent: " + authoredMap.mapName);
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
