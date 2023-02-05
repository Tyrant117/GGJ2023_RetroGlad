using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroGlad
{
    public class ResourceBarUI : MonoBehaviour
    {
        public enum Display
        {
            Value,
            Percent,
            Both
        }

        [SerializeField]
        private Image _fill;
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private Display _display;

        public void SetFill(float current, float fill)
        {
            fill = Mathf.Clamp01(fill);
            _fill.fillAmount = fill;
            _text.text = _display switch
            {
                Display.Value => $"{current:N0}",
                Display.Percent => $"{fill:P0}",
                Display.Both => $"{current:N0} / {fill:P0}",
                _ => $"{current:N0}",
            };
        }
    }
}
