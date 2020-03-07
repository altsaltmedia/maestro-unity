using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace AltSalt.Maestro
{
    public class ModuleReferences : ScriptableObject
    {

    #region Standard Objects

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
        private GameObject _cameraPrefab;

        public GameObject cameraPrefab => _cameraPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _responsiveCameraPrefab;

        public GameObject responsiveCameraPrefab => _responsiveCameraPrefab;
        
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
        
    #endregion


    #region UI Objects

        [Required]
        [SerializeField]
        private GameObject _canvasPrefab;

        public GameObject canvasPrefab => _canvasPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _childCanvasPrefab;

        public GameObject childCanvasPrefab => _childCanvasPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _overlayCanvasPrefab;

        public GameObject overlayCanvasPrefab => _overlayCanvasPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _imagePrefab;

        public GameObject imagePrefab => _imagePrefab;
        
        [Required]
        [SerializeField]
        private GameObject _backgroundPrefab;

        public GameObject backgroundPrefab => _backgroundPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _uiTextPrefab;

        public GameObject uiTextPrefab => _uiTextPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _buttonPrefab;

        public GameObject buttonPrefab => _buttonPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _buttonWithTextPrefab;

        public GameObject buttonWithTextPrefab => _buttonWithTextPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _sliderPrefab;

        public GameObject sliderPrefab => _sliderPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _callWebviewButtonPrefab;

        public GameObject callWebviewButtonPrefab => _callWebviewButtonPrefab;

    #endregion

        
    #region Sequencing

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
        
    #endregion

        
    #region Animation

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

    #endregion


    #region Audio

        [Required]
        [SerializeField]
        private AudioMixer _audioMixerTemplate;

        public AudioMixer audioMixerTemplate => _audioMixerTemplate;
        
        [Required]
        [SerializeField]
        private GameObject _simpleSignalListenerPrefab;

        public GameObject simpleSignalListenerPrefab => _simpleSignalListenerPrefab;

    #endregion
        
        
    #region Logic
        
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
        private GameObject _audioSourceController;

        public GameObject audioSourceController => _audioSourceController;

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
        
        [Required]
        [SerializeField]
        private GameObject _initializerPrefab;

        public GameObject initializerPrefab => _initializerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _appUtilsPrefab;

        public GameObject appUtilsPrefab => _appUtilsPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _currentAspectRatioCameraPrefab;

        public GameObject currentAspectRatioCameraPrefab => _currentAspectRatioCameraPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _systemDependenciesPrefab;

        public GameObject systemDependenciesPrefab => _systemDependenciesPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _faderProgressBarCanvasPrefab;

        public GameObject faderProgressBarCanvasPrefab => _faderProgressBarCanvasPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _faderPanelPrefab;

        public GameObject faderPanelPrefab => _faderPanelPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _progressBarPanelPrefab;

        public GameObject progressBarPanelPrefab => _progressBarPanelPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _requestAppUtilsPrefab;

        public GameObject requestAppUtilsPrefab => _requestAppUtilsPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _sceneControllerPrefab;

        public GameObject sceneControllerPrefab => _sceneControllerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _userDataControllerPrefab;

        public GameObject userDataControllerPrefab => _userDataControllerPrefab;

        [Required]
        [SerializeField]
        private GameObject _frameRateControllerPrefab;

        public GameObject frameRateControllerPrefab => _frameRateControllerPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _analyticsTrackersPrefab;

        public GameObject analyticsTrackersPrefab => _analyticsTrackersPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _touchMonitorPrefab;

        public GameObject touchMonitorPrefab => _touchMonitorPrefab;
        
        [Required]
        [SerializeField]
        private GameObject _handheldUtilsPrefab;

        public GameObject handheldUtilsPrefab => _handheldUtilsPrefab;

    #endregion
        
    }
}