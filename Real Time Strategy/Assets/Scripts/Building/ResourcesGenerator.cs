using Mirror;
using RTS.Combat;
using RTS.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Buildings
{
    public class ResourcesGenerator : NetworkBehaviour
    {
        [SerializeField] private Health health = null;
        [SerializeField] private int resourcesPerInterval = 10;
        [SerializeField] private float interval = 2f;

        private RTSPlayer player;
        private float timer;

        public override void OnStartServer()
        {
            
            player = connectionToClient.identity.GetComponent<RTSPlayer>();


            health.ServerOnDie += HandleServerOnDie;
            GameOverHandler.OnServerGameOver += HandleOnServerGameOver;

            StartCoroutine(GenerateResources());
        }
        public override void OnStopServer()
        {
            health.ServerOnDie -= HandleServerOnDie;
            GameOverHandler.OnServerGameOver -= HandleOnServerGameOver;
        }

        private void HandleServerOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void HandleOnServerGameOver()
        {
            enabled = false;
        }

        //only the server/host will run this update
        [ServerCallback]

        //only the server/host will run this update
        private void Update()
        {
            
        }

        private IEnumerator GenerateResources() 
        { 
            while(enabled == true)
            {
                yield return new WaitForSeconds(interval);
                player.SetResources(player.GetResources() + resourcesPerInterval);
            }
        }
    }
}
