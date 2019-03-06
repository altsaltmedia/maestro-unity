using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {

    public enum BranchName { yNeg, yPos, xNeg, xPos }

    [System.Serializable]
    public class BranchingPath
    {
        public bool isOrigin;
        public Sequence sequence;
        public GameObject directorObject;

        [HideInInspector]
        public DirectorUpdater director;

        [ValueDropdown("branchTypeValues")]
        public BranchName branchType;

        private ValueDropdownList<BranchName> branchTypeValues = new ValueDropdownList<BranchName>(){
            {"Y Negative", BranchName.yNeg },
            {"Y Positive", BranchName.yPos },
            {"X Negative", BranchName.xNeg },
            {"X Positive", BranchName.xPos }
        };
    }

    public class ForkSwitch : MonoBehaviour {
    

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Fork Variables", 1)]
        public FloatReference forkInflectionPoint = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Fork Variables", 1)]
        public FloatReference forkTransitionSpread = new FloatReference();

        // An additional threshold, beyond the transition threshold, that we use to update the status of
        // the new active and inactive axes after a transition
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Fork Variables", 1)]
        public FloatReference swipeOriginDestSpread = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Input Variables", 2)]
        [PropertyOrder(1)]
        public FloatReference swipeModifier;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Input Variables", 2)]
        [PropertyOrder(1)]
        public FloatReference momentumModifier;

        // Swipe variables
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Input Variables", 2)]
        [PropertyOrder(1)]
        public V3Reference swipeForce;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Input Variables", 2)]
        public V3Reference momentumForce;

        [Required]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Input Variables", 2)]
        public BoolReference isReversing;

        [PropertyOrder(1)]
        [ValidateInput("IsPopulated", "Fork must contain 3 or 4 branches!", InfoMessageType.Error)]
        public List<BranchingPath> branchingPaths = new List<BranchingPath>();

        [Required]
        [FoldoutGroup("Axis Variables", 3)]
        public Axis xSwipeAxis;

        [Required]
        [FoldoutGroup("Axis Variables", 3)]
        public Axis ySwipeAxis;

        [Required]
        [FoldoutGroup("Axis Variables", 3)]
        public Axis xMomentumAxis;

        [Required]
        [FoldoutGroup("Axis Variables", 3)]
        public Axis yMomentumAxis;

        // Momentum variables
        [Required]
        [FoldoutGroup("Axis Variables", 3)]
        public ComplexEvent ConvertMomentum;

        BranchingPath activeBranch;

        Dictionary<BranchName, BranchingPath> branchDictionary = new Dictionary<BranchName, BranchingPath>();

        enum SequenceUpdateType { Next, Previous }

        void Start()
        {
            for (int i = 0; i < branchingPaths.Count; i++) {
                branchDictionary.Add(branchingPaths[i].branchType, branchingPaths[i]);
            }

            Debug.Log(branchDictionary);

            foreach(KeyValuePair<BranchName, BranchingPath> entry in branchDictionary) {
                entry.Value.director = entry.Value.directorObject.GetComponent<DirectorUpdater>();
            }
        }

        public void UpdateDestinationSequence()
        {
            activeBranch = GetActiveSequence();

            // Activate the fork if we're within the correct thresholds of the inflection point. If active branch is the origin,
            // then this happens at the specified forkInflectionPoint, modified by the spread. However, if active branch is one of the destinations,
            // then the fork only activates at the branch's first frame, aka at 0 plus the spread.
            if ((activeBranch.sequence.currentTime >= forkInflectionPoint.Value - forkTransitionSpread.Value && activeBranch.isOrigin == true) ||
                (activeBranch.sequence.currentTime <= forkTransitionSpread.Value && activeBranch.isOrigin == false)) {

                xSwipeAxis.Active = true;
                ySwipeAxis.Active = true;

                // Determine which direction we're swiping, then activate the new path accordingly
                if (Mathf.Abs(swipeForce.Value.y) > Mathf.Abs(swipeForce.Value.x)) {
                    ActivateYBranch();
                } else if(Mathf.Abs(swipeForce.Value.x) > Mathf.Abs(swipeForce.Value.y)) {
                    ActivateXBranch();
                }

            // Once we pass through the fork, we need this handling to reset our variables
            } else if(
                (activeBranch.sequence.currentTime < forkInflectionPoint.Value - forkTransitionSpread.Value
                 && activeBranch.sequence.currentTime >= forkInflectionPoint.Value - (forkTransitionSpread.Value + swipeOriginDestSpread.Value) && activeBranch.isOrigin == true) ||
                      (activeBranch.sequence.currentTime > forkTransitionSpread.Value && 
                       activeBranch.sequence.currentTime <= (forkTransitionSpread.Value + swipeOriginDestSpread.Value) && activeBranch.isOrigin == false)) {
                ResetBranchStates(new[] { BranchName.yPos, BranchName.yNeg, BranchName.xPos, BranchName.xNeg });

                // Flip axes accordingly
                if (activeBranch.branchType == BranchName.yPos ||
                    activeBranch.branchType == BranchName.yNeg) {

                    ySwipeAxis.Active = true;
                    yMomentumAxis.Active = true;
                    xSwipeAxis.Active = false;
                    xMomentumAxis.Active = false;

                } else {

                    ySwipeAxis.Active = false;
                    yMomentumAxis.Active = false;
                    xSwipeAxis.Active = true;
                    xMomentumAxis.Active = true;
                }

            }
        }


        public void ActivateYBranch()
        {
            ResetBranchStates(new[] { BranchName.yPos, BranchName.yNeg });

            // Due to the peculiarities of working with a timeline, we need to force
            // the input on any opposing branches to be positive for the fork to work properly,
            // otherwise erroneous negative input will prevent us from moving through it
            if (activeBranch.branchType == BranchName.xPos ||
               activeBranch.branchType == BranchName.xNeg) {
                if(activeBranch.isOrigin == true) {
                    activeBranch.sequence.ForceForward = true;
                } else {
                    activeBranch.sequence.ForceBackward = true;
                }
            }

            yMomentumAxis.Active = true;

            // Subtract the value of the x axis, otherwise our swipe is not smooth
            float sign = swipeModifier.Value >= 0 ? 1 : -1;
            swipeModifier.Variable.SetValue((Mathf.Abs(swipeModifier.Value) - Mathf.Abs(swipeForce.Value.x)) * sign);

            sign = momentumModifier.Value >= 0 ? 1 : -1;
            momentumModifier.Variable.SetValue((Mathf.Abs(momentumModifier.Value) - Mathf.Abs(momentumForce.Value.x)) * sign);

            // If we're on the origin branch, then we need to set the sequence and director that follow
            // the current sequence. If we're on a subsequent branch, then we need to set the previous sequence
            // and director so that we can rewind correctly
            if (activeBranch.isOrigin == true) {

                if (isReversing.Value == false) {
                    if(branchDictionary.ContainsKey(BranchName.yPos)) {
                        UpdateBranchSibling(activeBranch, BranchName.yPos, SequenceUpdateType.Next);
                    }
                } else {
                    if(branchDictionary.ContainsKey(BranchName.yNeg)) {
                        UpdateBranchSibling(activeBranch, BranchName.yNeg, SequenceUpdateType.Next);
                    }
                }

            } else {

                if (isReversing.Value == false) {
                    if (branchDictionary.ContainsKey(BranchName.yPos)) {
                        UpdateBranchSibling(activeBranch, BranchName.yPos, SequenceUpdateType.Previous);
                    }
                } else {
                    if (branchDictionary.ContainsKey(BranchName.yNeg)) {
                        UpdateBranchSibling(activeBranch, BranchName.yNeg, SequenceUpdateType.Previous);
                    }
                }

            }
        }

        public void ActivateXBranch()
        {
            ResetBranchStates(new[] { BranchName.xPos, BranchName.xNeg });

            // Due to the peculiarities of working with a timeline, we need to force
            // the input on any opposing branches to be positive for the fork to work properly,
            // otherwise erroneous negative input will prevent us from moving through it
            if (activeBranch.branchType == BranchName.yPos ||
                activeBranch.branchType == BranchName.yNeg) {
                if (activeBranch.isOrigin == true) {
                    activeBranch.sequence.ForceForward = true;
                } else {
                    activeBranch.sequence.ForceBackward = true;
                }
            }

            xMomentumAxis.Active = true;

            // Subtract the value of the y axis, otherwise our swipe is not smooth
            float sign = swipeModifier.Value >= 0 ? 1 : -1;
            swipeModifier.Variable.SetValue((Mathf.Abs(swipeModifier.Value) - Mathf.Abs(swipeForce.Value.y)) * sign);

            sign = momentumModifier.Value >= 0 ? 1 : -1;
            momentumModifier.Variable.SetValue((Mathf.Abs(momentumModifier.Value) - Mathf.Abs(momentumForce.Value.y)) * sign);

            // If we're on the origin branch, then we need to set the sequence and director that follow
            // the current sequence. If we're on a subsequent branch, then we need to set the previous sequence
            // and director so that we can rewind correctly
            if (activeBranch.isOrigin == true) {

                if (isReversing.Value == false) {
                    if (branchDictionary.ContainsKey(BranchName.xPos)) {
                        UpdateBranchSibling(activeBranch, BranchName.xPos, SequenceUpdateType.Next);
                    }
                } else {
                    if (branchDictionary.ContainsKey(BranchName.xNeg)) {
                        UpdateBranchSibling(activeBranch, BranchName.xNeg, SequenceUpdateType.Next);
                    }
                }

            }
            else {

                if (isReversing.Value == false) {
                    if (branchDictionary.ContainsKey(BranchName.xPos)) {
                        UpdateBranchSibling(activeBranch, BranchName.xPos, SequenceUpdateType.Previous);
                    }
                } else {
                    if (branchDictionary.ContainsKey(BranchName.xNeg)) {
                        UpdateBranchSibling(activeBranch, BranchName.xNeg, SequenceUpdateType.Previous);
                    }
                }

            }
        }

        BranchingPath GetActiveSequence()
        {
            for (int i = 0; i < branchingPaths.Count; i++) {
                if (branchingPaths[i].sequence.Active == true) {
                    return branchingPaths[i];
                }
            }
            return null;
        }

        void ResetBranchStates(BranchName[] targetBranches) {

            foreach (KeyValuePair<BranchName, BranchingPath> entry in branchDictionary) {
                for (int q = 0; q < targetBranches.Length; q++) {
                    if (entry.Key == targetBranches[q]) {
                        entry.Value.sequence.ForceForward = false;
                        entry.Value.sequence.ForceBackward = false;
                    }
                }
            }
        }

        void UpdateBranchSibling(BranchingPath targetBranch, BranchName newBranchName, SequenceUpdateType updateType)
        {
            // A branch's previous / next sibling cannot be itself, so cancel the update if this occurs
            if(targetBranch.branchType == newBranchName) {
                return;
            }

            if(updateType == SequenceUpdateType.Next) {
                targetBranch.director.SetNextSequenceGroup(branchDictionary[newBranchName].sequence,
                                                           branchDictionary[newBranchName].directorObject,
                                                           branchDictionary[newBranchName].isOrigin);
            } else {
                targetBranch.director.SetPreviousSequenceGroup(branchDictionary[newBranchName].sequence,
                                                           branchDictionary[newBranchName].directorObject,
                                                           branchDictionary[newBranchName].isOrigin);
            }
        }

        private static bool IsPopulated(List<BranchingPath> attribute)
        {
            if(attribute.Count >= 3 && attribute.Count < 5) {
                return true;
            } else {
                return false;
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(V3Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
    
}