using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroGlad
{
    public class Entity : MonoBehaviour, iPickerUpper
    {
        [SerializeField]
        private Team _team;
        [SerializeField]
        private Class _class;
        [SerializeField]
        private FloatingEquipment _weapon;

        [Header("Projectiles"), SerializeField]
        private GameObject _fireball;
        [SerializeField]
        private GameObject _bomb;
        [SerializeField]
        private GameObject _arrow;
        [SerializeField]
        private GameObject _barrage;

        private StatSheet _statSheet = new();


        

        private float _gladAttack
        {
            get
            {
                return 1.0f + (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Strength));
            }
            set { _gladAttack = value; }
        } 
        private float _thiefAttack
        {
            get
            {
                return 1.0f + (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Strength));
            }
            set { _thiefAttack = value; }
        }
        private float _rangedAttack
        {
            get
            {
                return 1.0f + (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Strength));
            }
            set { _rangedAttack = value; }
        }
        private float _mageAttack
        {
            get
            {
                return 1.0f + (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Strength));
            }
            set { _mageAttack = value; }
        }
        private float _barrageAttack
        {
            get
            {
                return 4f + (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Intelligence));
            }
            set { _barrageAttack = value; }
        }
        private float _chargeAttack
        {
            get
            {
                return 5f + (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Intelligence));
            }
            set { _chargeAttack = value; }
        }
        private float _bombAttack
        {
            get
            {
                return 10f + (0.5f * _statSheet.GetComputedStat(CharacterAttribute.Intelligence));
            }
            set { _bombAttack = value; }
        }
        private float _mageStop
        {
            get
            {
                return 1.0f + (0.25f * _statSheet.GetComputedStat(CharacterAttribute.Intelligence));
            }
            set { _mageStop = value; }
        }

        private float _wisdomDelay
        {
            get
            {
                return 1.0f + (0.15f * _statSheet.GetComputedStat(CharacterAttribute.Wisdom));
            }
            set { _wisdomDelay = value; }
        }
        private float _wisdomRegen
        {
            get
            {
                return (0.1f * _statSheet.GetComputedStat(CharacterAttribute.Wisdom));
            }
        }

        public bool IsNPC { get; private set; } = true;
        public Team Team => _team;
        public Class Class => _class;
        public FloatingEquipment Weapon => _weapon;
        public EntityMotor Motor { get; private set; }

        private EntityInput _input;
        private EntityAI _ai;
        private SpriteRenderer _renderer;

        private float _health;
        private float _maxHealth;
        private float _hp5;
        private float _mana;
        private float _maxMana;
        private float _mp5;

        private float _autoAttackDelay;
        private float _currentAutoAttackDelay;

        private float _spellManaCost;
        private float _spellDelay;
        private float _currentSpellDelay;
        private float _regenTick;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _input = GetComponent<EntityInput>();
            _ai = GetComponent<EntityAI>();
            Motor = GetComponent<EntityMotor>();

            if (Team != Team.Blue)
            {
                Spawn(false, Team);
            }
        }

        public void Spawn(bool isPlayer, Team team)
        {
            _team = team;
            IsNPC = !isPlayer;
            if (!IsNPC)
            {
                FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;
                _input.AutoAttackClicked += OnAutoAttackClicked;
                _input.SpellClicked += OnSpellClicked;
                _input.SpellOneClicked += OnSpellOneClicked;
                _input.SpellTwoClicked += OnSpellTwoClicked;
                _input.SpellThreeClicked += OnSpellThreeClicked;
                _statSheet.Load(GameManager.Instance.Company.Squad[0]);
            }
            else
            {
                LoadStats();
            }
            StatUpdates();
        }

        private void Start()
        {            
            if (!IsNPC)
            {
                GameManager.Instance.HealthBar.SetFill(_health, _health / _maxHealth);
                GameManager.Instance.ManaBar.SetFill(_mana, _mana / _maxMana);
                GameManager.Instance.ReadyBar.SetFill(Mathf.InverseLerp(_spellDelay, 0, _currentSpellDelay));
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.RegisterEntity(this);            
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnregisterEntity(this);
            }
            if (!IsNPC)
            {
                _input.AutoAttackClicked -= OnAutoAttackClicked;
                _input.SpellClicked -= OnSpellClicked;
                _input.SpellOneClicked -= OnSpellOneClicked;
                _input.SpellTwoClicked -= OnSpellTwoClicked;
                _input.SpellThreeClicked -= OnSpellThreeClicked;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var e = collision.rigidbody.GetComponent<Entity>();
            if (e != null && e.Team != Team && Motor.Charging)
            {
                e.Damage(_chargeAttack);
            }
        }

        private void Update()
        {
            _regenTick += Time.deltaTime;
            _currentAutoAttackDelay -= Time.deltaTime;
            _currentSpellDelay -= Time.deltaTime;
            GameManager.Instance.ReadyBar.SetFill(Mathf.InverseLerp(_spellDelay, 0, _currentSpellDelay));
            if(_regenTick >= 5)
            {
                _health = Mathf.Min(_health + _hp5, _maxHealth);
                _mana = Mathf.Min(_mana + _mp5, _maxMana);
                if (!IsNPC)
                {
                    GameManager.Instance.HealthBar.SetFill(_health, _health / _maxHealth);
                    GameManager.Instance.ManaBar.SetFill(_mana, _mana / _maxMana);
                }
                _regenTick = 0;
            }
        }

        private void LoadStats()
        {
            //_statSheet = new StatSheet();
            _statSheet.RandStatsBoost(_ai.Level);
        }
        private void StatUpdates()
        {
            _health = 20 + _statSheet.GetComputedStat(CharacterAttribute.Consitution);
            _maxHealth = 20 + _statSheet.GetComputedStat(CharacterAttribute.Consitution);
            _hp5 = 0 + (_statSheet.GetComputedStat(CharacterAttribute.Physique) * 0.5f);
            _mana = 10 + (_statSheet.GetComputedStat(CharacterAttribute.Intelligence) * 1.25f);
            _maxMana = 10 + (_statSheet.GetComputedStat(CharacterAttribute.Intelligence) * 1.25f);

            switch (Class)
            {
                case Class.Gladiator:
                    _autoAttackDelay = 0.8f / (1 + (0.15f * _statSheet.GetComputedStat(CharacterAttribute.Dexterity)));
                    _spellManaCost = 1f;
                    _spellDelay = 1f / _wisdomDelay;
                    _mp5 = 1 + (_wisdomRegen * 0.8f);
                    break;
                case Class.Mage:
                    _autoAttackDelay = 1.4f / (1 + (0.15f * _statSheet.GetComputedStat(CharacterAttribute.Dexterity)));
                    _spellManaCost = 10f;
                    _spellDelay = 30f / _wisdomDelay;
                    _mp5 = 4 + (_wisdomRegen * 1.2f);
                    break;
                case Class.Rogue:
                    _autoAttackDelay = 0.6f / (1 + (0.15f * _statSheet.GetComputedStat(CharacterAttribute.Dexterity)));
                    _spellManaCost = 3.5f;
                    _spellDelay = 2f / _wisdomDelay;
                    _mp5 = 2 + _wisdomRegen;
                    break;
                case Class.Archer:
                    _autoAttackDelay = 1.2f / (1 + (0.15f * _statSheet.GetComputedStat(CharacterAttribute.Dexterity)));
                    _spellManaCost = 5f;
                    _spellDelay = 3f / _wisdomDelay;
                    _mp5 = 2 + _wisdomRegen;
                    break;
            }
            _mp5 += (_statSheet.GetComputedStat(CharacterAttribute.Wisdom) * .25f);

            Motor.ChangeSpeed(_statSheet.GetComputedStat(CharacterAttribute.Dexterity) * 0.05f);
            Motor.ModifyMass(_statSheet.GetComputedStat(CharacterAttribute.Physique) * 0.25f);
        }

        #region - Input -
        public void OnAutoAttackClicked()
        {
            if (_currentAutoAttackDelay > 0) { return; }

            switch (Class)
            {
                case Class.Gladiator:
                    _OnWeaponSwing(1f, _gladAttack);
                    break;
                case Class.Mage:
                    _OnFireball();
                    break;
                case Class.Rogue:
                    _OnWeaponSwing(0.5f, _thiefAttack);
                    break;
                case Class.Archer:
                    _OnArrow();
                    break;
                default:
                    break;
            }
            _currentAutoAttackDelay = _autoAttackDelay;

            void _OnWeaponSwing(float radius, float attack)
            {
                HashSet<Entity> alreadyHit = new();
                foreach (var col in Physics2D.OverlapCircleAll(_weapon.transform.position, radius, LayerMask.GetMask("Entity")))
                {
                    var e = col.attachedRigidbody.GetComponent<Entity>();
                    if (e != null && e.Team != Team && !alreadyHit.Contains(e))
                    {
                        e.Damage(attack);
                        alreadyHit.Add(e);
                    }
                }
            }

            void _OnFireball()
            {
                var go = GameObject.Instantiate(_fireball, _weapon.transform.position, Quaternion.identity);
                go.GetComponent<Projectile>().Fire(_weapon.Direction, Team, GameManager.Instance.TimeFrozen, _mageAttack);
            }

            void _OnArrow()
            {
                var go = GameObject.Instantiate(_arrow, _weapon.transform.position, Quaternion.identity);
                go.GetComponent<Projectile>().Fire(_weapon.Direction, Team, GameManager.Instance.TimeFrozen, _rangedAttack);
            }
        }

        public void OnSpellClicked(bool state)
        {
            if (state)
            {
                if (_currentSpellDelay > 0) { return; }
                if (_spellManaCost > _mana) { return; }
            }

            switch (Class)
            {
                case Class.Gladiator:
                    OnCharge(state);
                    break;
                case Class.Mage:
                    OnFreezeTime(state);
                    break;
                case Class.Rogue:
                    OnDropBomb(state);
                    break;
                case Class.Archer:
                    OnArrowBarrage(state, false, Vector3.zero);
                    break;
            }

            if (state)
            {
                _mana -= _spellManaCost;
                _currentSpellDelay = _spellDelay;
                if (!IsNPC)
                {
                    GameManager.Instance.ManaBar.SetFill(_mana, _mana / _maxMana);
                }
            }
        }        

        private void OnSpellOneClicked()
        {
        }

        private void OnSpellTwoClicked()
        {
        }

        private void OnSpellThreeClicked()
        {
        }
        #endregion

        #region - Effects -
        private void OnCharge(bool state)
        {
            if (state)
            {
                Motor.StartCharge();
            }
            else
            {
                Motor.EndCharge();
            }

        }

        private void OnFreezeTime(bool state)
        {
            if (state)
            {
                if (GameManager.Instance.TimeFrozen) { return; }
                StartCoroutine(FreezeTimeCountdown());
            }
        }

        private IEnumerator FreezeTimeCountdown()
        {
            GameManager.Instance.FreezeTime(this);
            yield return new WaitForSeconds(1.0f + (_mageStop));
            GameManager.Instance.UnFreezeTime(this);
        }

        private void OnDropBomb(bool state)
        {
            if (state)
            {
                var go = GameObject.Instantiate(_bomb, transform.position, Quaternion.identity);
                go.GetComponent<Projectile>().PlaceBomb(2.5f, Team, GameManager.Instance.TimeFrozen, _bombAttack);
            }
        }

        public void OnArrowBarrage(bool state, bool fromNPC, Vector3 point)
        {
            if (state)
            {
                if (fromNPC)
                {
                    if (_currentSpellDelay > 0) { return; }
                    if (_spellManaCost > _mana) { return; }

                    var go = GameObject.Instantiate(_barrage, new(point.x, point.y, 0), Quaternion.identity);
                    go.GetComponent<AttackMarker>().Place(Team, _barrageAttack);

                    _mana -= _spellManaCost;
                    _currentSpellDelay = _spellDelay;
                }
                else
                {
                    var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var go = GameObject.Instantiate(_barrage, new(p.x, p.y, 0), Quaternion.identity);
                    go.GetComponent<AttackMarker>().Place(Team, _barrageAttack);
                }
            }
        }

        public void Damage(float value)
        {
            ModifyHealth(-value);
            StartCoroutine(Flash());
            if (_health <= 0)
            {
                Die();
            }
        }

        public void ModifyMana(float delta)
        {
            if (Mathf.Sign(delta) > 0)
            {
                _mana = Mathf.Min(_mana + delta, _maxMana);
                if (!IsNPC)
                {
                    GameManager.Instance.ManaBar.SetFill(_mana, _mana / _maxMana);
                }
            }
            else
            {
                _mana = Mathf.Max(_mana + delta, 0);
                if (!IsNPC)
                {
                    GameManager.Instance.ManaBar.SetFill(_mana, _mana / _maxMana);
                }
            }
        }

        public void ModifyHealth(float delta)
        {
            if (Mathf.Sign(delta) > 0)
            {
                _health = Mathf.Min(_health + delta, _maxHealth);
                if (!IsNPC)
                {
                    GameManager.Instance.HealthBar.SetFill(_health, _health / _maxHealth);
                }
            }
            else
            {
                _health = Mathf.Max(_health + delta, 0);
                if (!IsNPC)
                {
                    GameManager.Instance.HealthBar.SetFill(_health, _health / _maxHealth);
                }
            }
        }

        private IEnumerator Flash()
        {
            float flashTime = 0.5f;
            _renderer.material.SetFloat("_Flash", 1f);
            while (flashTime > 0)
            {
                flashTime -= Time.deltaTime;
                _renderer.material.SetFloat("_Flash", Mathf.InverseLerp(0, 0.5f, flashTime));
                yield return null;
            }
        }

        public void Die()
        {
            if (Team == Team.Blue)
            {
                GameManager.Instance.Company.ClearMercs();
                SceneManager.LoadScene("Main Menu Scene");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Freeze()
        {
            Motor.Frozen = true;
            _weapon.Frozen = true;
        }

        public void UnFreeze()
        {
            Motor.Frozen = false;
            _weapon.Frozen = false;
        }
        #endregion

        #region - Pickups -
        public void OnPickup(IPickUp pickup)
        {
            if (pickup is ManaCrystal mana)
            {
                ModifyMana(mana.CrystalValue);
                GameManager.Instance.PlaySound(mana.clip);
            }

            if (pickup is HealthPotion health)
            {
                ModifyHealth(health.HealthGain);
                GameManager.Instance.PlaySound(health.clip);
            }

            if (pickup is Gold gold && !IsNPC)
            {
                GameManager.Instance.Company.Gold += gold.Value;
                GameManager.Instance.PlaySound(gold.clip);
            }
        }
        #endregion
    }
}
