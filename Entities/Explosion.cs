using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class Explosion : MonoBehaviour
    {
        public void Explode(Team team, float _attack)
        {
            HashSet<Entity> alreadyHit = new();
            foreach (var col in Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Entity")))
            {
                var e = col.attachedRigidbody.GetComponent<Entity>();
                if (e != null && e.Team != team && !alreadyHit.Contains(e))
                {
                    e.Damage(_attack);
                    alreadyHit.Add(e);
                }
            }
            Destroy(gameObject, 0.5f);
        }
    }
}
