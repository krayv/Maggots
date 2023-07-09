using UnityEngine;

namespace Maggots
{
    public class LinearMovable : MonoBehaviour, IMovable
    {
        [SerializeField] private float Speed = 1f;

        public void MoveByDirection(Vector2 direction)
        {
            transform.Translate(Speed * Time.deltaTime * direction);
        }
    }
}
