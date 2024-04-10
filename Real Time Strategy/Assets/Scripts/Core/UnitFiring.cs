using Mirror;
using RTS.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Core
{
    //this script is attached to the Unit prefab
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter targeter = null;
        [SerializeField] private GameObject projectilePrefab = null;
        [SerializeField] private Transform projectileSpawnPoint = null;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private float attackRange = 5f;
        [SerializeField] private float rotationSpeed = 20f;

        private float lastAttackTime;

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.GetTarget();
            if (!CanFireAtTarget()) return;
            if (target == null) return;

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //1/fireRate calculates how many times the Unit can shoot per second
            if(Time.time > (1/fireRate) + lastAttackTime)
            {
                Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);
                GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

                NetworkServer.Spawn(projectileInstance,connectionToClient);
                lastAttackTime = Time.time;
            }

        }
        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= attackRange * attackRange;
        }
    }

}