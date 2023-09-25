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
        public Sprite icon;
        [Tooltip("In Units")]
        public int Order = 0;
        public float ExplosionRadius = 0.5f;
        public float ForcePerDamage = 0.5f;
        public int Damage = 35;
        public float ProjectileStartForce = 100f;
        public float MaxUnitDistanceFromCenter = 100f;
        public int ProjectilesCount = 1;        
        public bool HasDelayBetweenShoots;
        public float DelayBetweenShoots;
        public bool IsUsingEndsTurn = true;
        public bool IsChargeble = true;
        public bool IsProjectileStabilized = true;
        public float ChargingTime = 1.2f;
        public float ExplodeDelay;
    }
}

