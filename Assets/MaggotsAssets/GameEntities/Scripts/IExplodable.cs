using UnityEngine;

namespace Maggots
{
    public interface IExplodable
    {
        void OnExplosion(Vector2 pointOfExplosion, Weapon source);
    }
}

