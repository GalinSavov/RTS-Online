using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace RTS.Network
{
    public class MyNetworkPlayer : NetworkBehaviour
    {
        //syncVar updates this variable's state for all other clients in the server
        [SyncVar][SerializeField] private string displayName = "Missing Name";
        [SyncVar][SerializeField] Color playerColor = Color.black;

        //this means that this method should only be run in the server (Network Manager)
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
    }

}