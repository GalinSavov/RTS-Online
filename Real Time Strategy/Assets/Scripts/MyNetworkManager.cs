using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace RTS.Network
{
    public class MyNetworkManager : NetworkManager
    {
        //overrides the method and gives each player a unique display name
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
            player.SetDisplayName($"Player{numPlayers}");
            player.SetPlayerColor();
        }
    }

}