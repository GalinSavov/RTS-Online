using Mirror;
using RTS.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;

        public Targetable GetTarget()
        {
            return target;
        }

        public override void OnStartServer()
        {
            GameOverHandler.OnServerGameOver += HandleOnServerGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.OnServerGameOver -= HandleOnServerGameOver;

        }

        [Server]
        private void HandleOnServerGameOver()
        {
            ClearTarget();
        }

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

        
    }

}