using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CurrentAspectRatioViewport : MonoBehaviour
    {
        [SerializeField]
        private AppSettingsReference _appSettings;

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        private float deviceWidth => appSettings.GetDeviceWidth(this.gameObject);

        private float deviceHeight => appSettings.GetDeviceHeight(this.gameObject);
        
        private float currentSceneWidth => appSettings.GetCurrentSceneWidth(this);
        
        private float currentSceneHeight => appSettings.GetCurrentSceneHeight(this);

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
#endif
            camera = GetComponent<Camera>();
            RefreshViewport();
        }

        [Button(ButtonSizes.Large)]
        public void RefreshViewport()
        {
            camera.pixelRect = new Rect((deviceWidth - currentSceneWidth) / 2f, (deviceHeight - currentSceneHeight) / 2f, currentSceneWidth, currentSceneHeight);
        }
    }
}