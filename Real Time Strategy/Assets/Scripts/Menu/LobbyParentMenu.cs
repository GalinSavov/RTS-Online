using Mirror;
using RTS.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTS.Menu
{
    public class LobbyParentMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _lobbyParentUI = null;
        [SerializeField] private Button startGameButton = null;

        private void OnEnable()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        }

        

        private void OnDisable()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        }

        private void HandleClientConnected()
        {
            _lobbyParentUI.SetActive(true);
        }
        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
                SceneManager.LoadScene(0);
            }
        }
        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool state)
        {
            startGameButton.gameObject.SetActive(state);
        }
    }
}
