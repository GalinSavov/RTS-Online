using Mirror;
using RTS.Buildings;
using RTS.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Core
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        public List<Unit> SelectedUnits { get; } = new List<Unit>();
        private Camera mainCamera;
        [SerializeField] private LayerMask layerMask = new LayerMask();
        [SerializeField] private RectTransform dragBoxSelectionArea = null;
        private RTSPlayer player;
        private Vector2 mouseStartPosition;


        private void Start()
        {
            mainCamera = Camera.main;
            Unit.ClientAuthorityOnUnitDespawned += HandleAuthorityOnUnitDespawned;
            GameOverHandler.OnCLientGameOver += HandleOnClientGameOver;
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        private void OnDestroy()
        {
            Unit.ClientAuthorityOnUnitDespawned -= HandleAuthorityOnUnitDespawned;
            GameOverHandler.OnCLientGameOver -= HandleOnClientGameOver;

        }

        private void HandleOnClientGameOver(string obj)
        {
           enabled = false;
        }

        //this handles when a destroyed Unit still exists in the list, it should be removed from the list
        private void HandleAuthorityOnUnitDespawned(Unit unit)
        {
            SelectedUnits.Remove(unit);
        }

        private void Update()
        {
            
            //it is the logic behind dragging and selecting with the mouse 
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            //update the drag box selection area's size while dragging with the mouse
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void UpdateSelectionArea()
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            float dragBoxWidht = currentMousePosition.x - mouseStartPosition.x;
            float dragBoxHeight = currentMousePosition.y - mouseStartPosition.y;

            //dragging values can also be negative, so using mathf.abs always returns a positive value
            dragBoxSelectionArea.sizeDelta = new Vector2(Mathf.Abs(dragBoxWidht), Mathf.Abs(dragBoxHeight));

            dragBoxSelectionArea.anchoredPosition = mouseStartPosition + new Vector2(dragBoxWidht / 2, dragBoxHeight / 2);

        }
        private void StartSelectionArea()
        {
            //deselect a unit
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (Unit unit in SelectedUnits)
                {
                    unit.Deselect();
                }
                SelectedUnits.Clear();
            }


            //enable the drag box when we first click and set the start position of the mouse
            dragBoxSelectionArea.gameObject.SetActive(true);
            mouseStartPosition = Mouse.current.position.ReadValue();
            UpdateSelectionArea();
        }

        private void ClearSelectionArea()
        {
            dragBoxSelectionArea.gameObject.SetActive(false);

            //check if the player has dragged the mouse to multi-select
            if (dragBoxSelectionArea.sizeDelta.magnitude == 0)
            {
                //select and enable the sprite on a clicked unit
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask)) return;
                if (!raycastHit.collider.TryGetComponent<Unit>(out Unit unit)) return;
                if (!unit.isOwned) return;

                SelectedUnits.Add(unit);

                foreach (Unit selectedUnit in SelectedUnits)
                {
                    selectedUnit.Select();
                }

                return;
            }
            //to check if the Unit is within bounds of the drag area
            //if the Unit's x value is greater than drag box min AND less than drag box max, it is within bounds
            Vector2 min = dragBoxSelectionArea.anchoredPosition - dragBoxSelectionArea.sizeDelta / 2;
            Vector2 max = dragBoxSelectionArea.anchoredPosition + dragBoxSelectionArea.sizeDelta / 2;

            foreach (Unit unit in player.GetMyUnits())
            {
                if (SelectedUnits.Contains(unit)) continue;

                Vector3 unitScreenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (unitScreenPosition.x > min.x && unitScreenPosition.x < max.x
                    && unitScreenPosition.y > min.y && unitScreenPosition.y < max.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }
    }
}
