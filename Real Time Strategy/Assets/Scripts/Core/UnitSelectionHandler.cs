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

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            //deselect a unit

            foreach (Unit unit in SelectedUnits)
            {
                unit.Deselect();
            }
            SelectedUnits.Clear();
        }

        private void ClearSelectionArea()
        {
            //select and enable the sprite on a clicked unit
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity,layerMask)) return;
            if (!raycastHit.collider.TryGetComponent<Unit>(out Unit unit)) return;
            if (!unit.authority) return;

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }
        }
    }
}
