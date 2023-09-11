using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Maggots
{
    public class WeaponGameObject : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform weaponSpritePoint;

        private WeaponSprite currentSprite;

        public Action onUsing;
        public Action<bool> onEndUsing;

        private readonly List<Projectile> projectiles = new();

        public void Use()
        {
            onUsing?.Invoke();
            if (!weapon.HasDelayBetweenShoots)
            {
                for (int i = 0; i < weapon.ProjectilesCount; i++)
                {
                    SpawnProjectile(weapon.Projectile);
                }
            }
            else
            {
               StartCoroutine(StartShooting());
            }
        }

        private IEnumerator StartShooting()
        {
            int i = 0;
            while (i < weapon.ProjectilesCount)
            {
                SpawnProjectile(weapon.Projectile);
                yield return new WaitForSeconds(weapon.DelayBetweenShoots);
                i++;
            }      
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
            currentSprite = GameObject.Instantiate(weapon.SpritePrefab, weaponSpritePoint);
        }

        public void RemoveWeapon()
        {
            if (currentSprite != null)
            {
                this.weapon = null;
                Destroy(currentSprite.gameObject);
                currentSprite = null;
            }            
        }

        private void SpawnProjectile(Projectile projectilePrefab)
        {
            Projectile projectile = Instantiate(projectilePrefab);
            projectile.transform.SetPositionAndRotation(currentSprite.projectileStartPoint.transform.position, weaponSpritePoint.rotation);
            projectile.Init(weapon);
            projectile.OnExplode += OnProjectileExplode;
            projectiles.Add(projectile);
        }

        private void OnProjectileExplode(Projectile projectile)
        {
            projectiles.Remove(projectile);
            projectile.OnExplode -= OnProjectileExplode;
            if (projectiles.Count == 0)
            {
                onEndUsing.Invoke(weapon.IsUsingEndsTurn);
            }
        }
    }
}
