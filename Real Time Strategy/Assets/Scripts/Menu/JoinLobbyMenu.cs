using Mirror;
using RTS.Network;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RTS.Menu
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _landingPageMenu = null;
        [SerializeField] private TMP_InputField _addressInputField = null;
        [SerializeField] private Button _joinButton = null;


        private void OnEnable()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        }

        public void JoinGame()
        {
            //set the address entered by the user
            string address = _addressInputField.text;
            NetworkManager.singleton.networkAddress = address;

            NetworkManager.singleton.StartClient();
            _joinButton.interactable = false;
        }
        private void HandleClientConnected()
        {
            _joinButton.interactable = true;

            gameObject.SetActive(false);
            _landingPageMenu.SetActive(false);
        }
        private void HandleClientDisconnected()
        {
            _joinButton.interactable = true;
        }
    }
}
