using UnityEngine;

namespace Maggots
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] RigidbodyMovement movement;
        [SerializeField] InputSystem inputSystem;
        [SerializeField] private float moveForce = 10f;
        [SerializeField] private float jumpForce = 60f;

        public void Start()
        {
            inputSystem.Init();
            inputSystem.HorizontalAxisEvent.AddListener(OnHorizontalAxis);
            inputSystem.JumpEvent.AddListener(OnJumpInput);
        }

        public void OnHorizontalAxis(AxisInputEventArgs inputArgs)
        {
            if (movement.IsStayOnGround)
            {
                movement.MoveByDirection(inputArgs.Value, Space.Self, Time.deltaTime * moveForce);
            }
            else
            {
                movement.MoveByDirection(inputArgs.Value, Space.World, Time.deltaTime * moveForce);
            }
        }

        public void OnJumpInput()
        {
            if (movement.IsStayOnGround)
            {
                movement.MoveByDirection(Vector2.up, Space.Self, Time.deltaTime * jumpForce, ForceMode2D.Impulse);
            }             
        }
    }
}
