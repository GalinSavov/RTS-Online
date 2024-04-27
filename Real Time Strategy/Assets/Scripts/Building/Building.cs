using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Buildings
{
    public class Building : NetworkBehaviour
    {
        [SerializeField] private Sprite _sprite = null;
        //the building to build will be identified by the server with an id
        [SerializeField] private int _id = -1;
        [SerializeField] private int price = 100;

        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingDespawned;

        public static event Action<Building> ClientOnBuildingSpawned;
        public static event Action<Building> ClientOnBuildingDespawned;

        public Sprite GetIconSprite () { return _sprite; }
        public int GetID() { return _id; }
        public int GetPrice() { return price; }

        #region Server
        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }
        public override void OnStopServer()
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        #endregion

        #region Client
        public override void OnStartAuthority()
        {
            ClientOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            ClientOnBuildingDespawned?.Invoke(this);

        }
        #endregion
    }

}