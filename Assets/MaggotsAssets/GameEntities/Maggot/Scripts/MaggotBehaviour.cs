using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maggots
{
    public abstract class MaggotBehaviour 
    {
        protected RigidbodyMovement movement;
        protected Maggot.PlayerMovementSettings movementSettings;
        protected WeaponGameObject weapon;
        protected SpriteRenderer mainSprite;
        protected Animator mainAnimator;
        protected SpriteOrientation orientation;

        protected readonly string JumpTrigger = "Jump";
        protected readonly string IdleTrigger = "Idle";
        protected readonly string WalkTrigger = "Walk";
        protected readonly string InAirTrigger = "InAir";

        public abstract void DoNothing();

        public abstract void Move(AxisInputEventArgs inputArgs);

        public abstract void Jump();

        public abstract void UseWeapon(Action onStartCharging, Action onEndCharging, Action onStartUsing, Action<bool> onEndUsing, Action<float> chargeWeaponProgress);

        public abstract void ReleaseFire();

        public abstract void UpdateWeaponDirection(Vector2 direction);

        protected void SetTrigger(string name)
        {
            ResetAllTriggers();
            mainAnimator.SetTrigger(name);
        }

        protected void ResetAllTriggers()
        {
            mainAnimator.ResetTrigger(JumpTrigger);
            mainAnimator.ResetTrigger(IdleTrigger);
            mainAnimator.ResetTrigger(WalkTrigger);
            mainAnimator.ResetTrigger(InAirTrigger);
        }

        protected void RotateSprite(SpriteOrientation orientation)
        {
            this.orientation = orientation;
            if (orientation == SpriteOrientation.Left)
            {
                mainSprite.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                mainSprite.gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }

        protected enum SpriteOrientation
        {
            Left, Right
        }
    }

    public enum MaggotState
    {
        Default, InAir, Shooting
    }
}

