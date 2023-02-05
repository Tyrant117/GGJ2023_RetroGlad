using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RetroGlad
{
    public class MercenaryBuyUI : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _selectGroup;
        [SerializeField]
        private CanvasGroup _statGroup;

        [Header("Select")]
        [SerializeField]
        private List<Sprite> _mercSprites = new();
        [SerializeField]
        private Image _mercImage;
        [SerializeField]
        private Button _nextButton;
        [SerializeField]
        private Button _prevButton;
        [SerializeField]
        private Button _continueButton;

        [Header("Stat")]
        [SerializeField]
        private TMP_Text _goldText;
        [SerializeField]
        private TMP_Text _costText;
        [SerializeField]
        private Button _buyButton;
        [SerializeField]
        private Button _cancelButton;
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private GameObject _attRow;


        private int _currentSelectIndex;
        private CompanyManager _manager;
        private int _nextLevel;

        public float PendingCost;
        public float PendingLevel;
        public float PendingStrength; // Attack Damage
        public float PendingDexterity; // Attack Speed, Movement Speed
        public float PendingConstitution; // HP
        public float PendingPhysique; // Mass and HP5
        public float PendingIntelligence; // Mana and Spell Damage
        public float PendingWisdom; // MP5 and Spell Cooldown Reduction

        private void Awake()
        {
            _nextButton.onClick.AddListener(NextButtonClick);
            _prevButton.onClick.AddListener(PrevButtonClick);
            _continueButton.onClick.AddListener(ContinueButtonClick);

            _buyButton.onClick.AddListener(TryBuyUpgrades);
            _cancelButton.onClick.AddListener(ClearUpgrades);
            _playButton.onClick.AddListener(StartGame);

            foreach (var row in _attRow.GetComponentsInChildren<AttributeBuyRowUI>())
            {
                row.Setup(this);
            }
        }

        private void OnDestroy()
        {
            _nextButton.onClick.RemoveAllListeners();
            _prevButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();

            _buyButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            _playButton.onClick.RemoveAllListeners();
        }

        public void LoadNew(CompanyManager manager)
        {
            _manager = manager;
            _nextLevel = 1;
            SetSelectWindow(true);
            SetStatWindow(false);
        }

        public void LoadContinue(int nextLevel)
        {
            _manager = CompanyManager.Load();
            _nextLevel = nextLevel;

            PendingLevel = _manager.Squad[0].Level;
            PendingStrength = _manager.Squad[0].Strength;
            PendingDexterity = _manager.Squad[0].Dexterity;
            PendingConstitution = _manager.Squad[0].Constitution;
            PendingPhysique = _manager.Squad[0].Physique;
            PendingIntelligence = _manager.Squad[0].Intelligence;
            PendingWisdom = _manager.Squad[0].Wisdom;
            PendingCost = 0;
            _goldText.text = $"Gold: {_manager.Gold:N0}";
            _costText.text = $"Cost: {PendingCost:N0}";

            foreach (var row in _attRow.GetComponentsInChildren<AttributeBuyRowUI>())
            {
                row.Format();
            }
            SetSelectWindow(false);
            SetStatWindow(true);
        }

        private void NextButtonClick()
        {
            _currentSelectIndex++;
            if(_currentSelectIndex >= _mercSprites.Count)
            {
                _currentSelectIndex = 0;
            }
            _mercImage.sprite = _mercSprites[_currentSelectIndex];
        }

        private void PrevButtonClick()
        {
            _currentSelectIndex--;
            if (_currentSelectIndex < 0)
            {
                _currentSelectIndex = _mercSprites.Count - 1;
            }
            _mercImage.sprite = _mercSprites[_currentSelectIndex];
        }

        private void ContinueButtonClick()
        {
            _manager.Squad.Add(new SquadMember(new MemberSaveData()
            {
                Name = ((Class)_currentSelectIndex).ToString(),
                Class = (Class)_currentSelectIndex,
                Level = 1,
                Strength = 1,
                Dexterity = 1,
                Constitution = 1,
                Physique = 1,
                Intelligence = 1,
                Wisdom = 1,
            }));
            CompanyManager.Save(_manager);            

            PendingLevel = _manager.Squad[0].Level;
            PendingStrength = _manager.Squad[0].Strength;
            PendingDexterity = _manager.Squad[0].Dexterity;
            PendingConstitution = _manager.Squad[0].Constitution;
            PendingPhysique = _manager.Squad[0].Physique;
            PendingIntelligence = _manager.Squad[0].Intelligence;
            PendingWisdom = _manager.Squad[0].Wisdom;
            PendingCost = 0;
            _goldText.text = $"Gold: {_manager.Gold:N0}";
            _costText.text = $"Cost: {PendingCost:N0}";
            foreach (var row in _attRow.GetComponentsInChildren<AttributeBuyRowUI>())
            {
                row.Format();
            }
            SetSelectWindow(false);
            SetStatWindow(true);
        }

        private void SetSelectWindow(bool show)
        {
            if (show)
            {
                _selectGroup.alpha = 1;
                _selectGroup.interactable = true;
                _selectGroup.blocksRaycasts = true;
            }
            else
            {
                _selectGroup.alpha = 0;
                _selectGroup.interactable = false;
                _selectGroup.blocksRaycasts = false;
            }
        }

        private void SetStatWindow(bool show)
        {
            if (show)
            {
                _statGroup.alpha = 1;
                _statGroup.interactable = true;
                _statGroup.blocksRaycasts = true;
            }
            else
            {
                _statGroup.alpha = 0;
                _statGroup.interactable = false;
                _statGroup.blocksRaycasts = false;
            }
        }

        public void ModifyAttribute(CharacterAttribute attribute, int delta)
        {
            switch (attribute)
            {
                case CharacterAttribute.Level:
                    PendingLevel = Mathf.Max(_manager.Squad[0].Level, PendingLevel + delta);
                    break;
                case CharacterAttribute.Strength:
                    PendingStrength = Mathf.Max(_manager.Squad[0].Strength, PendingStrength + delta);
                    break;
                case CharacterAttribute.Dexterity:
                    PendingDexterity = Mathf.Max(_manager.Squad[0].Dexterity, PendingDexterity + delta);
                    break;
                case CharacterAttribute.Consitution:
                    PendingConstitution = Mathf.Max(_manager.Squad[0].Constitution, PendingConstitution + delta);
                    break;
                case CharacterAttribute.Physique:
                    PendingPhysique = Mathf.Max(_manager.Squad[0].Physique, PendingPhysique + delta);
                    break;
                case CharacterAttribute.Intelligence:
                    PendingIntelligence = Mathf.Max(_manager.Squad[0].Intelligence, PendingIntelligence + delta);
                    break;
                case CharacterAttribute.Wisdom:
                    PendingWisdom = Mathf.Max(_manager.Squad[0].Wisdom, PendingWisdom + delta);
                    break;
            }
            PendingCost = 0;
            PendingCost += GetCost(_manager.Squad[0].Level, PendingLevel);
            PendingCost += GetCost(_manager.Squad[0].Strength, PendingStrength);
            PendingCost += GetCost(_manager.Squad[0].Dexterity, PendingDexterity);
            PendingCost += GetCost(_manager.Squad[0].Constitution, PendingConstitution);
            PendingCost += GetCost(_manager.Squad[0].Physique, PendingPhysique);
            PendingCost += GetCost(_manager.Squad[0].Intelligence, PendingIntelligence);
            PendingCost += GetCost(_manager.Squad[0].Wisdom, PendingWisdom);

            _costText.text = $"Cost: {PendingCost:N0}";
        }

        public float GetCost(float baseVal, float currentVal)
        {
            if(currentVal == baseVal) { return 0; }

            float b = 10 * baseVal;
            return Mathf.Floor(b + Mathf.Pow(5, 1 + (currentVal - baseVal) * 0.1f));
        }

        public void TryBuyUpgrades()
        {
            if(_manager.Gold >= PendingCost)
            {
                _manager.Squad[0].Level = PendingLevel;
                _manager.Squad[0].Strength = PendingStrength;
                _manager.Squad[0].Dexterity = PendingDexterity;
                _manager.Squad[0].Constitution = PendingConstitution;
                _manager.Squad[0].Physique = PendingPhysique;
                _manager.Squad[0].Intelligence = PendingIntelligence;
                _manager.Squad[0].Wisdom = PendingWisdom;
                _manager.Gold -= PendingCost;
                PendingCost = 0;
                _goldText.text = $"Gold: {_manager.Gold:N0}";
                _costText.text = $"Cost: {PendingCost:N0}";
            }
            CompanyManager.Save(_manager);
        }

        public void ClearUpgrades()
        {
            PendingLevel = _manager.Squad[0].Level;
            PendingStrength = _manager.Squad[0].Strength;
            PendingDexterity = _manager.Squad[0].Dexterity;
            PendingConstitution = _manager.Squad[0].Constitution;
            PendingPhysique = _manager.Squad[0].Physique;
            PendingIntelligence = _manager.Squad[0].Intelligence;
            PendingWisdom = _manager.Squad[0].Wisdom;
            PendingCost = 0;
            _costText.text = $"Cost: {PendingCost:N0}";
        }

        public string GetAttributeFormatted(CharacterAttribute attribute)
        {
            return attribute switch
            {
                CharacterAttribute.Level => $"Level: {PendingLevel}",
                CharacterAttribute.Strength => $"Str: {PendingStrength}",
                CharacterAttribute.Dexterity => $"Dex: {PendingDexterity}",
                CharacterAttribute.Consitution => $"Con: {PendingConstitution}",
                CharacterAttribute.Physique => $"Phy: {PendingPhysique}",
                CharacterAttribute.Intelligence => $"Int: {PendingIntelligence}",
                CharacterAttribute.Wisdom => $"Wis: {PendingWisdom}",
                _ => "",
            };
        }

        public void StartGame()
        {
            CompanyManager.Save(_manager);
            SceneManager.LoadScene($"Level {_nextLevel}");
        }
    }
}
