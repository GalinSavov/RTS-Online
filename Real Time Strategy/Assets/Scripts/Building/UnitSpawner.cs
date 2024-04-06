using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RTS.Building
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unitPrefab = null;
        [SerializeField] private Transform unitSpawnPoint = null;

       
        #region Server

        [Command]
        private void CmdSpawnUnit()
        {
            //first,create the game object on the server
            GameObject spawnedUnit = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

            //spawn a registered prefab that belongs to the client who spawned it
            //all clients get notified of this
            NetworkServer.Spawn(spawnedUnit,connectionToClient);
        }
        #endregion


        #region Client

        //called when I click on the game object this script is attached to
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!authority) return;

            CmdSpawnUnit();
        }

        #endregion
    }

}