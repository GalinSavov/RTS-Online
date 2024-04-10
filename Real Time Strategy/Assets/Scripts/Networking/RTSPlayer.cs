using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Core;
using Mirror;
using System;

namespace RTS.Network
{
    public class RTSPlayer : NetworkBehaviour
    {

        [SerializeField] private List<Unit> myUnits = new List<Unit>();

        public List<Unit> GetMyUnits()
        {
            return myUnits;
        }

        #region Server

        //this happens when the RTS player is spawned on the server aka BEFORE the unit is spawned into the server
        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        }
        //this happens when the RTS player is despawned from the server aka AFTER the unit is despawned from the server
        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        }
        private void ServerHandleUnitSpawned(Unit unit)
        {
            //check if this unit belongs to the same client connection as the Player object
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            myUnits.Add(unit);
        }
        private void ServerHandleUnitDespawned(Unit unit)
        {
            //check if this unit belongs to the same client connection as the Player object
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            myUnits.Remove(unit);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            if (!isClientOnly) return;
            Unit.ClientAuthorityOnUnitSpawned += HandleClientAuthorityOnUnitSpawned;
            Unit.ClientAuthorityOnUnitDespawned += HandleClientAuthorityOnUnitDespawned;
        }
        public override void OnStopClient()
        {
            if (!isClientOnly) return;
            Unit.ClientAuthorityOnUnitSpawned -= HandleClientAuthorityOnUnitSpawned;
            Unit.ClientAuthorityOnUnitDespawned -= HandleClientAuthorityOnUnitDespawned;
        }
        private void HandleClientAuthorityOnUnitSpawned(Unit unit)
        {
            if (!isOwned) return;
            myUnits.Add(unit);
        }
        private void HandleClientAuthorityOnUnitDespawned(Unit unit)
        {
            if (!isOwned) return;
            myUnits.Remove(unit);
        }
        
        #endregion
    }

}