using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Autorun
{
    public class Autoplayer : AutorunModule
    {
        [SerializeField]
        [ValidateInput("IsPopulated")]
        private FloatReference _frameStepValue;
        
        private float frameStepValue
        {
            get => _frameStepValue.Value;
        }

        [SerializeField]
        [Range(0f, 5f)]
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
                
                if (Interval.TimeWithinThreshold(autorunData.sequence.currentTime,
                        autorunData.autorunIntervals, out var currentInterval) == false)
                    continue;

                float autoplayModifer = frameStepValue;
                autoplayModifer *= CalculateAutoplayModifier(autorunData.sequence, currentInterval, isReversing, autoplayEaseThreshold, easingUtility);

                AutoplaySequence(autorunController.requestModifyToSequence, this, autorunData.sequence, autoplayModifer);
            
                if (autorunData.autoplayActive == true && (autorunData.sequence.currentTime > currentInterval.endTime || autorunData.sequence.currentTime < currentInterval.startTime))  {
                    autorunData.autoplayActive = false;
                    easingUtility.Reset();
                    TriggerInputActionComplete();
                }
            }
        }
        
        private static float CalculateAutoplayModifier(Sequence targetSequence, StartEndThreshold currentInterval,
            bool isReversing, float easeThreshold, EasingUtility easingUtility)
        {
            float timeModifier;
            
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
            
        private static bool WithinEaseThreshold(Sequence targetSequence, StartEndThreshold currentInterval,
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

        private static Sequence AutoplaySequence(ComplexEventTrigger applyEvent, InputModule source, Sequence targetSequence, float timeModifier)
        {
            EventPayload eventPayload = EventPayload.CreateInstance();
            eventPayload.Set(DataType.scriptableObjectType, targetSequence);
            eventPayload.Set(DataType.intType, source.priority);

            if (Application.platform == RuntimePlatform.Android)  {
                eventPayload.Set(DataType.floatType, timeModifier * 3f);
            } else {
                eventPayload.Set(DataType.floatType, timeModifier);
            }
            
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