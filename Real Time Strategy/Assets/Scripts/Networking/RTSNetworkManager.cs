using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace RTS.Network
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab = null;
        //when a client connects to the server, instantiate a unit spawner that belongs to that client
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            //first, create a unit spawner on the server
            GameObject unitSpawnerInstance = Instantiate(
                unitSpawnerPrefab, 
                conn.identity.transform.position,
                conn.identity.transform.rotation);

            //then, broadcast that to all other clients
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}