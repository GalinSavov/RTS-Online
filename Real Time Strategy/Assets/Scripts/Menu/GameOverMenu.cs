using Mirror;
using RTS.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RTS.Menu
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] GameObject gameOverMenuParent = null;
        [SerializeField] TextMeshProUGUI winnerText = null;
        void Start()
        {
            GameOverHandler.OnCLientGameOver += HandleOnClientGameOver;
        }
        private void OnDestroy()
        {
            GameOverHandler.OnCLientGameOver -= HandleOnClientGameOver;

        }

        //UI button to be clicked in-game
        public void LeaveGame()
        {
            if(NetworkClient.isConnected && NetworkServer.active)
            {
                //stop hosting
                NetworkManager.singleton.StopHost();
            }
            else
            {
                //stop client
                NetworkManager.singleton.StopClient();
            }
        }
        private void HandleOnClientGameOver(string winner)
        {
            winnerText.text = $"{winner} Has Won!";
            gameOverMenuParent.SetActive(true);
            
        }

    }

}