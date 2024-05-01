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
    public class BuldingButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField] private Building building = null;
        [SerializeField] private Image buildingImage = null;
        [SerializeField] private TextMeshProUGUI buildingPrice = null;
        [SerializeField] private LayerMask floorLayerMask = new LayerMask();

        private Camera mainCamera;
        private RTSPlayer player;
        private GameObject buildingPreview;
        private Renderer buildingRenderer;

        void Start()
        {
            mainCamera = Camera.main;

            buildingImage.sprite = building.GetIconSprite();
            buildingPrice.text = building.GetPrice().ToString();
        }

        void Update()
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            }
            if (buildingPreview == null) return;

            UpdateBuildingPreview();

        }
        private void UpdateBuildingPreview()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorLayerMask)) return;

            buildingPreview.transform.position = hit.point;
            
            if(!buildingPreview.activeSelf)
                buildingPreview.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            buildingPreview = Instantiate(building.GetBuildingPreview());
            buildingRenderer = buildingPreview.GetComponentInChildren<Renderer>();

            buildingPreview.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buildingPreview == null) return;
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity, floorLayerMask))
            {
                player.CmdTrySpawnBuilding(building.GetID(), hit.point);
            }
            Destroy(buildingPreview);
        }
    }

}