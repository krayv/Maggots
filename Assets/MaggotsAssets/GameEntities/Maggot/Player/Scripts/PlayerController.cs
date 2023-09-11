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

        private List<Maggot> trackedMaggots;

        public void Init(InputSystem input)
        {
            inputSystem = input;            
            inputSystem.HorizontalAxisEvent.AddListener(OnHorizontalAxis);
            inputSystem.JumpEvent.AddListener(OnJumpInput);
            inputSystem.FireEvent.AddListener(OnFireInput);
        }

        private void Update()
        {
            UpdateMousePosition();
        }

        private void UpdateMousePosition()
        {
            foreach (var maggot in trackedMaggots)
            {
                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - maggot.transform.position;
                maggot.UpdateWeaponDirection(direction);
            }
        }

        public void TrackNewMovement(List<Maggot> maggots)
        {
            trackedMaggots = maggots;
        }

        public void OnHorizontalAxis(AxisInputEventArgs inputArgs)
        {
            foreach (var maggot in trackedMaggots)
            {
                maggot.Move(inputArgs);
            }          
        }

        public void OnFireInput()
        {
            foreach (var maggot in trackedMaggots)
            {
                maggot.UseWeapon();
            }
        }

        public void OnJumpInput()
        {
            foreach (var maggot in trackedMaggots)
            {
                maggot.Jump();
            }                      
        }
    }
}
