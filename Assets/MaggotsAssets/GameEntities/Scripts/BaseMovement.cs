using UnityEngine;

namespace Maggots
{
    public class BaseMovement : MonoBehaviour, IMovementSystem
    {
        public void MoveByDirection(Vector2 direction, Space space = Space.World, float speed = 1f)
        {
            transform.Translate(Time.deltaTime * speed * direction);
        }
    }
}
