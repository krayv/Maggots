using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Maggots
{
    public class PlayerController : MonoBehaviour
    {     
        [SerializeField] InputSystem inputSystem;
        [SerializeField] private float moveForce = 10f;
        [SerializeField] private float jumpForce = 60f;

        private List<RigidbodyMovement> trackedMovements;

        public void Init(InputSystem input)
        {
            inputSystem = input;            
            inputSystem.HorizontalAxisEvent.AddListener(OnHorizontalAxis);
            inputSystem.JumpEvent.AddListener(OnJumpInput);
        }

        public void TrackNewMovement(List<RigidbodyMovement> movements)
        {
            trackedMovements = movements;
        }

        public void OnHorizontalAxis(AxisInputEventArgs inputArgs)
        {
            foreach (var movement in trackedMovements)
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
            
        }

        public void OnJumpInput()
        {
            foreach (var movement in trackedMovements)
            {
                if (movement.IsStayOnGround)
                {
                    movement.MoveByDirection(Vector2.up, Space.Self, Time.deltaTime * jumpForce, ForceMode2D.Impulse);
                }
            }
                      
        }
    }
}
