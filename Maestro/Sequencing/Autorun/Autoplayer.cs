using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public class Autoplayer : Autorun_Module
    {
        [SerializeField]
        [ValidateInput("IsPopulated")]
        private FloatReference _frameStepValue;
        
        private float frameStepValue
        {
            get => _frameStepValue.Value;
        }

        [SerializeField]
        [Range(0f, 1f)]
        private float _autoplayEaseThreshold = 0.25f;

        private float autoplayEaseThreshold
        {
            get => _autoplayEaseThreshold;
        }

        [SerializeField]
        private EasingUtility _easingUtility = new EasingUtility();

        private EasingUtility easingUtility
        {
            get => _easingUtility;
        }
        
        protected virtual void Update()
        {
            if (_isparentModuleNull || autorunController.appSettings.autoplayActive.Value == false) {
                return;
            }

            for (int q = 0; q < autorunController.autorunData.Count; q++)
            {
                var autorunData = autorunController.autorunData[q];

                if (autorunData.autoplayActive == false || autorunData.isLerping == true || autorunData.sequence.active == false) {
                    continue;
                }
                
                if (AutorunExtents.TimeWithinThreshold(autorunData.sequence.currentTime,
                        autorunData.autorunIntervals, out var currentInterval) == false)
                    continue;

                
                float autoplayModifer = 0f; 
#if UNITY_EDITOR
                autoplayModifer = Time.smoothDeltaTime;
#else
                if (Application.platform == RuntimePlatform.Android)  {
                    autoplayModifer = frameStepValue * 3f;
                } else  {
                    autoplayModifer = frameStepValue;
                }
#endif
                autoplayModifer *= CalculateAutoplayModifier(autorunData.sequence, currentInterval, autorunController.isReversing, autoplayEaseThreshold, easingUtility);

                AutoplaySequence(autorunController.requestModifyToSequence, this, autorunData.sequence, autoplayModifer);
            
                if (autorunData.autoplayActive == true &&
                    (Mathf.Approximately((float)autorunData.sequence.currentTime, (float)currentInterval.endTime)
                     || autorunData.sequence.currentTime > currentInterval.endTime
                     || Mathf.Approximately((float)autorunData.sequence.currentTime, (float)currentInterval.startTime)
                     || autorunData.sequence.currentTime < currentInterval.startTime)) {
                    
                    autorunData.autoplayActive = false;
                    easingUtility.Reset();
                    if (currentInterval.isEnd == true) {
                        TriggerInputActionComplete();
                    }
                }
            }
        }
        
        private static float CalculateAutoplayModifier(Sequence targetSequence, Extents currentInterval,
            bool isReversing, float easeThreshold, EasingUtility easingUtility)
        {
            float timeModifier = 0f;

            if (WithinEaseThreshold(targetSequence, currentInterval, isReversing, easeThreshold)) {
                timeModifier = easingUtility.GetMultiplier();
            } else {
                timeModifier = 1f;
            }

            if (isReversing == true) {
                timeModifier *= -1f;
            }

            return timeModifier;
        }
            
        private static bool WithinEaseThreshold(Sequence targetSequence, Extents currentInterval,
            bool isReversing, float easeThreshold)
        {
            if (isReversing == true)
            {
                if (targetSequence.currentTime - currentInterval.startTime <= easeThreshold)
                {
                    return true;
                }
            }
            else
            {
                if (currentInterval.endTime - targetSequence.currentTime  <= easeThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        private static Sequence AutoplaySequence(ComplexEventTrigger applyEvent, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            EventPayload eventPayload = EventPayload.CreateInstance();
            eventPayload.Set(DataType.scriptableObjectType, targetSequence);
            eventPayload.Set(DataType.intType, source.priority);
            eventPayload.Set(DataType.stringType, source.gameObject.name);
            eventPayload.Set(DataType.floatType, timeModifier);

            applyEvent.RaiseEvent(source.gameObject, eventPayload);
            return targetSequence;
        }

        public void ActivateAutoplay()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                
                if (autorunController.autorunData[q].sequence.active == true) {
                    
                    autorunController.autorunData[q].autoplayActive = true;
                }
            }
        }

        public void DeactivateAutoplay()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                
                if (autorunController.autorunData[q].sequence.active == true) {
                    autorunController.autorunData[q].autoplayActive = false;
                    easingUtility.Reset();
                }
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}