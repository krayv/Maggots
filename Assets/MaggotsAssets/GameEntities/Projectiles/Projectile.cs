using UnityEngine;

namespace Maggots
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D projectileRigidbody;
        [SerializeField] private Collider2D projectileCollider;

        private Weapon weapon;

        public void Init(Weapon weapon)
        {
            projectileRigidbody.AddForce(transform.right * weapon.ProjectileStartForce);
            this.weapon = weapon;
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
    }
}

