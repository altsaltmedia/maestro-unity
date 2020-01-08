using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Production Settings")]
    public class ProductionSettings : ScriptableObject
    {
        [SerializeField]
        private bool _hasBeenOpened = false;

        public bool hasBeenOpened
        {
            get => _hasBeenOpened;
            private set => hasBeenOpened = value;
        }
        
        [SerializeField]
        private bool _autoplayActive = true;

        public bool autoplayActive
        {
            get => _autoplayActive;
            private set => autoplayActive = value;
        }
        
        [SerializeField]
        private bool _momentumActive = true;

        public bool momentumActive
        {
            get => _momentumActive;
            private set => momentumActive = value;
        }
        
        [SerializeField]
        private bool _musicActive = true;

        public bool musicActive
        {
            get => _musicActive;
            private set => musicActive = value;
        }
        
        [SerializeField]
        private bool _soundEffectsActive = true;

        public bool soundEffectsActive
        {
            get => _soundEffectsActive;
            private set => soundEffectsActive = value;
        }

        [SerializeField]
        private bool _paused = false;

        public bool paused {
            get => _paused;
            private set => _paused = value;
        }
        
        [SerializeField]
        public float _timescale = 1f;
        
        public float timescale
        {
            get => _timescale;
            private set => _timescale = value;
        }
    }
}