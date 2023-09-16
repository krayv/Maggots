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
        [SerializeField] private SpriteRenderer mainSprite;
        [SerializeField] private Animator mainAnimator;

        public Team Team;
        public string MaggotName => Team.TeamName;
        public int Health => stats.CurrentLife;
        public float HealthPercent => stats.CurrentLife / stats.MaxLife;
        public RigidbodyMovement RigidbodyMovement => rigidbodyMovement;

        public Action<Maggot> OnDeath;
        public Action<Maggot> OnDestroyGO;
        public Action<Maggot> OnChangeLife;
        public Action<Maggot> OnEndTurn;

        private MaggotBehaviour _stateBehaviour;

        public MaggotState State
        {
            get
            {
                if (_stateBehaviour is MaggotStateDefault)
                {
                    return MaggotState.Default;
                }
                if (_stateBehaviour is MaggotStateInAir)
                {
                    return MaggotState.InAir;
                }
                if (_stateBehaviour is MaggotStateShooting)
                {
                    return MaggotState.Shooting;
                }
                return default;
            }
            set
            {
                switch (value)
                {
                    case MaggotState.Default:
                        _stateBehaviour = new MaggotStateDefault(rigidbodyMovement, moveSettings, weapon, mainSprite, mainAnimator);
                        break;
                    case MaggotState.InAir:
                        _stateBehaviour = new MaggotStateInAir(rigidbodyMovement, moveSettings, weapon, mainSprite, mainAnimator);
                        break;
                    case MaggotState.Shooting:
                        _stateBehaviour = new MaggotStateShooting(rigidbodyMovement, moveSettings, weapon, mainSprite, mainAnimator);
                        break;
                }
            }
        }

        private bool hasAction;

        private void Update()
        {
            UpdateState();
        }

        private void LateUpdate()
        {
            if (hasAction)
            {
                hasAction = false;
            }
            else
            {
                _stateBehaviour.DoNothing();
            }          
        }

        private void OnDestroy()
        {
            OnDestroyGO?.Invoke(this);
        }

        private void UpdateState()
        {
            if (State == MaggotState.Shooting)
            {
                return;
            }

            if (!rigidbodyMovement.IsStayOnGround && State == MaggotState.Default)
            {
                State = MaggotState.InAir;
            }

            if (rigidbodyMovement.IsStayOnGround && State == MaggotState.InAir)
            {
                State = MaggotState.Default;
            }
        }

        public void Init(ArenaController gameController)
        {
            gameController.OnChangeSelectedMaggot += OnChangeSelection;
            stats.OnZeroLife += OnZeroLife;
            stats.CurrentLife = stats.MaxLife;
            stats.OnChangeLife += ChangeLife;
            State = default;
        }

        public void Move(AxisInputEventArgs inputArgs)
        {
            hasAction = true;
            _stateBehaviour.Move(inputArgs);
        }

        public void Jump()
        {
            hasAction = true;
            _stateBehaviour.Jump();
        }

        public void UseWeapon()
        {
            hasAction = true;
            _stateBehaviour.UseWeapon(OnUseWeapon, OnEndUsingWeapon);
        }    

        public void UpdateWeaponDirection(Vector2 direction)
        {
            _stateBehaviour.UpdateWeaponDirection(direction);
        }

        public void OnExplosion(Vector2 pointOfExplosion, Weapon source)
        {
            stats.CurrentLife -= source.Damage;
        }

        private void OnUseWeapon()
        {
            State = MaggotState.Shooting;
            weapon.onUsing -= OnUseWeapon;
        }

        private void OnEndUsingWeapon(bool endTurn)
        {
            weapon.onEndUsing -= OnEndUsingWeapon;
            State = MaggotState.Default;
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

        private void ChangeLife(int value)
        {
            OnChangeLife.Invoke(this);
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
                        if (life <= 0)
                        {
                            OnZeroLife.Invoke();
                        }
                        _currentLife = life;
                        OnChangeLife?.Invoke(life);
                    }

                }
            }
            public int MaxLife;

            public Action<int> OnChangeLife;
            public Action OnZeroLife;
        }
    }
}

