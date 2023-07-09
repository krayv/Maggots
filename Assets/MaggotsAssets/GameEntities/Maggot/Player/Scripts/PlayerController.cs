using UnityEngine;

namespace Maggots
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LinearMovable baseMovable;
        [SerializeField] InputSystem inputSystem;

        public void Start()
        {
            inputSystem.Init();
            inputSystem.AxisEvent.AddListener(OnAxis);
        }

        public void OnAxis(AxisInputEventArgs inputArgs)
        {
            baseMovable.MoveByDirection(inputArgs.Value);
        }
    }
}
