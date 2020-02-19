using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class MomentumApplier : Touch_Module
    {
        protected override bool moduleActive
        {
            get
            {
                if (_moduleActive == false ||
                    touchController.appSettings.GetUserMomentumEnabled(this.gameObject, userKey) == false) {
                    return false;
                }

                return true;
            }
        }
        
        private SimpleEventTrigger momentumAppliedToSequences =>
            touchController.appSettings.GetMomentumAppliedToSequences(this.gameObject, inputGroupKey);
        
        private Vector2 swipeMonitorMomentumCache =>
            touchController.appSettings.GetSwipeMonitorMomentumCache(this.gameObject, inputGroupKey);
        
        private float momentumDecay =>
            touchController.appSettings.GetMomentumDecay(this.gameObject, inputGroupKey);

        
        [ShowInInspector]
        [ReadOnly]
        private Vector2 _momentumForceToApply;

        private Vector2 momentumForceToApply
        {
            get => _momentumForceToApply;
            set => _momentumForceToApply = value;
        }
        
        [ShowInInspector]
        [ReadOnly]
        private bool _hasMomentum;

        private bool hasMomentum
        {
            get => _hasMomentum;
            set => _hasMomentum = value;
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

        private SwipeDirection _lastSwipeDirection;

        private SwipeDirection lastSwipeDirection
        {
            get => _lastSwipeDirection;
            set => _lastSwipeDirection = value;
        }

        private void Update()
        {
            if (appUtilsRequested == true || hasMomentum == false || moduleActive == false) return;
            
            float momentumModifier;

            if (touchController.axisMonitor.axisTransitionActive == false &&
                touchController.joiner.forkTransitionActive == false) {
                momentumModifier = GetMomentumModifier(touchController, momentumForceToApply);

            } else {
                // If we're in a transition, only apply force from the axis currently receiving greatest input
                momentumForceToApply = NormalizeMomentumForce(lastSwipeDirection, momentumForceToApply);
                Vector2 modifiedForce = GetDominantMomentumForce(lastSwipeDirection, momentumForceToApply);
                momentumModifier = GetMomentumModifier(touchController, modifiedForce);
            }
            
            UpdateSequenceWithMomentum(this, momentumModifier);
            
            if (momentumForceToApply.sqrMagnitude < .00001f) {
                momentumForceToApply = new Vector2(0, 0);
                hasMomentum = false;
            }
            
            momentumForceToApply = new Vector2(momentumForceToApply.x * momentumDecay, momentumForceToApply.y * momentumDecay);
        }

        public void RefreshLocalMomentum()
        {
            hasMomentum = true;
            momentumForceToApply = swipeMonitorMomentumCache;
            if (SwipeDirection.TryParse(touchController.swipeDirection, out SwipeDirection swipeDirection)) {
                lastSwipeDirection = swipeDirection;
            }
        }
        
        public void HaltMomentum()
        {
            momentumForceToApply = new Vector2(0, 0);
            hasMomentum = false;
        }

        private static float GetMomentumModifier(Touch_Controller touchController, Vector2 momentumForce)
        {
            float momentumModifier = 0;

            // Force the momentum values to correspond to our axis based on whether we are reversing
            // or not (which is determined by the swipe applier)
            if (touchController.yMomentumAxis.IsActive() == true) {
                momentumModifier += GetAxisMomentum(momentumForce,
                    touchController.yMomentumAxis.GetVariable() as Axis, touchController.isReversing);
            }

            if (touchController.xMomentumAxis.IsActive() == true) {
                momentumModifier += GetAxisMomentum(momentumForce,
                    touchController.xMomentumAxis.GetVariable() as Axis, touchController.isReversing);
            }

            return momentumModifier;
        }

        private static Touch_Controller UpdateSequenceWithMomentum(MomentumApplier momentumApplier, float momentumModifier)
        {
            Touch_Controller touchController = momentumApplier.touchController;
            
            for (int q=0; q < touchController.touchDataList.Count; q++)
            {
                Touch_Data touchData = touchController.touchDataList[q];

                if (touchData.sequence.active == false || touchData.pauseMomentumActive == true)  {
                    continue;
                }

                touchController.momentumModifierOutput = momentumModifier;
                
                if (touchData.forceForward == true) {
                    touchController.momentumModifierOutput = Mathf.Abs(momentumModifier);
                } else if (touchData.forceBackward == true) {
                    touchController.momentumModifierOutput = Mathf.Abs(momentumModifier) * -1f;
                }

                ApplyMomentumModifier(touchController.rootConfig.masterSequences, momentumApplier, touchData.sequence, touchController.momentumModifierOutput);
            }
            
            momentumApplier.momentumAppliedToSequences.RaiseEvent(momentumApplier.gameObject);
            return touchController;
        }
        
        private static Sequence ApplyMomentumModifier(List<MasterSequence> masterSequences, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            MasterSequence masterSequence = masterSequences.Find(x => x.sequenceControllers.Find(y => y.sequence == targetSequence));
            masterSequence.RequestModifySequenceTime(targetSequence, source.priority, source.gameObject.name, timeModifier);

            return targetSequence;
        }

        private static float GetAxisMomentum(Vector2 momentumForce, Axis sourceAxis, bool isReversing)
        {
            float correctedMomentum;
            float directionModifier;
            
            switch (sourceAxis.axisType) {
            
                case AxisType.Y:
                    directionModifier = isReversing == false ? 1f : -1f;
                    correctedMomentum = Mathf.Abs(momentumForce.y) * directionModifier;
                    break;
                
                case AxisType.X:
                    directionModifier = isReversing == false ? 1f : -1f;
                    correctedMomentum = Mathf.Abs(momentumForce.x) * directionModifier;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return correctedMomentum;
        }
        
        private static Vector2 NormalizeMomentumForce(SwipeDirection swipeDirection, Vector2 momentumForce)
        {
            // Replace momentum on the new axis with momentum from the old axis.
            // In most cases, we'll need to do this transformation by undoing the sensitivity operation

            if (swipeDirection == SwipeDirection.xPositive || swipeDirection == SwipeDirection.xNegative) {
                return new Vector2(momentumForce.x, momentumForce.x);
            } 
            
            return new Vector2( momentumForce.y, momentumForce.y);
        }

        private static Vector2 GetDominantMomentumForce(SwipeDirection swipeDirection, Vector2 momentumForce)
        {
            if (swipeDirection == SwipeDirection.xPositive || swipeDirection == SwipeDirection.xNegative) {
                return new Vector2(momentumForce.x, 0);
            } 
            
            return new Vector2( 0, momentumForce.y);
        }
    }
}
