using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using System.Linq;

namespace AltSalt.Sequencing.Touch
{
    public class MomentumModifier : TouchModule
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
            
            touchController.momentumModifierOutput = 0;

            if (touchController.yMomentumAxis.active) 
            {
                if (touchController.yMomentumAxis.inverted == false) {
                    touchController.momentumModifierOutput += touchController.momentumForce.y;
                } else {
                    touchController.momentumModifierOutput += touchController.momentumForce.y * -1f;
                }
            }
            
            if (touchController.xMomentumAxis.active)
            {
                if (touchController.xMomentumAxis.inverted == false) {
                    touchController.momentumModifierOutput += touchController.momentumForce.x;
                } else {
                    touchController.momentumModifierOutput += touchController.momentumForce.x * -1f;
                }
            }

            for (int q=0; q < touchController.touchDataList.Count; q++)
            {
                TouchController.TouchData touchData = touchController.touchDataList[q];

                if (touchData.sequence.active == false || touchData.pauseMomentumActive == true)  {
                    continue;
                }
                
                if(touchData.sequence.invert == true) {
                    touchController.momentumModifierOutput *= -1f;
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
        
        private static Sequence ApplyMomentumModifier(ComplexEventTrigger applyEvent, InputModule source, Sequence targetSequence, float timeModifier)
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
