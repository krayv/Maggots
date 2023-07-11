using UnityEngine;

namespace Maggots
{
    public interface IMovementSystem
    {
        void MoveByDirection(Vector2 direction, Space space = Space.World, float speed = 1f);
    }
}
