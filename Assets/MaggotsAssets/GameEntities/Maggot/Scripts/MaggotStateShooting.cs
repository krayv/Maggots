using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maggots
{
    public class MaggotStateShooting : MaggotBehaviour
    {
        public MaggotStateShooting(RigidbodyMovement movement, Maggot.PlayerMovementSettings settings, WeaponGameObject weapon, SpriteRenderer sprite, Animator animator)
        {
            this.movement = movement;
            this.weapon = weapon;
            movementSettings = settings;
            this.mainSprite = sprite;
            this.mainAnimator = animator;
        }

        public override void DoNothing()
        {

        }

        public override void Jump()
        {

        }

        public override void Move(AxisInputEventArgs inputArgs)
        {

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

