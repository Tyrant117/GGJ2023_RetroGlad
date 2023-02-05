using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RetroGlad
{
    public class SpellReadyBarUI : MonoBehaviour
    {
        [SerializeField]
        private Image _fill;

        [SerializeField]
        private Color _notReady = Color.white;
        [SerializeField]
        private Color _ready = Color.white;

        public void SetFill(float fill)
        {
            fill = Mathf.Clamp01(fill);
            _fill.fillAmount = fill;
            if(fill == 1)
            {
                _fill.color = _ready;
            }
            else
            {
                _fill.color = _notReady;
            }
        }
    }
}
