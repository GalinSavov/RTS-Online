using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS.Core;
using Mirror;
using System;
using RTS.Buildings;

namespace RTS.Network
{
    public class RTSPlayer : NetworkBehaviour
    {
        [SerializeField] private List<Building> buildings = new List<Building>();

        private List<Unit> myUnits = new List<Unit>();
        [SerializeField] private List<Building> myBuildings = new List<Building>();

        private Color teamColor = new Color();

        [SyncVar(hook = nameof(HandleClientResoursesUpdated))] private int resources = 200;
        [SerializeField] private LayerMask buildingLayerMask = new LayerMask();
        [SerializeField] private float buildingRangeLimit = 5f;
        [SerializeField] private Transform cameraTransform = null;

        public event Action<int> OnClientResourcesUpdated;

        public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
        {
            //check if the building collides with another building
            if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingLayerMask))
            {
                return false;
            }

            foreach (Building building in myBuildings)
            {
                if (Vector3.Distance(point, building.transform.position) <= buildingRangeLimit)
                {
                    return true;
                }
            }
            return false;
        }
        public List<Unit> GetMyUnits()
        {
            return myUnits;
        }
        public List<Building> GetMyBuildings()
        {
            return myBuildings;
        }
        public int GetResources()
        {
            return resources;
        }
        public Color GetTeamColor()
        {
            return teamColor;
        }

        public Transform GetCameraTransform()
        {
            return cameraTransform;
        }

        #region Server

        //this happens when the RTS player is spawned on the server aka BEFORE the unit is spawned into the server
        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        }

        //this happens when the RTS player is despawned from the server aka AFTER the unit is despawned from the server
        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
        }
        [Server]
        public void SetResources(int newResources)
        {
            resources = newResources;
        }
        [Server]
        public void SetColor(Color color)
        {
            teamColor = color;
        }
        private void ServerHandleUnitSpawned(Unit unit)
        {
            //check if this unit belongs to the same client connection as the Player object
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            myUnits.Add(unit);
        }
        private void ServerHandleUnitDespawned(Unit unit)
        {
            //check if this unit belongs to the same client connection as the Player object
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

            myUnits.Remove(unit);
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            //check if this unit belongs to the same client connection as the Player object
            if (building.connectionToClient.connectionId != connectionToClient.connectionId)
            {
                return;
            }
            myBuildings.Add(building);
        }
        private void ServerHandleBuildingDespawned(Building building)
        {
            //check if this unit belongs to the same client connection as the Player object
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;

            myBuildings.Remove(building);
        }

        [Command]
        public void CmdTrySpawnBuilding(int buildingID, Vector3 point)
        {
            Building buildingToPlace = null;
            foreach (Building building in buildings)
            {
                if (building.GetID() == buildingID)
                {
                    buildingToPlace = building;
                    break;
                }
            }

            if (buildingToPlace == null) return;
            if (resources < buildingToPlace.GetPrice()) return;
            BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();


            if (!CanPlaceBuilding(buildingCollider,point)) return;
           

            GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
            NetworkServer.Spawn(buildingInstance, connectionToClient);

            SetResources(resources - buildingToPlace.GetPrice());
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;
            Unit.ClientAuthorityOnUnitSpawned += HandleClientAuthorityOnUnitSpawned;
            Unit.ClientAuthorityOnUnitDespawned += HandleClientAuthorityOnUnitDespawned;

            Building.ClientOnBuildingSpawned += HandleClientAuthorityOnBuildingSpawned;
            Building.ClientOnBuildingDespawned += HandleClientAuthorityOnBuildingDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !isOwned) return;
            Unit.ClientAuthorityOnUnitSpawned -= HandleClientAuthorityOnUnitSpawned;
            Unit.ClientAuthorityOnUnitDespawned -= HandleClientAuthorityOnUnitDespawned;

            Building.ClientOnBuildingSpawned -= HandleClientAuthorityOnBuildingSpawned;
            Building.ClientOnBuildingDespawned -= HandleClientAuthorityOnBuildingDespawned;
        }
        private void HandleClientAuthorityOnUnitSpawned(Unit unit)
        {
            myUnits.Add(unit);
        }
        private void HandleClientAuthorityOnUnitDespawned(Unit unit)
        {
            myUnits.Remove(unit);
        }
        private void HandleClientAuthorityOnBuildingSpawned(Building building)
        {
            myBuildings.Add(building);
        }
        private void HandleClientAuthorityOnBuildingDespawned(Building building)
        {
            myBuildings.Remove(building);
        }

        //hook for the SyncVar resources
        private void HandleClientResoursesUpdated(int oldValue, int newValue)
        {
            OnClientResourcesUpdated?.Invoke(newValue);
        }

        #endregion
    }

}