using System;
using System.Collections;
using UnityEngine;

namespace AltSalt.Sequencing.Autorun
{
    public class Lerper : AutorunModule
    {
        [SerializeField]
        private float _lerpInterval;

        private float lerpInterval
        {
            get => _lerpInterval;
            set => _lerpInterval = value;
        }

        private delegate AutorunController.AutorunData CoroutineCallback(AutorunController.AutorunData autorunData, Interval interval);

        public void TriggerLerpSequence(Sequence targetSequence)
        {
            if(CanLerpSequence(targetSequence, autorunController, out var autorunData) == false) return;

            if (autorunData.sequence.active == true && autorunData.isLerping == false)
            {
                if (Interval.TimeWithinThreshold(autorunData.sequence.currentTime,
                        autorunData.autorunIntervals, out var currentInterval) == false) return;

                autorunData.isLerping = true;

                float timeModifier = CalculateLerpModifier(lerpInterval, isReversing); 
                
                StartCoroutine(LerpSequence(autorunController.requestModifyToSequence, this, autorunData,
                    timeModifier, currentInterval, CheckEndLerp));
            }
        }

        private AutorunController.AutorunData CheckEndLerp(AutorunController.AutorunData autorunData, Interval interval)
        {
            if (isReversing == false)
            {
                if (autorunData.sequence.currentTime > interval.endTime)
                {
                    autorunData.isLerping = false;
                    StopCoroutine(nameof(LerpSequence));
                    TriggerInputActionComplete();
                }
            }
            else
            {
                if (autorunData.sequence.currentTime < interval.startTime) {
                    autorunData.isLerping = false;
                    StopCoroutine(nameof(LerpSequence));
                    TriggerInputActionComplete();
                }    
            }

            return autorunData;
        }
        
        private static bool CanLerpSequence(Sequence targetSequence, AutorunController autorunController, out AutorunController.AutorunData autorunData)
        {
            for (int i = 0; i < autorunController.autorunData.Count; i++)
            {
                if (targetSequence == autorunController.autorunData[i].sequence)
                {
                    autorunData = autorunController.autorunData[i];
                    return true;
                }
            }
            
            throw new Exception("Target sequence not found. Did you forget to assign an InputController or MasterController?");
        }


        private static float CalculateLerpModifier(float lerpValue, bool isReversing)
        {
            if (isReversing == true)
            {
                return lerpValue * -1f;
            }

            return lerpValue;
        }

        private static IEnumerator LerpSequence(ComplexEventTrigger applyEvent, InputModule source,
            AutorunController.AutorunData autorunData, float timeModifier, Interval currentInterval, CoroutineCallback callback)
        {
            while(true) {

                EventPayload eventPayload = EventPayload.CreateInstance();
                eventPayload.Set(DataType.scriptableObjectType, autorunData.sequence);
                eventPayload.Set(DataType.intType, source.priority);
                
                if (Application.platform == RuntimePlatform.Android)
                {
                    eventPayload.Set(DataType.floatType, timeModifier * 3f);
                }
                else
                {
                    eventPayload.Set(DataType.floatType, timeModifier);
                }
                
                applyEvent.RaiseEvent(source.gameObject, eventPayload);
                callback(autorunData, currentInterval);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}