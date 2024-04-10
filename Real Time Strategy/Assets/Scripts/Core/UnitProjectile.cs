using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Core
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] Rigidbody rb = null;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileLifetime = 5f;
        [SerializeField] private int damageAmount = 15;

        // Start is called before the first frame update
        void Start()
        {
            rb.velocity = transform.forward * projectileSpeed;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroyProjectile), projectileLifetime);
        }

        [Server]
        private void DestroyProjectile()
        {
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) return;

            }
            if(other.TryGetComponent(out Health health))
            {
                health.DealDamage(damageAmount);
            }

            DestroyProjectile();
        }

    }

}