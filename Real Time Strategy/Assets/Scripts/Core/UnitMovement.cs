using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Core
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] NavMeshAgent playerNavMesh = null;

        #region Server

        [Command]
        public void CmdMovePlayerToPosition(Vector3 position)
        {

            if (!NavMesh.SamplePosition(position, out NavMeshHit navMeshHit, 1f, NavMesh.AllAreas)) return;
            
            playerNavMesh.SetDestination(navMeshHit.position);
            
        }
        #endregion

       
    }

}