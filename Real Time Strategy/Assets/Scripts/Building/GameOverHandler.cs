using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Building
{
    public class GameOverHandler : NetworkBehaviour
    {
        private List<UnitBase> unitBases = new List<UnitBase>();
        #region Server
        public override void OnStartServer()
        {
            UnitBase.OnServerBaseSpawned += HandleOnServerBaseSpawned;
            UnitBase.OnServerBaseDespawned += HandleOnServerBaseDespawned;

        }
        public override void OnStopServer()
        {
            UnitBase.OnServerBaseSpawned -= HandleOnServerBaseSpawned;
            UnitBase.OnServerBaseDespawned -= HandleOnServerBaseDespawned;
        }

        [Server]
        private void HandleOnServerBaseSpawned(UnitBase unitBase)
        {
            unitBases.Add(unitBase);
        }


        [Server]
        private void HandleOnServerBaseDespawned(UnitBase unitBase)
        {
            unitBases.Remove(unitBase);

            if (unitBases.Count != 1) return;

            Debug.Log("Game over");
        }

        #endregion

        #region Client

        #endregion
    }
}
