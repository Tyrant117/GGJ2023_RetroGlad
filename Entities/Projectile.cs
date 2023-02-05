using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;
        [SerializeField]
        private Animation _anim;
        [SerializeField]
        private GameObject _destroyedPrefab;

        [SerializeField]
        private float _speed;

        private Team _team;
        private bool _collided;
        private bool _frozen;
        private bool _ignoreFixedUpdate;
        private float _attack;


        private void OnValidate()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            GameManager.Instance.RegisterProjectile(this);
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UnregisterProjectile(this);
            }
        }

        public void Fire(Vector3 dir, Team team, bool frozen, float attack)
        {
            _attack = attack;
            transform.up = dir;
            _team = team;
            if (frozen)
            {
                Freeze();
            }
        }

        public void PlaceBomb(float life, Team team, bool frozen, float attack)
        {
            _attack = attack;
            _team = team;
            _ignoreFixedUpdate = true;
            if (frozen)
            {
                Freeze();
            }
            StartCoroutine(Explode(life));
        }

        private IEnumerator Explode(float life)
        {
            while (life > 0)
            {
                if (!_frozen)
                {
                    life -= Time.deltaTime;
                }
                yield return null;
            }
            var go = GameObject.Instantiate(_destroyedPrefab, transform.position, transform.rotation);
            go.GetComponent<Explosion>().Explode(_team, _attack);
            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            if (_ignoreFixedUpdate) { return; }
            if (_frozen) { _rb.velocity = Vector2.zero; return; }
            _rb.velocity = transform.up * _speed;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_frozen) { return; }

            var e = collider.attachedRigidbody.GetComponent<Entity>();
            if (e != null && e.Team != _team && !_collided)
            {
                _collided = true;
                e.Damage(_attack);
                Destroy(gameObject);
            }
        }

        public void Freeze()
        {
            _frozen = true;
            if (_anim)
            {
                _anim.Stop();
            }
        }

        public void UnFreeze()
        {
            _frozen = false;
            if (_anim)
            {
                _anim.Play();
            }
        }
    }
}
