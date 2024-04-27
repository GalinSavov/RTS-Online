using RTS.Building;
using RTS.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTS.Core
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private Camera mainCamera;
        private void Start()
        {
            mainCamera = Camera.main;
            GameOverHandler.OnCLientGameOver += HandleOnClientGameOver;
        }
        private void OnDestroy()
        {
            GameOverHandler.OnCLientGameOver -= HandleOnClientGameOver;
        }

        private void HandleOnClientGameOver(string obj)
        {
            enabled = false;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;
            

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity,layerMask)) return;

            if(raycastHit.collider.TryGetComponent(out Targetable targetable))
            {
                if(!targetable.isOwned)
                {
                    TryTarget(targetable);
                    return;
                }
                TryMove(raycastHit.point);
                return;
            }

            TryMove(raycastHit.point);
        }

        private void TryTarget(Targetable targetable)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetTargeter().CmdSetTarget(targetable.gameObject);
            }
        }

        private void TryMove(Vector3 point)
        {
            foreach (Unit unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMovePlayerToPosition(point);
            }
        }
    }
}
