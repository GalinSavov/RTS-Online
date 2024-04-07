using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RTS.Core
{
    public class Unit : NetworkBehaviour
    {
        //unity events to toggle the selected sprite on/off
        [SerializeField] private UnityEvent OnSelected = null;
        [SerializeField] private UnityEvent OnDeselected = null;

        [SerializeField] private UnitMovement unitMovement = null;

        #region Client
        [Client]
        public void Select()
        {
            if (!authority) return;
            OnSelected?.Invoke();
        }
        [Client]
        public void Deselect()
        {
            if(!authority) return;
            OnDeselected?.Invoke();
        }

        public UnitMovement GetUnitMovement()
        {
            return unitMovement;
        }

        #endregion
    }
}
