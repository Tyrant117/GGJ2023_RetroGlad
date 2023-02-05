using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroGlad
{
    public class MoveLevel : MonoBehaviour
    {
        public int NextLevel;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var rb = collider.attachedRigidbody;
            if (rb)
            {
                var e = rb.GetComponent<Entity>();
                if(e && e.Team == Team.Blue)
                {
                    CompanyManager.Save(GameManager.Instance.Company);
                    SceneManager.sceneLoaded += Loaded;
                    SceneManager.LoadScene("Mercenary Buy Scene");
                }
            }
        }

        private void Loaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= Loaded;
            var ui = FindObjectOfType<MercenaryBuyUI>();
            ui.LoadContinue(NextLevel);
        }
    }
}
