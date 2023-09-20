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
            weapon.UpdateCharge(Time.deltaTime);
        }

        public override void Jump()
        {
            weapon.UpdateCharge(Time.deltaTime);
        }

        public override void Move(AxisInputEventArgs inputArgs)
        {
            weapon.UpdateCharge(Time.deltaTime);
        }

        public override void ReleaseFire()
        {
            weapon.Fire();
        }

        public override void UpdateWeaponDirection(Vector2 direction)
        {
            weapon.SetDirection(direction);
        }

        public override void UseWeapon(Action onStartCharging, Action onEndCharging, Action onStartUsing, Action<bool> onEndUsing, Action<float> chargeWeaponProgress)
        {

        }
    }
}

