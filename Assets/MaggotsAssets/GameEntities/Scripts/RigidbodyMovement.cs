using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maggots
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodyMovement : MonoBehaviour, IMovementSystem
    {
        public Rigidbody2D objectRigidbody;

        [SerializeField, Range(-1f, 1f), Tooltip("-1 is a ground looking down, 1 is a ground looking up, 0 is a vertical ground")] 
        private float groundNormalDotProduct = 0.5f;

        private MovementStatus _status;
        public MovementStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                if (_status != value)
                {
                    OnChangeStatus(value);
                    _status = value;
                }              
            }
        }

        private Vector2 leftDirectionVector = Vector2.left;
        private Vector2 rightDirectionVector = Vector2.right;

        public bool IsStayOnGround { get; private set; }

        private ContactPoint2D groundContact;

        private Vector2 lastDirection;

        private void Awake()
        {
            objectRigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay((Vector3)groundContact.point + new Vector3(0,0,-1f), groundContact.normal);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay((Vector3)transform.position, (Vector3)lastDirection);
        }

        public void MoveByDirection(Vector2 direction, Space space = Space.Self, float speed = 1f)
        {
            MoveByDirection(direction, space, speed, ForceMode2D.Force);
        }

        public void MoveByDirection(Vector2 direction, Space space = Space.Self, float speed = 1f, ForceMode2D forceMode = ForceMode2D.Force)
        {
            lastDirection = direction;
            
            switch (space)
            {
                case Space.World:
                    objectRigidbody.AddForce(direction * speed, forceMode);
                    break;
                case Space.Self:
                    objectRigidbody.AddRelativeForce(direction * speed, forceMode);
                    break;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            List<ContactPoint2D> contactPoints = new();
            collision.GetContacts(contactPoints);
            foreach (ContactPoint2D contactPoint in contactPoints)
            {
                if (Vector2.Dot(contactPoint.normal, Vector2.up) > groundNormalDotProduct)
                {
                    IsStayOnGround = true;
                    groundContact = contactPoint;
                    return;
                }
            }
            IsStayOnGround = false;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            IsStayOnGround = false;
        }

        private void OnChangeStatus(MovementStatus newStatus)
        {

        }

        public enum MovementStatus
        {
            Idle, Move, Fly, Fall
        }
    }
}

