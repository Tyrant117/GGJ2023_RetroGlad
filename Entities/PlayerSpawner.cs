using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _classes = new();

        [Header("Debug"), SerializeField]
        private bool _debugSpawn;
        [SerializeField]
        private Class _debugClass;
        [SerializeField]
        private Team _debugTeam;

        private void Start()
        {
            if (_debugSpawn)
            {
                Spawn(_debugClass, _debugTeam);
            }
            else
            {
                Spawn(GameManager.Instance.Company.Squad[0].Class, Team.Blue);
            }
        }

        public void Spawn(Class c, Team t)
        {
            foreach (var go in _classes)
            {
                if (go.name == c.ToString())
                {
                    var player = GameObject.Instantiate(go, transform.position, Quaternion.identity);
                    player.GetComponent<Entity>().Spawn(true, t);
                    break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}
