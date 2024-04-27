using Mirror;
using RTS.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))] private int currentHealth;

        public event Action ServerOnDie;
        public event Action<int, int> ClientOnHealthUpdated;

        #region Server

        public override void OnStartServer()
        {
            UnitBase.OnServerPlayerDie += HandleOnServerPlayerDie;
            currentHealth = maxHealth;
        }
        public override void OnStopServer()
        {
            UnitBase.OnServerPlayerDie -= HandleOnServerPlayerDie;

        }

        [Server]
        private void HandleOnServerPlayerDie(int playerConnectionID)
        {
            if (connectionToClient.connectionId != playerConnectionID) return;

            DealDamage(currentHealth);
        }

        [Server]
        public void DealDamage(int damageAmount)
        {
            if (currentHealth == 0) return;

            currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

            if (currentHealth != 0) return;

            ServerOnDie?.Invoke();

            Debug.Log("Died");
        }


        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        }
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        #endregion
    }
}
