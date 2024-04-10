using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        [SerializeField] private Targetable target;

        #region Server
        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            //if the game object does not have a Target component, return
            if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) return;
            target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        #endregion


        #region Client

        #endregion
    }

}