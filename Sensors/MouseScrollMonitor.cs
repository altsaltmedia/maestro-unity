using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace AltSalt.Maestro.Sensors
{
    public class MouseScrollMonitor : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

        [SerializeField]
        private UserDataKeyReference _userDataKey = new UserDataKeyReference();

        private UserDataKey userDataKey => _userDataKey.GetVariable() as UserDataKey;
        
        // Input Group
        
        [SerializeField]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        private InputGroupKey inputGroupKey => _inputGroupKey.GetVariable() as InputGroupKey;
        
        private float mouseScrollDelta
        {
            get => appSettings.GetMouseScrollDelta(this.gameObject, inputGroupKey);
            set => appSettings.SetMouseScrollDelta(this.gameObject, inputGroupKey, value);
        }

        [SerializeField]
        private TouchInputManager inputManager;

        private SimpleEvent onScrollEnd => appSettings.GetOnScrollEnd(this.gameObject, inputGroupKey).GetVariable() as SimpleEvent;
            
        private float mouseScrollSensitivity => appSettings.GetMouseScrollSensitivity(this.gameObject, userDataKey);

        private float _previousMouseScroll;

        private float previousMouseScroll
        {
            get => _previousMouseScroll;
            set => _previousMouseScroll = value;
        }

//#if UNITY_IOS
//        private void Update()
//        {
//            if (Device.iosAppOnMac)
//            {
//                Debug.Log("on mac");
//                float delta = Input.GetAxis("Mouse ScrollWheel") * mouseScrollSensitivity * -1f;
//                Debug.Log(delta);

//                if (Mathf.Abs(delta) > 0)
//                {
//                    mouseScrollDelta = delta;
//                }

//                if (Mathf.Abs(previousMouseScroll) > 0 && delta == 0)
//                {
//                    onScrollEnd.StoreCaller(this.gameObject);
//                    onScrollEnd.SignalChange();
//                }

//                previousMouseScroll = delta;
//            }
//        }
//#endif

#if UNITY_EDITOR
        private void Awake()
        {
            _appSettings.PopulateVariable(this, nameof(_appSettings));
        }
#endif

        private void OnEnable()
        {
            if (!inputManager)
            {
                inputManager = GameObject.Find("TouchInputManager").GetComponent<TouchInputManager>();
            }
            inputManager.Scroll += OnScroll;
        }

        private void OnDisable()
        {
            inputManager.Scroll -= OnScroll;
        }

        private void OnScroll(Vector2 scroll)
        {            
            float delta = scroll.y * mouseScrollSensitivity * -1f;

            if (Mathf.Abs(delta) > 0)
            {
                mouseScrollDelta = delta;
            }

            if (Mathf.Abs(previousMouseScroll) > 0 && delta == 0)
            {
                onScrollEnd.StoreCaller(this.gameObject);
                onScrollEnd.SignalChange();
            }

            previousMouseScroll = delta;      
        }
    }
    
}
