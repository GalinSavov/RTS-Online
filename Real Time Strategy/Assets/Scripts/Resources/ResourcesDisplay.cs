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


        private IEnumerator GetPlayer()
        {
            if (player == null)
            {
                yield return new WaitForSeconds(0.5f);
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }
        }

        void Update()
        {
            StartCoroutine(GetPlayer());

            if (player != null)
            {
                HandleOnClientResourcesUpdated(player.GetResources());
                player.OnClientResourcesUpdated += HandleOnClientResourcesUpdated;
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