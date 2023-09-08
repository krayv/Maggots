using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    [CreateAssetMenu(fileName ="Weapon", menuName = "MaggotsAssets/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public WeaponSprite spritePrefab;
        public Projectile projectile;
        [Tooltip("In Units")]
        public float ExplosionRadius = 0.5f;
        public int Damage = 35;
        public float ProjectileStartForce = 100f;
    }
}

