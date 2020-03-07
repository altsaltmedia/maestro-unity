using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class ResponsiveAutoViewport : MonoBehaviour, ISceneDimensionListener, IDynamicLayoutElement
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        private float deviceWidth => appSettings.GetDeviceWidth(this.gameObject);

        private float deviceHeight => appSettings.GetDeviceHeight(this.gameObject);

        [ShowInInspector]
        [ReadOnly]
        private float _sceneWidth = 4.65f;

        public float sceneWidth
        {
            get => _sceneWidth;
            set => _sceneWidth = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneHeight = 4.594452f;

        public float sceneHeight
        {
            get => _sceneHeight;
            set => _sceneHeight = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneAspectRatio = 1.33f;

        public float sceneAspectRatio
        {
            get => _sceneAspectRatio;
            set => _sceneAspectRatio = value;
        }
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _enableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger enableDynamicElement => _enableDynamicElement;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _disableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger disableDynamicElement => _disableDynamicElement;

        public string elementName => this.name;
        
        [SerializeField]
        protected bool _logElementOnLayoutUpdate;
        
        public bool logElementOnLayoutUpdate {
            get
            {
                if (_logElementOnLayoutUpdate == true || AppSettings.logGlobalResponsiveElementActions == true) {
                    return true;
                }

                return false;
            }
        }

        public Scene parentScene => this.gameObject.scene;

        [SerializeField]
#if UNITY_EDITOR        
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        private int _priority;

        public int priority => _priority; 

        private Camera _camera;

        private Camera camera
        {
            get
            {
                if (_camera == null) {
                    _camera = GetComponent<Camera>();
                }
                return _camera;
            }
            set => _camera = value;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            if (string.IsNullOrEmpty(_enableDynamicElement.referenceName) == true) {
                _enableDynamicElement.referenceName = nameof(enableDynamicElement).Capitalize();
            }
            _enableDynamicElement.PopulateVariable(this, nameof(_enableDynamicElement));
            if (string.IsNullOrEmpty(_disableDynamicElement.referenceName) == true) {
                _disableDynamicElement.referenceName = nameof(disableDynamicElement).Capitalize();
            }
            _disableDynamicElement.PopulateVariable(this, nameof(_disableDynamicElement));
#endif
            camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }
        
        public void CallExecuteLayoutUpdate(Object callingObject)
        {
            if (logElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + elementName, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }
            ExecuteResponsiveAction();
        }

        [InfoBox("Automatically sets the viewport to the current scene's dimensions")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        private void ExecuteResponsiveAction()
        {
            camera.pixelRect = new Rect((deviceWidth - sceneWidth) / 2f, (deviceHeight - sceneHeight) / 2f, sceneWidth, sceneHeight);
        }
        
        
#if UNITY_EDITOR
        private void ResetResponsiveElementData()
        {
            disableDynamicElement.RaiseEvent(this.gameObject, this);
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }
#endif

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}