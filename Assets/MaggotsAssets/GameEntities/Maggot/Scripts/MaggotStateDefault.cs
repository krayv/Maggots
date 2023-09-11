using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maggots
{
    public class MaggotStateDefault : MaggotBehaviour
    {
        public MaggotStateDefault(RigidbodyMovement movement, Maggot.PlayerMovementSettings settings, WeaponGameObject weapon)
        {
            this.movement = movement;
            this.weapon = weapon;
            movementSettings = settings;
        }

        public override void Jump()
        {
            movement.MoveByDirection(Vector2.up, Space.Self, Time.deltaTime * movementSettings.JumpForce, ForceMode2D.Impulse);
        }

        public override void Move(AxisInputEventArgs inputArgs)
        {
            movement.MoveByDirection(inputArgs.Value, Space.World, Time.deltaTime * movementSettings.HorizontalMoveForce);
        }

        public override void UpdateWeaponDirection(Vector2 direction)
        {
            weapon.SetDirection(direction);
        }

        public override void UseWeapon(Action onStartUsing, Action<bool> onEndUsing)
        {
            weapon.onUsing += onStartUsing;
            weapon.onEndUsing += onEndUsing;
            weapon.Use();
        }
    }
}

