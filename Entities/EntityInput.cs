using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class EntityInput : MonoBehaviour
    {
        public Vector3 Direction { get; private set; }

        public event Action AutoAttackClicked;
        public event Action<bool> SpellClicked;

        public event Action SpellOneClicked;
        public event Action SpellTwoClicked;
        public event Action SpellThreeClicked;


        private void Update()
        {
            var dir = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
            {
                dir += Vector2.up;
            }

            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector2.left;
            }

            if (Input.GetKey(KeyCode.S))
            {
                dir += Vector2.down;
            }

            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector2.right;
            }

            Direction = new Vector3(dir.x, dir.y, 0).normalized;

            if (Input.GetMouseButton(0))
            {
                AutoAttackClicked?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                SpellClicked?.Invoke(true);
            }

            if (Input.GetMouseButtonUp(1))
            {
                SpellClicked?.Invoke(false);
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                SpellOneClicked?.Invoke();
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                SpellTwoClicked?.Invoke();
            }

            if (Input.GetKey(KeyCode.Alpha3))
            {
                SpellThreeClicked?.Invoke();
            }
        }
    }
}
