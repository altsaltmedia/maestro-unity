using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

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
        private GameObject _simpleVideoPlayerPrefab;

        public GameObject simpleVideoPlayerPrefab => _simpleVideoPlayerPrefab;

        [Required]
        [SerializeField]
        private GameObject _scrollSnapControllerPrefab;

        public GameObject scrollSnapControllerPrefab => _scrollSnapControllerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _scrollSnapIcon;

        public GameObject scrollSnapIcon => _scrollSnapIcon;
        
        [Required]
        [SerializeField]
        private GameObject _dynamicAppLayoutControllerPrefab;

        public GameObject dynamicAppLayoutControllerPrefab => _dynamicAppLayoutControllerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _dynamicStoryLayoutControllerPrefab;

        public GameObject dynamicStoryLayoutControllerPrefab => _dynamicStoryLayoutControllerPrefab;

        [Required]
        [SerializeField]
        private GameObject _simplePlayableDirectorPrefab;

        public GameObject simplePlayableDirectorPrefab => _simplePlayableDirectorPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _contentExtensionControllerPrefab;

        public GameObject contentExtensionControllerPrefab => _contentExtensionControllerPrefab;

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
        
        [Required]
        [SerializeField]
        private GameObject _arrowIndicatorPrefab;

        public GameObject arrowIndicatorPrefab => _arrowIndicatorPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _scrubberPrefab;

        public GameObject scrubberPrefab => _scrubberPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _manualVideoPlayerPrefab;

        public GameObject manualVideoPlayerPrefab => _manualVideoPlayerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _forwardReverseVideoPlayerPrefab;

        public GameObject forwardReverseVideoPlayerPrefab => _forwardReverseVideoPlayerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _DOTweenUtilsPrefab;

        public GameObject DOTweenUtilsPrefab => _DOTweenUtilsPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _audioSourceController;

        public GameObject audioSourceController => _audioSourceController;
        
        [Required]
        [SerializeField]
        private AudioMixer _audioMixerTemplate;

        public AudioMixer audioMixerTemplate => _audioMixerTemplate;
        
        [Required]
        [SerializeField]
        private GameObject _simpleSignalListenerPrefab;

        public GameObject simpleSignalListenerPrefab => _simpleSignalListenerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _complexEventListenerPrefab;

        public GameObject complexEventListenerPrefab => _complexEventListenerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _actionTriggerPrefab;

        public GameObject actionTriggerPrefab => _actionTriggerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _prepareScenePrefab;

        public GameObject prepareScenePrefab => _prepareScenePrefab;


    }
}