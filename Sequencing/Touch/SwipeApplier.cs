using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class SwipeApplier : Touch_Module
    {
        [SerializeField]
        [Required]
        [BoxGroup("Android dependencies")]
        SimpleEventTrigger pauseSequenceComplete;

        public void UpdateSequenceWithSwipe()
        {
            if (appUtilsRequested == true || moduleActive == false) return; 
            
            Vector2 swipeForceToApply = touchController.swipeForce;

            // If we're in a fork, only apply force from the axis currently receiving greatest input
            if (touchController.axisMonitor.axisTransitionActive == true || touchController.joiner.forkTransitionActive == true) {
                swipeForceToApply = touchController.GetDominantTouchForce(swipeForceToApply);
            }

            touchController.swipeModifierOutput = 0;

            if (touchController.ySwipeAxis.IsActive() == true) 
            {
                if (touchController.ySwipeAxis.IsInverted() == false) {
                    touchController.swipeModifierOutput += swipeForceToApply.y;
                } else {
                    touchController.swipeModifierOutput += swipeForceToApply.y * -1f;
                }
            }
            
            if (touchController.xSwipeAxis.IsActive() == true)
            {
                if (touchController.xSwipeAxis.IsInverted() == false) {
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
                Touch_Data touchData = touchController.touchDataList[q];

                if (touchData.sequence.active == false)  {
                    continue;
                }

                if (touchData.forceForward == true) {
                    touchController.swipeModifierOutput = Mathf.Abs(touchController.swipeModifierOutput);
                } else if (touchData.forceBackward == true) {
                    touchController.swipeModifierOutput = Mathf.Abs(touchController.swipeModifierOutput) * -1f;
                }

                ApplySwipeModifier(touchController.rootConfig.masterSequences, this, touchData.sequence, touchController.swipeModifierOutput);
            }
        }

        private static Sequence ApplySwipeModifier(List<MasterSequence> masterSequences, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            MasterSequence masterSequence = masterSequences.Find(x => x.sequenceControllers.Find(y => y.sequence == targetSequence));
            masterSequence.TriggerModifyRequest(targetSequence, source.priority, source.gameObject.name, timeModifier);

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
