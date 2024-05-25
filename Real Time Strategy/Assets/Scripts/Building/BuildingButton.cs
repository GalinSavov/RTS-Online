using Mirror;
using RTS.Network;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RTS.Buildings
{
    public class BuldingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Building building = null;
        [SerializeField] private Image buildingImage = null;
        [SerializeField] private TextMeshProUGUI buildingPrice = null;
        [SerializeField] private LayerMask floorLayerMask = new LayerMask();

        private Camera mainCamera;
        private RTSPlayer player;
        private GameObject buildingPreview;
        private Renderer buildingRenderer;
        private BoxCollider buildingCollider;

        void Start()
        {
            mainCamera = Camera.main;

            buildingImage.sprite = building.GetIconSprite();
            buildingPrice.text = building.GetPrice().ToString();
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            buildingCollider = building.GetComponent<BoxCollider>();
            
        }
        void Update()
        {
            if (buildingPreview == null) return;

            UpdateBuildingPreview();

        }
        private void UpdateBuildingPreview()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorLayerMask)) return;

            buildingPreview.transform.position = hit.point;

            if (!player.CanPlaceBuilding(buildingCollider, hit.point))
                buildingRenderer.material.SetColor("_BaseColor", Color.red);
            else
                buildingRenderer.material.SetColor("_BaseColor", Color.green);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (player.GetResources() < building.GetPrice()) return;

            buildingPreview = Instantiate(building.GetBuildingPreview());
            buildingRenderer = buildingPreview.GetComponentInChildren<Renderer>();


        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buildingPreview == null) return;
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorLayerMask))
            {
                player.CmdTrySpawnBuilding(building.GetID(), hit.point);
            }
            Destroy(buildingPreview);
        }
    }

}