using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
namespace RTS.Network
{
    public class MyNetworkPlayer : NetworkBehaviour
    {
        //syncVar updates this variable's state for all other clients in the server
        [SyncVar(hook = nameof(HandlePlayerColorChanged))][SerializeField] Color playerColor = Color.black;
        [SerializeField] TMP_Text playerNameText = null;
        [SerializeField] Renderer playerColorRenderer = null;
        [SyncVar(hook = nameof(HandlePlayerTextChanged))][SerializeField] private string displayName = "Missing Name";

        #region Server
        /// <summary>
        /// Update the display name in the server. Can't be called from a client, unlike a Command.
        /// </summary>
        /// <param name="newDisplayName"></param>
        [Server]
        public void SetDisplayName(string newDisplayName)
        {
            displayName = newDisplayName;
        }
        [Server]
        public void SetPlayerColor()
        {
            //creates a random color each time
            playerColor = new Color(Random.value, Random.value, Random.value);
        }

        /// <summary>
        /// Update the display name in the server and then broadcast it to all other clients
        /// </summary>
        /// <param name="newDisplayName"></param>
        [Command] 
        private void CmdSetDisplayName(string newDisplayName)
        {
            if (newDisplayName.Length < 2) return;

            RpcShowUpdatedDisplayNameToClients(newDisplayName);
            SetDisplayName(newDisplayName);
        }
        #endregion

        #region Client
        private void HandlePlayerColorChanged(Color oldColour,Color newColour)
        {
            playerColorRenderer.material.color = newColour;
        }
        private void HandlePlayerTextChanged(string oldText, string newText)
        {
            playerNameText.text = newText;
        }
        [ContextMenu("Set my name")]
        private void SetMyName()
        {
            CmdSetDisplayName("My new name");
        }

        [ClientRpc] public void RpcShowUpdatedDisplayNameToClients(string updatedName)
        {
            Debug.Log(updatedName);
        }
        #endregion
    }

}