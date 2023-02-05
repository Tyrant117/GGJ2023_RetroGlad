using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        [SerializeField, Tooltip("")]
        private ResourceBarUI _healthBar;
        [SerializeField, Tooltip("")]
        private ResourceBarUI _manaBar;
        [SerializeField, Tooltip("")]
        private SpellReadyBarUI _readyBar;
        [SerializeField]
        private SpriteRenderer _freezeTimeRenderer;
        [SerializeField]
        private AudioSource _audio;

        public static GameManager Instance { get; private set; }
        public CompanyManager Company { get; private set; }
        public bool TimeFrozen { get; private set; }
        public ResourceBarUI HealthBar
        {
            get { return _healthBar; }
            set { _healthBar = value; }
        }
        public ResourceBarUI ManaBar
        {
            get { return _manaBar; }
            set { _manaBar = value; }
        }
        public SpellReadyBarUI ReadyBar
        {
            get { return _readyBar; }
            set { _readyBar = value; }
        }

        private readonly HashSet<Entity> _activeEntities = new();
        private readonly HashSet<Projectile> _activeProjectiles = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            Instance = null;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            Company = CompanyManager.Load();
        }

        private void OnDestroy()
        {
            CompanyManager.Save(Company);
            _activeEntities.Clear();
            _activeProjectiles.Clear();
            Instance = null;
        }

        public void RegisterEntity(Entity e)
        {
            _activeEntities.Add(e);
        }
        public void UnregisterEntity(Entity e)
        {
            _activeEntities.Remove(e);
        }

        public void RegisterProjectile(Projectile p)
        {
            _activeProjectiles.Add(p);
        }
        public void UnregisterProjectile(Projectile p)
        {
            _activeProjectiles.Remove(p);
        }

        public void FreezeTime(Entity sender)
        {
            foreach (var e in _activeEntities)
            {
                if(sender == e) { continue; }
                e.Freeze();
            }

            foreach (var p in _activeProjectiles)
            {
                p.Freeze();
            }
            TimeFrozen = true;
            _freezeTimeRenderer.enabled = true;
        }

        public void UnFreezeTime(Entity sender)
        {
            foreach (var e in _activeEntities)
            {
                if (sender == e) { continue; }
                e.UnFreeze();
            }

            foreach (var p in _activeProjectiles)
            {
                p.UnFreeze();
            }
            TimeFrozen = false;
            _freezeTimeRenderer.enabled = false;
        }

        public void PlaySound(AudioClip clip)
        {
            _audio.PlayOneShot(clip, 0.33f);
        }
    }
}
