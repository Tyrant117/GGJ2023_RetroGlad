using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class EntityAI : MonoBehaviour
    {
        public enum Smartness
        {
            Dumb,
            Average,
            Smart,
        }

        public Vector3 Direction { get; private set; }

        [SerializeField]
        private float _agroRange = 10;
        [SerializeField]
        private Smartness _smartness;
        [SerializeField]
        public float Level;
        
        

        [Header("Debug"), SerializeField]
        private Entity _debugTarget;

        private Rigidbody2D _rb;
        private Entity _entity;
        private Entity _target;

        private float _wanderTimer = 5;
        private float _wanderReset;
        private readonly Collider2D[] _nonAlloc = new Collider2D[100];

        private float _aiProcessInterval = 1f;
        private float _aiProcessTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _entity = GetComponent<Entity>();
        }

        private void Update()
        {
            if (!_entity.IsNPC) { return; }
            if (_entity.Motor.Frozen) { return; }

            if (!_target)
            {
                GetNearestEntity();
            }

            if (_target)
            {
                MoveToTarget();
                _aiProcessTimer += Time.deltaTime;
                if (_aiProcessTimer >= _aiProcessInterval)
                {
                    _aiProcessTimer = 0;
                    switch (_entity.Class)
                    {
                        case Class.Gladiator:
                            ProcessGladiatorAI();
                            break;
                        case Class.Mage:
                            ProcessMageAI();
                            break;
                        case Class.Rogue:
                            ProcessRogueAI();
                            break;
                        case Class.Archer:
                            ProcessArcherAI();
                            break;
                    }
                }
            }
            else
            {
                _wanderReset += Time.deltaTime;
                if (_wanderReset >= _wanderTimer)
                {
                    _wanderReset = 0;
                    WanderInDirection();
                }
            }

            _debugTarget = _target;
        }


        private void GetNearestEntity()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, _agroRange, _nonAlloc, LayerMask.GetMask("Entity"));
            if (count > 0)
            {
                float nearest = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (_nonAlloc[i].attachedRigidbody == _rb) { continue; }

                    var d = Vector3.Distance(transform.position, _nonAlloc[i].attachedRigidbody.transform.position);
                    if (d < nearest)
                    {
                        var t = _nonAlloc[i].attachedRigidbody.GetComponent<Entity>();
                        if (t.Team != _entity.Team)
                        {
                            _target = t;
                            nearest = d;
                        }
                    }
                }
            }
            else
            {
                _target = null;
            }
        }

        private void MoveToTarget()
        {
            var dir = _target.transform.position - transform.position;
            Direction = dir.normalized;
        }

        private void WanderInDirection()
        {
            var rot = Quaternion.Euler(new(0, Random.Range(0, 360), 0));
            var dir = rot * Vector3.forward;
            Direction = new Vector3(dir.x, dir.z, 0).normalized;
        }

        private void ProcessGladiatorAI()
        {
            float chance = 0;
            float abilityChance = 0;
            switch (_smartness)
            {
                case Smartness.Dumb:
                    chance = 0.3f;
                    abilityChance = 0.15f;
                    break;
                case Smartness.Average:
                    chance = 0.6f;
                    abilityChance = 0.4f;
                    break;
                case Smartness.Smart:
                    chance = 0.9f;
                    abilityChance = 0.7f;
                    break;
            }

            if (DistanceToTarget() <= 5f && Random.Range(0, 1f) <= abilityChance)
            {
                _entity.OnSpellClicked(true);
            }

            if (DistanceToTarget() <= 1f && Random.Range(0, 1f) <= chance)
            {
                _entity.OnAutoAttackClicked();
            }
        }

        private IEnumerator EndCharge()
        {
            yield return new WaitForSeconds(1.2f);
            _entity.OnSpellClicked(false);
        }

        private void ProcessMageAI()
        {
            float chance = 0;
            switch (_smartness)
            {
                case Smartness.Dumb:
                    chance = 0.1f;
                    break;
                case Smartness.Average:
                    chance = 0.5f;
                    break;
                case Smartness.Smart:
                    chance = 0.9f;
                    break;
            }

            if (DistanceToTarget() <= 5f && Random.Range(0, 1f) <= chance)
            {
                _entity.OnAutoAttackClicked();
            }
        }

        private void ProcessRogueAI()
        {
            float chance = 0;
            float abilityChance = 0.2f;
            switch (_smartness)
            {
                case Smartness.Dumb:
                    chance = 0.1f;
                    break;
                case Smartness.Average:
                    chance = 0.5f;
                    break;
                case Smartness.Smart:
                    chance = 0.9f;
                    break;
            }

            if (DistanceToTarget() <= 4f && Random.Range(0, 1f) <= abilityChance)
            {
                _entity.OnSpellClicked(true);
            }

            if (DistanceToTarget() <= 0.5f && Random.Range(0, 1f) <= chance)
            {
                _entity.OnAutoAttackClicked();
            }
        }

        private void ProcessArcherAI()
        {
            float chance = 0;
            float abilityChance = 0;
            switch (_smartness)
            {
                case Smartness.Dumb:
                    chance = 0.1f;
                    abilityChance = 0;
                    break;
                case Smartness.Average:
                    chance = 0.5f;
                    abilityChance = 0.1f;
                    break;
                case Smartness.Smart:
                    chance = 0.9f;
                    abilityChance = 0.3f;
                    break;
            }

            if (DistanceToTarget() <= 6f && Random.Range(0, 1f) <= abilityChance)
            {
                _entity.OnArrowBarrage(true, true, _target.transform.position);
            }

            if (DistanceToTarget() <= 8f && Random.Range(0, 1f) <= chance)
            {
                _entity.OnAutoAttackClicked();
            }
        }

        private float DistanceToTarget()
        {
            return Vector3.Distance(_target.transform.position, _entity.Weapon.transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _agroRange);
        }
    }
}
