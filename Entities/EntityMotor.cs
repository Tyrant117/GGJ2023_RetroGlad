using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class EntityMotor : MonoBehaviour
    {
        public Vector3 Direction => _entity.IsNPC ? _ai.Direction : _input.Direction;
        public bool Charging => _charging;
        public bool Frozen { get; set; }

        //public float speedMod = 0.0f;


        [SerializeField]
        private float _speed = 1f;

        private float _modifiedSpeed;
        private bool _hasInput;
        private Entity _entity;
        private EntityInput _input;
        private EntityAI _ai;
        private Rigidbody2D _rb;

        private bool _charging;
        private Vector3 _chargeDir;

        private void Awake()
        {
            _entity = GetComponent<Entity>();
            _rb = GetComponent<Rigidbody2D>();
            _input = GetComponent<EntityInput>();
            _ai = GetComponent<EntityAI>();
            
        }

        private void FixedUpdate()
        {
            if (Frozen)
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            if (_charging)
            {
                _rb.velocity = _chargeDir;
                return;
            }

            if (!_entity.IsNPC)
            {
                _rb.velocity = _input.Direction * _speed;
            }
            else
            {
                _rb.velocity = _ai.Direction * _speed;
            }
        }

        public void StartCharge()
        {
            _charging = true;
            _chargeDir = Direction * _speed * 10;
        }

        public void EndCharge()
        {
            _charging = false;
        }

        public void ChangeSpeed(float speedBonus)
        {
            _modifiedSpeed = _speed * (1.0f + speedBonus);
        }

        public void ModifyMass(float mass)
        {
            if (!_rb)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
            _rb.mass = mass;
        }
    }
}
