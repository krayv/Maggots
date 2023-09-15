using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maggots
{
    public class MaggotStateInAir : MaggotBehaviour
    {
        public MaggotStateInAir(RigidbodyMovement movement, Maggot.PlayerMovementSettings settings, WeaponGameObject weapon, SpriteRenderer sprite, Animator animator)
        {
            this.movement = movement;
            this.weapon = weapon;
            movementSettings = settings;
            this.mainSprite = sprite;
            this.mainAnimator = animator;
        }

        public override void Jump()
        {

        }

        public override void Move(AxisInputEventArgs inputArgs)
        {
            if (inputArgs.Value.x > 0)
            {
                RotateSprite(SpriteOrientation.Right);
            }
            if (inputArgs.Value.x < 0)
            {
                RotateSprite(SpriteOrientation.Left);
            }
            movement.MoveByDirection(inputArgs.Value, Space.World, Time.deltaTime * movementSettings.HorizontalMoveForce);
        }

        public override void UpdateWeaponDirection(Vector2 direction)
        {
            weapon.SetDirection(direction);
        }

        public override void UseWeapon(Action onStartUsing, Action<bool> onEndUsing)
        {

        }
    }
}

