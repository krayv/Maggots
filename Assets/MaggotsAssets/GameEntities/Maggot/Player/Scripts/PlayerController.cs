using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Maggots
{
    public class PlayerController : MonoBehaviour
    {     
        [SerializeField] InputSystem inputSystem;

        private List<Maggot> trackedMaggots;

        public void Init(InputSystem input)
        {
            inputSystem = input;            
            inputSystem.HorizontalAxisEvent.AddListener(OnHorizontalAxis);
            inputSystem.JumpEvent.AddListener(OnJumpInput);
            inputSystem.FireStartEvent.AddListener(OnFireInput);
            inputSystem.FireReleaseEvent.AddListener(OnReleaseFireInput);
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
            trackedMaggots?.ForEach(m => m.OnDeath -= OnMaggotDeath);
            trackedMaggots = maggots;
            trackedMaggots.ForEach(m => m.OnDeath += OnMaggotDeath);
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

        public void OnReleaseFireInput()
        {
            foreach (var maggot in trackedMaggots)
            {
                maggot.ReleaseFire();
            }
        }

        public void OnJumpInput()
        {
            foreach (var maggot in trackedMaggots)
            {
                maggot.Jump();
            }                      
        }

        private void OnMaggotDeath(Maggot maggot)
        {
            maggot.OnDeath -= OnMaggotDeath;
            trackedMaggots.Remove(maggot);
            //if (trackedMaggots.Count == 0)
            //{
            //    maggot.OnEndTurn.Invoke(maggot);
            //}
        }
    }
}
