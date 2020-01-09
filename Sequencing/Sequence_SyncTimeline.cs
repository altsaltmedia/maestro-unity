/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Sequencing
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Sequence_Config))]
    [RequireComponent(typeof(Sequence_ProcessModify))]
    [RequireComponent(typeof(PlayableDirector))]
    public class Sequence_SyncTimeline : MonoBehaviour, IDynamicLayoutElement
    {
        [SerializeField]
        [ReadOnly]
        [InfoBox("This value must be set at runtime by a SequenceConfig component.")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;
        
        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }
        
        [SerializeField]
        private bool _logElementOnLayoutUpdate = false;

        public bool logElementOnLayoutUpdate {
            get {
                if (appSettings.logResponsiveElementActions == true) {
                    return true;
                } else {
                    return _logElementOnLayoutUpdate;
                }
            }
        }
        
        public string elementName => gameObject.name;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _dynamicElementEnable = new ComplexEventTrigger();

        public ComplexEventTrigger dynamicElementEnable => _dynamicElementEnable;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventTrigger _dynamicElementDisable = new ComplexEventTrigger();

        public ComplexEventTrigger dynamicElementDisable => _dynamicElementDisable;

        public Scene parentScene => gameObject.scene;

        public int priority => -10;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private BoolReference _scrubberActive;

        public BoolReference scrubberActive
        {
            get => _scrubberActive;
            set => _scrubberActive = value;
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
            
            RefreshPlayableDirector();
        }

        public void RefreshPlayableDirector()
        {
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }

        public void ForceEvaluate()
        {
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }

        public void SetToBeginning()
        {
            sequence.currentTime = 0;
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }

        public void SetToEnd()
        {
            sequence.currentTime = sequence.sourcePlayable.duration;
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }


#if UNITY_EDITOR
        private void OnEnable()
        {
            dynamicElementEnable.RaiseEvent(this.gameObject, this);
        }

//        private void OnDisable()
//        {
//            dynamicElementDisable.RaiseEvent(this.gameObject, this);
//        }
    
#endif
        private static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}