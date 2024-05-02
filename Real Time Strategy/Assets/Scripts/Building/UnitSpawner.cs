using Mirror;
using RTS.Combat;
using RTS.Core;
using RTS.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTS.Buildings
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private Health health = null;
        [SerializeField] private Unit unitPrefab = null;
        [SerializeField] private Transform unitSpawnPoint = null;
        [SerializeField] private TextMeshProUGUI remainingUnitsText = null;
        [SerializeField] private Image unitProgressImage = null;
        [SerializeField] private int maxUnitQueue = 5;
        [SerializeField] private float spawnMoveRange = 15;
        [SerializeField] private float unitSpawnDuration = 5f;
        private float progressImageVelocity;
        private RTSPlayer player;


        [SyncVar] private float unitTimer;
        [SyncVar(hook = nameof(HandleClientQueuedUnits))] private int queuedUnits;

        private void Update()
        {
            if (isServer)
                ProduceUnits();

            if (isClient)
                UpdateTimerDisplay();

        }

        private IEnumerator GetPlayer()
        {
            if (player == null)
            {
                yield return new WaitForSeconds(0.5f);
                player = connectionToClient.identity.GetComponent<RTSPlayer>();
            }
        }
            
        private void UpdateTimerDisplay()
        {
            float newProgress = unitTimer / unitSpawnDuration;

            if (newProgress < unitProgressImage.fillAmount)
                unitProgressImage.fillAmount = newProgress;

            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
            }
        }

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += HandleServerOnDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= HandleServerOnDie;
        }

        [Server]
        private void HandleServerOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (queuedUnits == maxUnitQueue) return;

            StartCoroutine(GetPlayer());

            if(player != null)
            {
                if (player.GetResources() < unitPrefab.GetResourcesCost()) return;

                queuedUnits++;

                player.SetResources(player.GetResources() - unitPrefab.GetResourcesCost());
            }
        }

        [Server]
        private void ProduceUnits()
        {
            if (queuedUnits == 0) return;

            unitTimer += Time.deltaTime;

            if (unitTimer < unitSpawnDuration) return;

            //first,create the game object on the server
            GameObject spawnedUnit = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);

            //spawn a registered prefab that belongs to the client who spawned it
            //all clients get notified of this
            NetworkServer.Spawn(spawnedUnit, connectionToClient);

            //add some offset so the tanks dont stack on top of each other when they spawn
            Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = unitSpawnPoint.position.y;

            //move the unit automatically to the offset
            UnitMovement unitMovement = spawnedUnit.GetComponent<UnitMovement>();
            unitMovement.ServerMoveUnitToPosition(spawnOffset);

            //decrease the queued units amount
            queuedUnits--;

            //reset the timer
            unitTimer = 0;

        }
        #endregion


        #region Client

        //called when I click on the game object this script is attached to
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!isOwned) return;

            CmdSpawnUnit();
        }

        private void HandleClientQueuedUnits(int oldQueuedUnits, int newQueuedUnits)
        {
            remainingUnitsText.text = newQueuedUnits.ToString();
        }
        #endregion
    }

}