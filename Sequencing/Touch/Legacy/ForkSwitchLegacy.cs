using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{

    [Serializable]
    public class ForkSwitchLegacy : AxisSwitchLegacy
    {

//        [PropertyOrder(1)]
//        [ShowInInspector]
//        [ReadOnly]
//        [InfoBox("The switch is enabled at runtime when connected to an AxisSwitchTrigger playable")]
//        bool switchEnabled;
//        public bool SwitchEnabled {
//            get {
//                return switchEnabled;
//            }
//
//            set {
//                switchEnabled = value;
//            }
//        }
//
//        [SerializeField]
//        [ValidateInput(nameof(IsPopulated))]
//        [FoldoutGroup("Fork Variables", 1)]
//        BoolReference forkActive;
//
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Fork Variables", 1)]
//        public FloatReference forkTransitionSpread = new FloatReference();
//
//        // An additional threshold, beyond the transition threshold, that we use to update the status of
//        // the new active and inactive axes after a transition
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Fork Variables", 1)]
//        public FloatReference swipeOriginDestSpread = new FloatReference();
//
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Input Variables", 2)]
//        [PropertyOrder(1)]
//        public FloatReference swipeModifier;
//
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Input Variables", 2)]
//        [PropertyOrder(1)]
//        public FloatReference momentumModifier;
//
//        // Swipe variables
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Input Variables", 2)]
//        [PropertyOrder(1)]
//        public V3Reference swipeForce;
//
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Input Variables", 2)]
//        public V3Reference momentumForce;
//
//        [Required]
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Input Variables", 2)]
//        public BoolReference isReversing;
//
//        [PropertyOrder(1)]
//        [ValidateInput("IsPopulated", "Fork must contain 3 or 4 branches!", InfoMessageType.Error)]
//        public List<BranchingPath> branchingPaths = new List<BranchingPath>();
//
//        [Required]
//        [FoldoutGroup("Axis Variables", 3)]
//        public Axis xSwipeAxis;
//
//        [Required]
//        [FoldoutGroup("Axis Variables", 3)]
//        public Axis ySwipeAxis;
//
//        [Required]
//        [FoldoutGroup("Axis Variables", 3)]
//        public Axis xMomentumAxis;
//
//        [Required]
//        [FoldoutGroup("Axis Variables", 3)]
//        public Axis yMomentumAxis;
//
//        // Momentum variables
//        [Required]
//        [FoldoutGroup("Axis Variables", 3)]
//        public ComplexEvent ConvertMomentum;
//
//        BranchingPath activeBranch;
//
//        Dictionary<BranchType, BranchingPath> branchDictionary = new Dictionary<BranchType, BranchingPath>();
//
//        enum SequenceUpdateType { Next, Previous }
//
//        float[] swipeYHistory = new float[10];
//
//        float[] swipeXHistory = new float[10];
//
//        int swipeHistoryIndex;
//
//        void Start()
//        {
//            for (int i = 0; i < branchingPaths.Count; i++) {
//                branchDictionary.Add(branchingPaths[i].branchType, branchingPaths[i]);
//            }
//
//            foreach (KeyValuePair<BranchType, BranchingPath> entry in branchDictionary) {
//                entry.Value.director = entry.Value.directorObject.GetComponent<SyncTimelineToSequence>();
//            }
//        }
//
//        public void ResetSwipeHistory()
//        {
//            swipeYHistory = new float[10];
//            swipeXHistory = new float[10];
//            swipeHistoryIndex = 0;
//        }
//
//        public void UpdateDestinationSequence()
//        {
//            activeBranch = GetActiveSequence();
//
//            if (activeBranch == null || SwitchEnabled == false) {
//                return;
//            }
//
//            if (swipeHistoryIndex < swipeYHistory.Length) {
//                swipeYHistory[swipeHistoryIndex] = Mathf.Abs(swipeForce.Value.y);
//                swipeXHistory[swipeHistoryIndex] = Mathf.Abs(swipeForce.Value.x);
//                swipeHistoryIndex++;
//            }
//
//            // Activate the fork if we're within the correct thresholds of the inflection point. If active branch is the origin,
//            // then this happens at the specified forkInflectionPoint, modified by the spread. However, if active branch is one of the destinations,
//            // then the fork only activates at the branch's first frame, aka at 0 plus the spread.
//            if ((activeBranch.sequence.currentTime >= _inflectionPoint.Value - forkTransitionSpread.Value && activeBranch.isOrigin == true) ||
//                (activeBranch.sequence.currentTime <= forkTransitionSpread.Value && activeBranch.isOrigin == false)) {
//
//                forkActive.Variable.SetValue(true);
//
//                xSwipeAxis.active = true;
//                ySwipeAxis.active = true;
//
//                Vector3 swipeForceToApply = swipeForce.Value;
//
//                // If we're in a fork, only apply force from the axis currently receiving greatest input    
//                swipeForceToApply = SequenceTouchApplier.GetForkSwipeForce(swipeForceToApply, swipeYHistory, swipeXHistory);
//
//                // Determine which direction we're swiping, then activate the new path accordingly
//                if (Mathf.Abs(swipeForceToApply.y) > Mathf.Abs(swipeForceToApply.x)) {
//                    ActivateXBranch();
//                } else if (Mathf.Abs(swipeForceToApply.x) > Mathf.Abs(swipeForceToApply.y)) {
//                    ActivateYBranch();
//                }
//
//                // Once we pass through the fork, we need this handling to reset our variables
//            } else if (
//                (activeBranch.sequence.currentTime < _inflectionPoint.Value - forkTransitionSpread.Value
//                 && activeBranch.sequence.currentTime >= _inflectionPoint.Value - (forkTransitionSpread.Value + swipeOriginDestSpread.Value) && activeBranch.isOrigin == true) ||
//                      (activeBranch.sequence.currentTime > forkTransitionSpread.Value &&
//                       activeBranch.sequence.currentTime <= (forkTransitionSpread.Value + swipeOriginDestSpread.Value) && activeBranch.isOrigin == false)) {
//                ResetBranchStates(new[] { BranchType.yPos, BranchType.yNeg, BranchType.xPos, BranchType.xNeg });
//
//                forkActive.Variable.SetValue(false);
//
//                // Flip axes accordingly
//                if (activeBranch.branchType == BranchType.yPos ||
//                    activeBranch.branchType == BranchType.yNeg) {
//
//                    ySwipeAxis.active = true;
//                    yMomentumAxis.active = true;
//                    xSwipeAxis.active = false;
//                    xMomentumAxis.active = false;
//
//                } else {
//
//                    ySwipeAxis.active = false;
//                    yMomentumAxis.active = false;
//                    xSwipeAxis.active = true;
//                    xMomentumAxis.active = true;
//                }
//
//            }
//        }
//
//
//        public void ActivateYBranch()
//        {
//            ResetBranchStates(new[] { BranchType.yPos, BranchType.yNeg });
//
//            // Due to the peculiarities of working with a timeline, we need to force
//            // the input on any opposing branches to be positive for the fork to work properly,
//            // otherwise erroneous negative input will prevent us from moving through it
//            if (activeBranch.branchType == BranchType.xPos ||
//               activeBranch.branchType == BranchType.xNeg) {
//                if (activeBranch.isOrigin == true) {
//                    activeBranch.sequence.forceForward = true;
//                } else {
//                    activeBranch.sequence.forceBackward = true;
//                }
//            }
//
//            yMomentumAxis.active = true;
//            xMomentumAxis.active = false;
//
//            // Subtract the value of the x axis, otherwise our swipe is not smooth
//            float sign = swipeModifier.Value >= 0 ? 1 : -1;
//            swipeModifier.Variable.SetValue((Mathf.Abs(swipeModifier.Value) - Mathf.Abs(swipeForce.Value.x)) * sign);
//
//            sign = momentumModifier.Value >= 0 ? 1 : -1;
//            momentumModifier.Variable.SetValue((Mathf.Abs(momentumModifier.Value) - Mathf.Abs(momentumForce.Value.x)) * sign);
//
//            // If we're on the origin branch, then we need to set the sequence and director that follow
//            // the current sequence. If we're on a subsequent branch, then we need to set the previous sequence
//            // and director so that we can rewind correctly
//            if (activeBranch.isOrigin == true) {
//
//                if (swipeForce.Value.y < 0) {
//                    if (branchDictionary.ContainsKey(BranchType.yPos)) {
//                        UpdateBranchSibling(activeBranch, BranchType.yPos, SequenceUpdateType.Next);
//                    }
//                } else {
//                    if (branchDictionary.ContainsKey(BranchType.yNeg)) {
//                        UpdateBranchSibling(activeBranch, BranchType.yNeg, SequenceUpdateType.Next);
//                    }
//                }
//
//            } else {
//
//                if (swipeForce.Value.y < 0) {
//                    if (branchDictionary.ContainsKey(BranchType.yPos)) {
//                        UpdateBranchSibling(activeBranch, BranchType.yPos, SequenceUpdateType.Previous);
//                    }
//                } else {
//                    if (branchDictionary.ContainsKey(BranchType.yNeg)) {
//                        UpdateBranchSibling(activeBranch, BranchType.yNeg, SequenceUpdateType.Previous);
//                    }
//                }
//
//            }
//        }
//
//        public void ActivateXBranch()
//        {
//            ResetBranchStates(new[] { BranchType.xPos, BranchType.xNeg });
//
//            // Due to the peculiarities of working with a timeline, we need to force
//            // the input on any opposing branches to be positive for the fork to work properly,
//            // otherwise erroneous negative input will prevent us from moving through it
//            if (activeBranch.branchType == BranchType.yPos ||
//                activeBranch.branchType == BranchType.yNeg) {
//                if (activeBranch.isOrigin == true) {
//                    activeBranch.sequence.forceForward = true;
//                } else {
//                    activeBranch.sequence.forceBackward = true;
//                }
//            }
//
//            xMomentumAxis.active = true;
//            yMomentumAxis.active = false;
//
//            // Subtract the value of the y axis, otherwise our swipe is not smooth
//            float sign = swipeModifier.Value >= 0 ? 1 : -1;
//            swipeModifier.Variable.SetValue((Mathf.Abs(swipeModifier.Value) - Mathf.Abs(swipeForce.Value.y)) * sign);
//
//            sign = momentumModifier.Value >= 0 ? 1 : -1;
//            momentumModifier.Variable.SetValue((Mathf.Abs(momentumModifier.Value) - Mathf.Abs(momentumForce.Value.y)) * sign);
//
//            // If we're on the origin branch, then we need to set the sequence and director that follow
//            // the current sequence. If we're on a subsequent branch, then we need to set the previous sequence
//            // and director so that we can rewind correctly
//            if (activeBranch.isOrigin == true) {
//
//                if (swipeForce.Value.x > 0) {
//                    if (branchDictionary.ContainsKey(BranchType.xPos)) {
//                        UpdateBranchSibling(activeBranch, BranchType.xPos, SequenceUpdateType.Next);
//                    }
//                } else {
//                    if (branchDictionary.ContainsKey(BranchType.xNeg)) {
//                        UpdateBranchSibling(activeBranch, BranchType.xNeg, SequenceUpdateType.Next);
//                    }
//                }
//
//            } else {
//
//                if (swipeForce.Value.x > 0) {
//                    if (branchDictionary.ContainsKey(BranchType.xPos)) {
//                        UpdateBranchSibling(activeBranch, BranchType.xPos, SequenceUpdateType.Previous);
//                    }
//                } else {
//                    if (branchDictionary.ContainsKey(BranchType.xNeg)) {
//                        UpdateBranchSibling(activeBranch, BranchType.xNeg, SequenceUpdateType.Previous);
//                    }
//                }
//
//            }
//        }
//
//        BranchingPath GetActiveSequence()
//        {
//            for (int i = 0; i < branchingPaths.Count; i++) {
//                if (branchingPaths[i].sequence.active == true) {
//                    return branchingPaths[i];
//                }
//            }
//            return null;
//        }
//
//        void ResetBranchStates(BranchType[] targetBranches)
//        {
//
//            foreach (KeyValuePair<BranchType, BranchingPath> entry in branchDictionary) {
//                for (int q = 0; q < targetBranches.Length; q++) {
//                    if (entry.Key == targetBranches[q]) {
//                        entry.Value.sequence.forceForward = false;
//                        entry.Value.sequence.forceBackward = false;
//                    }
//                }
//            }
//        }
//
//        void UpdateBranchSibling(BranchingPath targetBranch, BranchType newBranchType, SequenceUpdateType updateType)
//        {
//            // A branch's previous / next sibling cannot be itself, so cancel the update if this occurs
//            if (targetBranch.branchType == newBranchType) {
//                return;
//            }
//
//            if (updateType == SequenceUpdateType.Next) {
//                targetBranch.director.SetNextSequenceGroup(branchDictionary[newBranchType].sequence,
//                                                           branchDictionary[newBranchType].directorObject,
//                                                           branchDictionary[newBranchType].isOrigin);
//            } else {
//                targetBranch.director.SetPreviousSequenceGroup(branchDictionary[newBranchType].sequence,
//                                                           branchDictionary[newBranchType].directorObject,
//                                                           branchDictionary[newBranchType].isOrigin);
//            }
//        }
//
//        private static bool IsPopulated(List<BranchingPath> attribute)
//        {
//            if (attribute.Count >= 3 && attribute.Count < 5) {
//                return true;
//            } else {
//                return false;
//            }
//        }
//
//        private static bool IsPopulated(FloatReference attribute)
//        {
//            return Utils.IsPopulated(attribute);
//        }
//
//        private static bool IsPopulated(BoolReference attribute)
//        {
//            return Utils.IsPopulated(attribute);
//        }
//
//        private static bool IsPopulated(V3Reference attribute)
//        {
//            return Utils.IsPopulated(attribute);
//        }
    }

}