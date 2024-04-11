using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTS.Building;
using UnityEngine.SceneManagement;

namespace RTS.Network
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab = null;

        [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
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

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
            {
                GameOverHandler gameOverHandler = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandler.gameObject);
            }
        }
    }
}