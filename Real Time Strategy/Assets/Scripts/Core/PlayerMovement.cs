using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent = null;
    private Camera mainCamera;

    #region Server
    [Command] private void CmdMove(Vector3 position)
    {
        //sample position checks if the position is valid / is it within the bounds of the NavMesh
        //maxDistance is the allowed distance outside of the NavMesh to click
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        //the agent should move to where the nav mesh hit on the surface, not to the passed in parameter position
        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client

    //similar to a Start()
    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
    }

    //called only for the client and not the server
    [ClientCallback]
    private void Update()
    {
        if (!authority) return;
        if (Input.GetMouseButtonDown(1))
        {
           //get the ray from the mouse cursor position
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //draws a raycast and returns false if it did not hit anything
            if (!Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity)) return;

            //move to the point where the Raycast hit
            CmdMove(raycastHit.point);
        }
    }
    #endregion
}
