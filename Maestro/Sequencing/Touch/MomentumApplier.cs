using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using System.Linq;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class MomentumApplier : Touch_Module
    {
        [Required]
        [SerializeField]
        private SimpleEventTrigger _momentumApplied;

        private SimpleEventTrigger momentumApplied
        {
            get => _momentumApplied;
            set => _momentumApplied = value;
        }

        [SerializeField]
        [Required]
        [BoxGroup("Android dependencies")]
        private SimpleEventTrigger _pauseSequenceComplete;

        public SimpleEventTrigger pauseSequenceComplete
        {
            get => _pauseSequenceComplete;
            set => _pauseSequenceComplete = value;
        }

        public void UpdateSequenceWithMomentum()
        {
            if (touchController.appSettings.pauseMomentum == true) return;
            
            Vector2 momentumForceToApply = touchController.momentumForce;

            // If we're in a fork, only apply force from the axis currently receiving greatest input
            if (touchController.axisMonitor.axisTransitionActive == true || touchController.joiner.forkTransitionActive == true) {
                momentumForceToApply = touchController.GetDominantTouchForce(momentumForceToApply);
            }
            
            touchController.momentumModifierOutput = 0;

            if (touchController.yMomentumAxis.active) 
            {
                if (touchController.yMomentumAxis.inverted == false) {
                    touchController.momentumModifierOutput += momentumForceToApply.y;
                } else {
                    touchController.momentumModifierOutput += momentumForceToApply.y * -1f;
                }
            }
            
            if (touchController.xMomentumAxis.active)
            {
                if (touchController.xMomentumAxis.inverted == false) {
                    touchController.momentumModifierOutput += momentumForceToApply.x;
                } else {
                    touchController.momentumModifierOutput += momentumForceToApply.x * -1f;
                }
            }

            for (int q=0; q < touchController.touchDataList.Count; q++)
            {
                Touch_Data touchData = touchController.touchDataList[q];

                if (touchData.sequence.active == false || touchData.pauseMomentumActive == true)  {
                    continue;
                }

                if (touchData.forceForward == true) {
                    touchController.momentumModifierOutput = Mathf.Abs(touchController.momentumModifierOutput);
                } else if (touchData.forceBackward == true) {
                    touchController.momentumModifierOutput = Mathf.Abs(touchController.momentumModifierOutput) * -1f;
                }

                ApplyMomentumModifier(touchController.requestModifyToSequence, this, touchData.sequence, touchController.momentumModifierOutput);
            }
            
            momentumApplied.RaiseEvent(this.gameObject);
        }
        
        private static Sequence ApplyMomentumModifier(ComplexEventTrigger applyEvent, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            EventPayload eventPayload = EventPayload.CreateInstance();
            
            eventPayload.Set(DataType.scriptableObjectType, targetSequence);
            eventPayload.Set(DataType.intType, source.priority);
            eventPayload.Set(DataType.stringType, source.gameObject.name);
            eventPayload.Set(DataType.floatType, timeModifier);
            
            applyEvent.RaiseEvent(source.gameObject, eventPayload);

            return targetSequence;
        }

        private static bool IsPopulated(V3Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
