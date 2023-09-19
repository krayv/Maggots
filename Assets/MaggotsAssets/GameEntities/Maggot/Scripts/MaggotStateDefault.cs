using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maggots
{
    public class MaggotStateDefault : MaggotBehaviour
    {
        private readonly CapsuleCollider2D capsuleCollider;
        private readonly float climbSpeed = 15f;
        private bool jumped;
        public MaggotStateDefault(RigidbodyMovement movement, Maggot.PlayerMovementSettings settings, WeaponGameObject weapon, SpriteRenderer sprite, Animator animator, CapsuleCollider2D capsuleCollider)
        {
            this.movement = movement;
            this.weapon = weapon;
            movementSettings = settings;
            this.mainSprite = sprite;
            this.mainAnimator = animator;
            this.capsuleCollider = capsuleCollider;
        }

        public override void Jump()
        {           
            if (!jumped)
            {
                SetTrigger(JumpTrigger);
                movement.MoveByDirection(Vector2.up, Space.World, movementSettings.JumpForce, ForceMode2D.Impulse);
                jumped = true;
            }            
        }

        public override void DoNothing()
        {
            SetTrigger(IdleTrigger);
        }

        public override void Move(AxisInputEventArgs inputArgs)
        {
            SetTrigger(WalkTrigger);
            Vector2 direction = Vector2.zero;
            if (inputArgs.Value.x > 0)
            {
                direction = GetHorizontalDirection(false);
                RotateSprite(SpriteOrientation.Right);
            }
            if (inputArgs.Value.x < 0)
            {
                direction = GetHorizontalDirection(true);
                RotateSprite(SpriteOrientation.Left);
            }
            movement.MoveByDirection(direction, Space.Self, Time.deltaTime * movementSettings.HorizontalMoveForce);
        }

        private Vector2 GetHorizontalDirection(bool left)
        {
            float threshold = 0.0f;
            float lowestYColliderPoint = -capsuleCollider.size.y / 2f;
            Vector3 lowestColliderPoint = capsuleCollider.gameObject.transform.localToWorldMatrix.MultiplyPoint(new Vector3(0, lowestYColliderPoint, 0));

            List<ContactPoint2D> points = new();
            capsuleCollider.GetContacts(points);

            if (points.Count == 0)
            {
                return left ? Vector2.left : Vector2.right;
            }

            Vector2 highestPoint;
            highestPoint = points[0].point;
            foreach (var point in points)
            {
                if ((left && point.point.x < capsuleCollider.transform.position.x) || (!left && point.point.x < capsuleCollider.transform.position.x))
                {                      
                    if (point.point.y > lowestColliderPoint.y && point.point.y > highestPoint.y)
                    {
                        highestPoint = point.point;
                    }
                }
            }
            if (highestPoint.y < lowestColliderPoint.y + threshold)
            {
                return left ? Vector2.left : Vector2.right;
            }
            else
            {
                highestPoint = capsuleCollider.transform.worldToLocalMatrix.MultiplyPoint(highestPoint + new Vector2(0, capsuleCollider.size.y / 2f));
                return left ? new Vector2(-1f, highestPoint.y * climbSpeed) : new Vector2(1f, highestPoint.y * climbSpeed);
            }
        }

        public override void UpdateWeaponDirection(Vector2 direction)
        {
            if (direction.x > 0)
            {
                RotateSprite(SpriteOrientation.Right);
            }
            if (direction.x < 0)
            {
                RotateSprite(SpriteOrientation.Left);
            }
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

