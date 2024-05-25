using Mirror;
using RTS.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTS.Menu
{
    public class LobbyParentMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _lobbyParentUI = null;
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private TextMeshProUGUI[] playerTextFields = null;

        private void OnEnable()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
            RTSPlayer.OnClientInfoUpdated += HandleClientInfoUpdated;
        }

        

        private void OnDisable()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
            RTSPlayer.OnClientInfoUpdated -= HandleClientInfoUpdated;

        }

        private void HandleClientConnected()
        {
            _lobbyParentUI.SetActive(true);
        }

        private void HandleClientInfoUpdated()
        {
            List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).Players;

            for (int i = 0; i < players.Count; i++)
            {
                playerTextFields[i].text = players[i].GetDisplayName();
            }

            for (int i = players.Count; i < playerTextFields.Length; i++)
            {
                playerTextFields[i].text = "Waiting for player..";
            }

            startGameButton.interactable = players.Count >=2 ;    
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
