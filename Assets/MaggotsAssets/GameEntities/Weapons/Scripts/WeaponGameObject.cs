using UnityEngine;

namespace Maggots
{
    public class WeaponGameObject : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform weaponSpritePoint;

        private WeaponSprite currentSprite;

        public void Fire()
        {
            Projectile projectile = Instantiate(weapon.projectile);
            projectile.transform.position = currentSprite.projectileStartPoint.transform.position;
            projectile.transform.rotation = weaponSpritePoint.rotation;
            projectile.Init(weapon);
        }

        public void SetDirection(Vector2 direction)
        {
            float angle = -Mathf.Atan2(direction.x, direction.y) * 57.2958f + 90f;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            weaponSpritePoint.rotation = rotation;
        }

        public void SwitchWeapon(Weapon weapon)
        {
            this.weapon = weapon;
            Destroy(currentSprite);
            currentSprite = GameObject.Instantiate(weapon.spritePrefab, weaponSpritePoint);
        }

        public void RemoveWeapon()
        {
            this.weapon = null;
            Destroy(currentSprite);
        }
    }
}
