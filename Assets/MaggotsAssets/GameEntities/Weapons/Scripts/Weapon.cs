using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    [CreateAssetMenu(fileName ="Weapon", menuName = "MaggotsAssets/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public WeaponSprite SpritePrefab;
        public Projectile Projectile;
        [Tooltip("In Units")]
        public float ExplosionRadius = 0.5f;
        public int Damage = 35;
        public float ProjectileStartForce = 100f;
        public float MaxUnitDistanceFromCenter = 100f;
        public int ProjectilesCount = 1;
        public bool HasDelayBetweenShoots;
        public float DelayBetweenShoots;
        public bool IsUsingEndsTurn = true;
    }
}

