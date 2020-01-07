using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    public class ModuleReferences : ScriptableObject
    {
        [Required]
        [SerializeField]
        private GameObject _textPrefab;

        public GameObject textPrefab => _textPrefab;

        [Required]
        [SerializeField]
        private GameObject _spritePrefab;

        public GameObject spritePrefab => _spritePrefab;

        [Required]
        [SerializeField]
        private GameObject _containerPrefab;

        public GameObject containerPrefab => _containerPrefab;

        [Required]
        [SerializeField]
        private GameObject _responsiveContainerPrefab;

        public GameObject responsiveContainerPrefab => _responsiveContainerPrefab;

        [Required]
        [SerializeField]
        private GameObject _responsiveContainerLeftAnchorPrefab;

        public GameObject responsiveContainerLeftAnchorPrefab => _responsiveContainerLeftAnchorPrefab;

        [Required]
        [SerializeField]
        private GameObject _sequenceTouchApplier;

        public GameObject sequenceTouchApplier => _sequenceTouchApplier;

        [Required]
        [SerializeField]
        private GameObject _sequenceAutoplayer;

        public GameObject sequenceAutoplayer => _sequenceAutoplayer;

        [Required]
        [SerializeField]
        private GameObject _standardDirector;

        public GameObject standardDirector => _standardDirector;

        [Required]
        [SerializeField]
        private GameObject _swipeDirector;

        public GameObject swipeDirector => _swipeDirector;

        [Required]
        [SerializeField]
        private GameObject _scrollSnapController;

        public GameObject scrollSnapController
        {
            get => _scrollSnapController;
            set => _scrollSnapController = value;
        }

        [Required]
        [SerializeField]
        private GameObject _scrollSnapIcon;

        public GameObject scrollSnapIcon => _scrollSnapIcon;

        [Required]
        [SerializeField]
        private GameObject _rootConfig;

        public GameObject rootConfig => _rootConfig;
        
        [Required]
        [SerializeField]
        private GameObject _masterSequence;

        public GameObject masterSequence => _masterSequence;
        
        [Required]
        [SerializeField]
        private GameObject _sequenceConfig;

        public GameObject sequenceConfig => _sequenceConfig;
    }
}