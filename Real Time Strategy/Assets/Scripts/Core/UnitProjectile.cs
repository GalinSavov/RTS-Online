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


    }

}