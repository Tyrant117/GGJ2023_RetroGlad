using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class AttackMarker : MonoBehaviour
    {
        [SerializeField]
        private GameObject _markerPrefab;
        [SerializeField]
        private GameObject _attackPrefab;
        [SerializeField]
        private List<Vector3> _offsets = new();

        [SerializeField]
        private float _delay = 3;

        private Team _team;
        private float _attack;
        private readonly List<GameObject> _markers = new();

        public void Place(Team team, float _barrageAttack)
        {
            _attack = _barrageAttack;
            _team = team;
            foreach (var offset in _offsets)
            {
                SpawnMarker(offset);
            }
            StartCoroutine(StartBarrage());
            StartCoroutine(FadeMarkers());
        }

        private IEnumerator FadeMarkers()
        {
            float fadeTime = 0.5f;
            while (fadeTime > 0)
            {
                foreach (var go in _markers)
                {
                    go.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.InverseLerp(0, 0.5f, fadeTime));
                }
                fadeTime -= Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator StartBarrage()
        {
            yield return new WaitForSeconds(_delay);
            foreach (var go in _markers)
            {                
                SpawnAttacks(go.transform.position + Vector3.up * 3);
            }
            StartCoroutine(EndBarrage(3f / 8f));
        }

        private IEnumerator EndBarrage(float endTime)
        {
            yield return new WaitForSeconds(endTime);
            foreach (var go in _markers)
            {
                Explode(go.transform.position);
            }
        }

        private void SpawnMarker(Vector3 offset)
        {
            var go = GameObject.Instantiate(_markerPrefab, transform.position + offset, Quaternion.identity);
            go.transform.SetParent(transform, true);
            _markers.Add(go);
        }

        private void SpawnAttacks(Vector3 spawn)
        {
            var go = GameObject.Instantiate(_attackPrefab, spawn, Quaternion.identity);
            Destroy(go, 3f / 8f);
        }

        private void Explode(Vector3 spawn)
        {
            HashSet<Entity> alreadyHit = new();
            foreach (var col in Physics2D.OverlapCircleAll(spawn, 1f, LayerMask.GetMask("Entity")))
            {
                var e = col.attachedRigidbody.GetComponent<Entity>();
                if (e != null && e.Team != _team && !alreadyHit.Contains(e))
                {
                    e.Damage(_attack);
                    alreadyHit.Add(e);
                }
            }
            Destroy(gameObject);
        }
    }
}
