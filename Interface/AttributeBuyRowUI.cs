using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RetroGlad
{
    public class AttributeBuyRowUI : MonoBehaviour
    {
        [SerializeField]
        private Button _minus;
        [SerializeField]
        private Button _plus;
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private CharacterAttribute _attribute;

        private MercenaryBuyUI _ui;

        private void Awake()
        {
            _minus.onClick.AddListener(MinusClicked);
            _plus.onClick.AddListener(PlusClicked);
        }

        public void Setup(MercenaryBuyUI ui)
        {
            _ui = ui;
        }

        private void MinusClicked()
        {
            _ui.ModifyAttribute(_attribute, -1);
            _text.text = _ui.GetAttributeFormatted(_attribute);
        }

        private void PlusClicked()
        {
            _ui.ModifyAttribute(_attribute, 1);
            _text.text = _ui.GetAttributeFormatted(_attribute);
        }

        public void Format()
        {
            _text.text = _ui.GetAttributeFormatted(_attribute);
        }
    }
}
