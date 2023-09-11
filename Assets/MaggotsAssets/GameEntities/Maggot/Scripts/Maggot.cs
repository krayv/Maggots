using UnityEngine;
using System;

namespace Maggots
{
    public class Maggot : MonoBehaviour, IExplodable
    {
        [SerializeField] private RigidbodyMovement rigidbodyMovement;
        [SerializeField] private PlayerMovementSettings moveSettings;
        [SerializeField] private WeaponGameObject weapon;
        [SerializeField] private Weapon weaponSO;
        [SerializeField] private MaggotStats stats;

        public RigidbodyMovement RigidbodyMovement => rigidbodyMovement;

        public Action<Maggot> OnDeath;
        public Action<Maggot> OnEndTurn;

        public void Init(ArenaController gameController)
        {
            gameController.OnChangeSelectedMaggot += OnChangeSelection;
            stats.OnZeroLife += OnZeroLife;
            stats.CurrentLife = stats.MaxLife;
        }

        public void Move(AxisInputEventArgs inputArgs)
        {
            if (rigidbodyMovement.IsStayOnGround)
            {
                rigidbodyMovement.MoveByDirection(inputArgs.Value, Space.Self, Time.deltaTime * moveSettings.HorizontalMoveForce);
            }
            else
            {
                rigidbodyMovement.MoveByDirection(inputArgs.Value, Space.World, Time.deltaTime * moveSettings.HorizontalMoveForce);
            }
        }

        public void Jump()
        {
            if (rigidbodyMovement.IsStayOnGround)
            {
                rigidbodyMovement.MoveByDirection(Vector2.up, Space.Self, Time.deltaTime * moveSettings.JumpForce, ForceMode2D.Impulse);
            }
        }

        public void UseWeapon()
        {
            weapon.onEndUsing += OnEndUsingWeapon;
            weapon.Use();
        }

        public void OnExplosion(Vector2 pointOfExplosion, Weapon source)
        {
            stats.CurrentLife -= source.Damage;
        }

        public void UpdateWeaponDirection(Vector2 direction)
        {
            weapon.SetDirection(direction);
        }

        private void OnEndUsingWeapon(bool endTurn)
        {
            if (endTurn)
            {
                OnEndTurn?.Invoke(this);
            }
        }

        private void OnChangeSelection(Maggot maggot)
        {
            if (maggot != this)
            {
                weapon.RemoveWeapon();
            }
            else
            {
                weapon.SwitchWeapon(weaponSO);
            }
        }

        private void OnZeroLife()
        {
            stats.OnZeroLife -= OnZeroLife;
            OnDeath.Invoke(this);
            GameObject.Destroy(gameObject);
        }


        [Serializable]
        public struct PlayerMovementSettings
        {
            public float JumpForce;
            public float HorizontalMoveForce;
        }

        [Serializable]
        public struct MaggotStats
        {
            private int _currentLife;

            public int CurrentLife
            {
                get
                {
                    return _currentLife;
                }
                set
                {
                    int life = value > MaxLife ? MaxLife : value;
                    if (life != _currentLife)
                    {
                        //OnChangeLife.Invoke(life);
                        if (life <= 0)
                        {
                            OnZeroLife.Invoke();
                        }
                        _currentLife = life;
                    }

                }
            }
            public int MaxLife;

            public Action<int> OnChangeLife;
            public Action OnZeroLife;
        }
    }   
}

