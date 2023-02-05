using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class FixedVelocity : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;
        [SerializeField]
        private Vector3 _velocity;

        private void OnValidate()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = _velocity;
        }
    }
}
