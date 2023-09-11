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

        public abstract void Move(AxisInputEventArgs inputArgs);

        public abstract void Jump();

        public abstract void UseWeapon(Action onStartUsing, Action<bool> onEndUsing);

        public abstract void UpdateWeaponDirection(Vector2 direction);
    }

    public enum MaggotState
    {
        Default, InAir, Shooting
    }
}

