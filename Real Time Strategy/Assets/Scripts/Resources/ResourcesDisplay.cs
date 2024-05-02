using Mirror;
using RTS.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RTS.Resources
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resourcesText = null;
        private RTSPlayer player;
    

        void Update()
        {
            if(player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

                if(player != null)
                {
                    HandleOnClientResourcesUpdated(player.GetResources());
                    player.OnClientResourcesUpdated += HandleOnClientResourcesUpdated;
                }
            }
        }

        private void OnDestroy()
        {
            player.OnClientResourcesUpdated -= HandleOnClientResourcesUpdated;
        }

        private void HandleOnClientResourcesUpdated(int resourcesAmount)
        {
            resourcesText.text = $"Resources: {resourcesAmount}";
        }
    }

}