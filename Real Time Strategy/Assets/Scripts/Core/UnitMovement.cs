using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace RTS.Core
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] NavMeshAgent playerNavMesh = null;

        #region Server

        [ServerCallback]
        private void Update()
        {

            if (!playerNavMesh.hasPath) return;
            //when the Unit reaches the stopping distance, it will clear its path and no longer push other units around
            //if there are 3 units, 1 will get there first and stop fighting, then number 2 will get there and stop fighting
            //this goes on until all units stop fighting each other

            if (playerNavMesh.remainingDistance > playerNavMesh.stoppingDistance) return;
            playerNavMesh.ResetPath();
        }

        [Command]
        public void CmdMovePlayerToPosition(Vector3 position)
        {

            if (!NavMesh.SamplePosition(position, out NavMeshHit navMeshHit, 1f, NavMesh.AllAreas)) return;
            
            playerNavMesh.SetDestination(navMeshHit.position);
            
        }
        #endregion

       
    }

}