using UnityEngine;
using System;
using System.Collections.Generic;

namespace Maggots
{
    public class Maggot : MonoBehaviour, IExplodable
    {
        [SerializeField] private RigidbodyMovement rigidbodyMovement;
        [SerializeField] private PlayerMovementSettings moveSettings;
        [SerializeField] private WeaponGameObject weaponGO;
         
        [SerializeField] private MaggotStats stats;
        [SerializeField] private SpriteRenderer mainSprite;
        [SerializeField] private Animator mainAnimator;
        [SerializeField] private CapsuleCollider2D capsuleCollider;

        public Team Team;
        public string MaggotName => Team.TeamName;
        public int Health => stats.CurrentLife;
        public float HealthPercent => stats.CurrentLife / stats.MaxLife;
        public RigidbodyMovement RigidbodyMovement => rigidbodyMovement;

        public Action<Maggot> OnDeath;
        public Action<Maggot> OnDestroyGO;
        public Action<Maggot> OnChangeLife;
        public Action<Maggot> OnEndTurn;
        public Action<Maggot, float> OnChargeWeapon;

        private MaggotBehaviour _stateBehaviour;
        private readonly float switchToAirStateDelay = 0.5f;
        private float currentSwitchDelay;
        private Inventory Inventory => Team.Inventory;
        private Weapon SelectedWeapon => Inventory.SelectedWeapon;

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
                        _stateBehaviour = new MaggotStateDefault(rigidbodyMovement, moveSettings, weaponGO, mainSprite, mainAnimator, capsuleCollider);
                        break;
                    case MaggotState.InAir:
                        _stateBehaviour = new MaggotStateInAir(rigidbodyMovement, moveSettings, weaponGO, mainSprite, mainAnimator);
                        break;
                    case MaggotState.Shooting:
                        _stateBehaviour = new MaggotStateShooting(rigidbodyMovement, moveSettings, weaponGO, mainSprite, mainAnimator);
                        break;                    
                }
                currentSwitchDelay = switchToAirStateDelay;
            }
        }

        private bool hasAction;
        private bool blockUsingNewWeapon;

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

        private void OnDisable()
        {
            Inventory.OnChangeWeapon -= SelectWeapon;
        }

        private void UpdateState()
        {
            if (State == MaggotState.Shooting)
            {
                return;
            }

            if (!rigidbodyMovement.IsStayOnGround && State == MaggotState.Default)
            {
                if (currentSwitchDelay > 0f)
                {
                    currentSwitchDelay -= Time.deltaTime;
                }
                else
                {
                    State = MaggotState.InAir;                    
                }               
            }

            if (rigidbodyMovement.IsStayOnGround && State == MaggotState.InAir)
            {
                State = MaggotState.Default;
            }
        }

        public void Init(ArenaController gameController, Team team)
        {
            gameController.OnChangeSelectedMaggot += OnChangeSelection;
            stats.OnZeroLife += OnZeroLife;
            stats.MaxLife = team.HealthPerCharacter;
            stats.CurrentLife = stats.MaxLife;
            stats.OnChangeLife += ChangeLife;
            State = default;
            Team = team;
            
            Inventory.OnChangeWeapon += SelectWeapon;
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
            if (!weaponGO.IsReady)
            {
                return;
            }
            hasAction = true;
            if(!blockUsingNewWeapon)
                _stateBehaviour.UseWeapon(OnStartChargingWeapon, OnEndChargingWeapon, OnStartUsingWeapon, OnEndUsingWeapon, OnUpdateChargeWeapon);
        }

        public void ReleaseFire()
        {
            _stateBehaviour.ReleaseFire();
        }

        public void UpdateWeaponDirection(Vector2 direction)
        {
            if (!weaponGO.IsReady)
            {
                return;
            }
            _stateBehaviour.UpdateWeaponDirection(direction);
        }

        public void OnExplosion(Vector2 pointOfExplosion, Weapon source)
        {
            stats.CurrentLife -= source.Damage;
            rigidbodyMovement.MoveByDirection(((Vector2)transform.position - pointOfExplosion).normalized, Space.Self, source.Damage * source.ForcePerDamage, ForceMode2D.Impulse);
        }

        private void SelectWeapon(Weapon weapon)
        {
            if (Team.CurrentMaggot() != this)
            {
                return;
            }
            if (weapon == null)
            {
                weaponGO.RemoveWeapon();
            }
            else
            {
                weaponGO.SwitchWeapon(weapon);
            }
            
        }

        private void OnStartChargingWeapon()
        {
            State = MaggotState.Shooting;
            weaponGO.onStartCharging -= OnStartChargingWeapon;
            Inventory.RemoveWeapon(Inventory.SelectedWeapon, 1);
        }
        
        private void OnEndChargingWeapon()
        {
            State = MaggotState.Default;
            weaponGO.onEndCharging -= OnEndChargingWeapon;
            weaponGO.onChargeWeapon -= OnUpdateChargeWeapon;
            OnChargeWeapon.Invoke(this, 0f);
        }

        private void OnStartUsingWeapon()
        {
            blockUsingNewWeapon = true;
            weaponGO.onUsing -= OnStartUsingWeapon;
        }

        private void OnUpdateChargeWeapon(float progress)
        {
            OnChargeWeapon.Invoke(this, progress);
        }

        private void OnEndUsingWeapon(bool endTurn)
        {
            blockUsingNewWeapon = false;
            weaponGO.onEndUsing -= OnEndUsingWeapon;
            weaponGO.RemoveWeapon();
            if (endTurn)
            {
                OnEndTurn?.Invoke(this);
            }
        }

        private void OnChangeSelection(Maggot maggot)
        {
            if (maggot != this)
            {
                weaponGO.RemoveWeapon();
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

