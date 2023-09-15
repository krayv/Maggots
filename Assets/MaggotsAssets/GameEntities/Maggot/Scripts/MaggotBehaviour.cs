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

        public abstract void Move(AxisInputEventArgs inputArgs);

        public abstract void Jump();

        public abstract void UseWeapon(Action onStartUsing, Action<bool> onEndUsing);

        public abstract void UpdateWeaponDirection(Vector2 direction);

        protected void RotateSprite(SpriteOrientation orientation)
        {
            if (this.orientation == orientation)
            {
                return;
            }
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

