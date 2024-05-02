using Mirror;
using RTS.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.Core
{
    public class Unit : NetworkBehaviour
    {
        
        //unity events to toggle the selected sprite on/off
        [SerializeField] private UnityEvent OnSelected = null;
        [SerializeField] private UnityEvent OnDeselected = null;

        [SerializeField] private int resourcesCost = 10;
        [SerializeField] private UnitMovement unitMovement = null;
        [SerializeField] private Targeter targeter = null;
        [SerializeField] private Health health = null;

        //events to be called on the server when a unit spawns/despawns
        //it is static because the server does not need to have a reference to what unit it was exactly, at least for now
        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> ClientAuthorityOnUnitSpawned;
        public static event Action<Unit> ClientAuthorityOnUnitDespawned;




        #region Server

        //a unit has been spawned into the server; happens AFTER the RTS Player has been spawned into the server
        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += HandleServerOnDie;
        }
        //a unit has been despawned from the server; happens BEFORE the RTS Player has been despawned from the server
        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
            health.ServerOnDie -= HandleServerOnDie;
        }
        [Server]
        private void HandleServerOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }


        #endregion


        #region Client
        [Client]
        public void Select()
        {
            if (!isOwned) return;
            OnSelected?.Invoke();
        }
        [Client]
        public void Deselect()
        {
            if(!isOwned) return;
            OnDeselected?.Invoke();
        }

        public UnitMovement GetUnitMovement()
        {
            return unitMovement;
        }
        public Targeter GetTargeter()
        {
            return targeter;
        }
        public int GetResourcesCost()
        {
            return resourcesCost;
        }

        //used to store the units for each unique client in the game
        public override void OnStartAuthority()
        {
            ClientAuthorityOnUnitSpawned?.Invoke(this);

        }
        
        public override void OnStopClient()
        {
            if (!isOwned) return;
            ClientAuthorityOnUnitDespawned?.Invoke(this);
        }

        #endregion
    }
}
