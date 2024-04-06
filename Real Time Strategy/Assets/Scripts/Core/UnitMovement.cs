using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace RTS.Core
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] NavMeshAgent playerNavMesh = null;
        Camera mainCamera;

        #region Server
        [Command]
        private void CmdMovePlayerToPosition(Vector3 position)
        {
            if(NavMesh.SamplePosition(position,out NavMeshHit navMeshHit, 1f, NavMesh.AllAreas))
            {
                playerNavMesh.SetDestination(navMeshHit.position);
            }
        }
        #endregion

        #region Client
        public override void OnStartAuthority()
        {
            mainCamera = Camera.main;
        }
        [ClientCallback]
        void Update()
        {
            if (!authority) return;
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
                if(Physics.Raycast(ray,out RaycastHit raycastHit, Mathf.Infinity))
                {
                    CmdMovePlayerToPosition(raycastHit.point);
                }
            }
        }
        #endregion
    }

}