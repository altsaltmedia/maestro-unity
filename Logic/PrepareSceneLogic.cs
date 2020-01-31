using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt.Maestro.Logic {

    [ExecuteInEditMode]
	public class PrepareSceneLogic : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
#if UNITY_EDITOR                
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }
#endif
                return _appSettings;
            }
            set => _appSettings = value;
        }

        [SerializeField]
        private bool _resetTouchVariables = false;

        private bool resetTouchVariables => _resetTouchVariables;

        [SerializeField]
        bool defaultX;
        [SerializeField]
        bool defaultY;
        [SerializeField]
        bool defaultZ;
        
        [SerializeField]
        Axis xSwipeAxis;
        [SerializeField]
        Axis ySwipeAxis;
        [SerializeField]
        Axis zSwipeAxis;

        [SerializeField]
        Axis xMomentumAxis;
        [SerializeField]
        Axis yMomentumAxis;
        [SerializeField]
        Axis zMomentumAxis;

        [SerializeField]
        private List<ModifiableEditorVariable> _variablesToReset;

        [FormerlySerializedAs("_prepareSceneCompleteEvents")]
        [SerializeField]
        private UnityEvent _prepareSceneEvents;

        private UnityEvent prepareSceneEvents => _prepareSceneEvents;

        private void Start() {
            ResetScene();
		}
        
        [HorizontalGroup("Split", 1f)]
        [InfoBox("Reset scene variables")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ResetScene()
        {
            prepareSceneEvents.Invoke();
        }
        
    }
}
