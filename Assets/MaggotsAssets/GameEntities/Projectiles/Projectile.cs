using UnityEngine;
using System;

namespace Maggots
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D projectileRigidbody;
        [SerializeField] private Collider2D projectileCollider;

        public Action<Projectile> OnExplode;

        private Weapon weapon;

        public void Init(Weapon weapon, float chargeProgress)
        {
            float force = weapon.IsChargeble ? weapon.ProjectileStartForce * chargeProgress : weapon.ProjectileStartForce;
            projectileRigidbody.AddForce(transform.right * force);
            this.weapon = weapon;
        }

        private void Update()
        {
            if (weapon.MaxUnitDistanceFromCenter * weapon.MaxUnitDistanceFromCenter < transform.position.sqrMagnitude)
            {
                Explode();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Explode();
        }
        private void Explode()
        {
            RaycastHit2D[] raycastHits = Physics2D.CircleCastAll(transform.position, weapon.ExplosionRadius, Vector2.zero);
            foreach (RaycastHit2D hit in raycastHits)
            {
                if (hit.transform.TryGetComponent<IExplodable>(out IExplodable explodable))
                {
                    explodable.OnExplosion(transform.position, weapon);
                }
            }
            FXObjectsPool.Instance.PlayFXParticle(FXObjectsPool.FXType.Explosion, transform.position, new Vector2(weapon.ExplosionRadius, weapon.ExplosionRadius));
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            OnExplode.Invoke(this);
        }
    }
}

