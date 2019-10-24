﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{
    public class SwipeModifier : TouchModule
    {
        [SerializeField]
        [Required]
        [BoxGroup("Android dependencies")]
        SimpleEventTrigger pauseSequenceComplete;

        public void UpdateSequenceWithSwipe()
        {
            Vector2 swipeForceToApply = touchController.swipeForce;

            // If we're in a fork, only apply force from the axis currently receiving greatest input
            if (touchController.forkActive == true) {
                swipeForceToApply = touchController.GetDominantSwipeForce(swipeForceToApply);
            }

            touchController.swipeModifierOutput = 0;

            if (touchController.ySwipeAxis.active) 
            {
                if (touchController.ySwipeAxis.inverted == false) {
                    touchController.swipeModifierOutput += swipeForceToApply.y;
                } else {
                    touchController.swipeModifierOutput += swipeForceToApply.y * -1f;
                }
            }
            
            if (touchController.xSwipeAxis.active)
            {
                if (touchController.xSwipeAxis.inverted == false) {
                    touchController.swipeModifierOutput += swipeForceToApply.x;
                } else {
                    touchController.swipeModifierOutput += swipeForceToApply.x * -1f;
                }
            }

            if (touchController.swipeModifierOutput > 0) {
                touchController.isReversing = false;
            } else if (touchController.swipeModifierOutput < 0) {
                touchController.isReversing = true;
            }

            for (int q=0; q < touchController.touchDataList.Count; q++)
            {
                TouchController.TouchData touchData = touchController.touchDataList[q];

                if (touchData.sequence.active == false)  {
                    continue;
                }
                
                if(touchData.sequence.invert == true) {
                    touchController.swipeModifierOutput *= -1f;
                }
                
                if (touchData.forceForward == true) {
                    touchController.swipeModifierOutput = Mathf.Abs(touchController.swipeModifierOutput);
                } else if (touchData.forceBackward == true) {
                    touchController.swipeModifierOutput = Mathf.Abs(touchController.swipeModifierOutput) * -1f;
                }

                ApplySwipeModifier(touchController.requestModifyToSequence, this, touchData.sequence, touchController.swipeModifierOutput);
            }
        }

        private static EventPayload ApplySwipeModifier(ComplexEventTrigger applyEvent, InputModule source, Sequence targetSequence, float timeModifier)
        {
            EventPayload eventPayload = EventPayload.CreateInstance();
            
            eventPayload.Set(DataType.scriptableObjectType, targetSequence);
            eventPayload.Set(DataType.intType, source.priority);
            eventPayload.Set(DataType.floatType, timeModifier);
            eventPayload.Set(DataType.stringType, source.name);
            
            applyEvent.RaiseEvent(source.gameObject, eventPayload);

            return eventPayload;
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
