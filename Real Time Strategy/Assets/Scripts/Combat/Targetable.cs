using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Combat
{
    //similar to a tag, every object that our unit can target will have this script on it
    public class Targetable : NetworkBehaviour
    {
        [SerializeField] private Transform aimAtPoint = null;

        public Transform GetAimAtPoint()
        {
            return aimAtPoint;
        }
    }

}