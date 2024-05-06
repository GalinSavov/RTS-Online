using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Network
{
    public class ColorSetter : NetworkBehaviour
    {
        [SerializeField] private List<Renderer> renderers = new List<Renderer>();

        [SyncVar(hook = nameof(HandleTeamColorUpdated))] private Color teamColor = new Color();

        



        #region Server

        public override void OnStartServer()
        {
            RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
            teamColor = player.GetTeamColor();
        }
        #endregion

        #region Client 
        private void HandleTeamColorUpdated(Color oldColor, Color newColor)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor("_BaseColor", newColor);
            }
        }
        #endregion
    }
}
