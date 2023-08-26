using UnityEngine;

namespace Maggots
{
    public class MaggotEntity : MonoBehaviour
    {
        [SerializeField] private RigidbodyMovement rigidbodyMovement;
        public RigidbodyMovement RigidbodyMovement => rigidbodyMovement;
    }
}

