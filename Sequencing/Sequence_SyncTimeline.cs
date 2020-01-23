/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;

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
            get
            {
                if (_logElementOnLayoutUpdate == true || appSettings.logGlobalResponsiveElementActions == true) {
                    return true;
                }

                return false;
            }
        }
        
        public string elementName => gameObject.name;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _enableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger enableDynamicElement => _enableDynamicElement;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _disableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger disableDynamicElement => _disableDynamicElement;

        public Scene parentScene => gameObject.scene;

        [SerializeField]
        private int _priority = -10;
        
        public int priority => _priority;

        public bool scrubberActive => appSettings.GetScrubberActive(this.gameObject,
            sequence.sequenceConfig.masterSequence.rootConfig.inputGroupKey);

        private void Start()
        {
            
        }

        public void CallExecuteLayoutUpdate(Object callingObject)
        {
#if UNITY_EDITOR
            if (sequence == null || sequence.sequenceConfig == null ||
                TimelineEditor.inspectedDirector != sequence.sequenceConfig.playableDirector) {
                return;
            }
            
            if (logElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + elementName, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }
            
            sequence.sequenceConfig.playableDirector.Evaluate();
#endif
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
            _enableDynamicElement.PopulateVariable(this, nameof(_enableDynamicElement));
            _disableDynamicElement.PopulateVariable(this, nameof(_disableDynamicElement));
            
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }

//        private void OnDisable()
//        {
//            dynamicElementDisable.RaiseEvent(this.gameObject, this);
//        }
    
#endif
        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}