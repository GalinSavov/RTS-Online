using Mirror;
using RTS.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Buildings
{
    public class UnitBase : NetworkBehaviour
    {

        [SerializeField] private Health health;

        public static event Action<int> OnServerPlayerDie;
        public static event Action<UnitBase> OnServerBaseSpawned;
        public static event Action<UnitBase> OnServerBaseDespawned;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += HandleServerOnDie;
            OnServerBaseSpawned?.Invoke(this);
        }
        public override void OnStopServer()
        {
            health.ServerOnDie -= HandleServerOnDie;
            OnServerBaseDespawned?.Invoke(this);
        }

        [Server]
        private void HandleServerOnDie()
        {
            OnServerPlayerDie?.Invoke(connectionToClient.connectionId);
            NetworkServer.Destroy(gameObject);
        }
        #endregion


        #region Client

        #endregion
    }
}
