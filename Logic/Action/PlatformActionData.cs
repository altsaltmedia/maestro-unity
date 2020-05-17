using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public class PlatformActionData : ActionData, ISyncUnityEventHeadings, IRegisterNestedActionData
    {
        protected override string title => nameof(PlatformActionData);
        
        [SerializeField]
        [TitleGroup("$"+nameof(header))]
        private bool _ios;

        private bool ios => _ios;
        
        [SerializeField]
        [TitleGroup("$"+nameof(header))]
        private bool _android;

        private bool android => _android;
        
        [SerializeField]
        [TitleGroup("$"+nameof(header))]
        private bool _webGL;

        private bool webGL => _webGL;
        
        [SerializeField]
        [TitleGroup("$"+nameof(header))]
        private bool _standalone;

        private bool standalone => _standalone;
        
        private string header => "Execute for platforms: ";
        
        private bool _isIOS;

        private bool isIOS
        {
            get => _isIOS;
            set => _isIOS = value;
        }

        private bool _isAndroid;

        private bool isAndroid
        {
            get => _isAndroid;
            set => _isAndroid = value;
        }

        private bool _isWebGL;

        private bool isWebGL
        {
            get => _isWebGL;
            set => _isWebGL = value;
        }

        private bool _isStandalone;

        private bool isStandalone
        {
            get => _isStandalone;
            set => _isStandalone = value;
        }
        
        [PropertySpace]
        
        [SerializeField]
        [HideReferenceObjectPicker]
        private GameObjectGenericAction _action = new GameObjectGenericAction();

        private GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

        public PlatformActionData(int priority) : base(priority) {}

        private List<UnityEventData> _cachedEventData = new List<UnityEventData>();

        private List<UnityEventData> cachedEventData
        {
            get
            {
                if (_cachedEventData == null) {
                    _cachedEventData = new List<UnityEventData>();
                }

                return _cachedEventData;
            }
            set => _cachedEventData = value;
        }

        private string GetPlatformString()
        {
            string titleOutput = "";

            if (ios) {
                titleOutput += "(iOS)";
            }
            
            if (android) {
                titleOutput += " (Android)";
            }
            
            if (webGL) {
                titleOutput += " (WebGL)";
            }
            
            if (standalone) {
                titleOutput += " (Standalone)";
            }

            if (string.IsNullOrEmpty(titleOutput)) {
                return "No platforms specified";
            }

            return titleOutput;
        }

        public override void PerformAction(GameObject callingObject)
        {
#if UNITY_IOS
            if(ios == true) {
                action.Invoke(callingObject);
            }
#endif
            
#if UNITY_ANDROID
            if(android == true) {
                action.Invoke(callingObject);
            }
#endif
            
#if UNITY_WEBGL
            if(webGL == true) {
                action.Invoke(callingObject);
            }
#endif
            
#if UNITY_STANDALONE
            if(standalone == true) {
                action.Invoke(callingObject);
            }
#endif
        }

#if UNITY_EDITOR        
        public override void SyncEditorActionHeadings()
        {
//            string targets = "";
//            
//            for (int i = 0; i < eventAction.GetPersistentEventCount(); i++) {
//                if (eventAction.GetPersistentTarget(i) != null) {
//                    targets += eventAction.GetPersistentTarget(i).name;
//                    if (i < eventAction.GetPersistentEventCount() - 1) {
//                        targets += ", ";
//                    }
//                }
//            }
//
//            actionDescription = targets;
        }

        public void SyncUnityEventHeadings(SerializedProperty unityEventSerializedParent)
        {
            string newDescription = GetPlatformString() + "\n";

            UnityEventParameter[] parameters = UnityEventUtils.GetUnityEventParameters(unityEventSerializedParent, nameof(_action));
            if (UnityEventUtils.UnityEventValuesChanged(action, parameters, cachedEventData, out var eventData)) {
                newDescription += UnityEventUtils.ParseUnityEventDescription(eventData);
                cachedEventData = eventData;
            }

            if (string.IsNullOrEmpty(newDescription) == true) {
                actionDescription = "No generic events populated";
                return;
            }

            actionDescription = newDescription;
        }
#endif

    }
}