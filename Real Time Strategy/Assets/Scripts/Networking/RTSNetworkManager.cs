using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RTS.Buildings;
using UnityEngine.SceneManagement;
using System;

namespace RTS.Network
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitBasePrefab = null;

        [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

        //events whose subscribers are in the JoinLobbyMenu.cs
        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;


        private bool isGameInProgress = false;

        public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

        #region Server
        //when a client connects to the server, instantiate a unit spawner that belongs to that client
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

            Players.Add(player);

            player.SetColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));

            player.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (!isGameInProgress) return;

            conn.Disconnect();
        }
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            Players.Remove(player);

            base.OnServerDisconnect(conn);
        }
        public override void OnStopServer()
        {
            Players.Clear();
            isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2) return;
            isGameInProgress = true;
            ServerChangeScene("Map_01Scene");
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Map_"))
            {
                GameOverHandler gameOverHandler = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(gameOverHandler.gameObject);

                foreach (RTSPlayer player in Players)
                {
                    GameObject baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);
                    NetworkServer.Spawn(baseInstance, player.connectionToClient);
                }
            }


        }
        #endregion


        #region Client
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientOnConnected?.Invoke();
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientOnDisconnected?.Invoke();
        }
        public override void OnStopClient()
        {
            Players.Clear();
        }
        #endregion




    }
}