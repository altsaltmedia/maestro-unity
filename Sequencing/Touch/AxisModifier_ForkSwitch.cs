using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class ForkSwitch
        {
            [SerializeField]
            private ForkSwitchClip _sourceClip;

            private ForkSwitchClip sourceClip
            {
                get => _sourceClip;
                set => _sourceClip = value;
            }

            [SerializeField]
            private Dictionary<BranchType, BranchingPathSwitchData> _branchDictionary =
                new Dictionary<BranchType, BranchingPathSwitchData>();

            private Dictionary<BranchType, BranchingPathSwitchData> branchDictionary
            {
                get => _branchDictionary;
                set => _branchDictionary = value;
            }
            
            enum SequenceUpdateType { Next, Previous }

            public static ForkSwitch CreateInstance(TouchController touchController,
                TouchController.TouchData touchData, ForkSwitchClip sourceClip)
            {
                //var inputData = ScriptableObject.CreateInstance(typeof(ForkSwitch)) as ForkSwitch;
                
                var inputData = new ForkSwitch();

                inputData.touchController = touchController;
                inputData.touchData = touchData;
                inputData.sourceClip = sourceClip;
                inputData.inflectionPoint =
                    (float) MasterSequence.LocalToMasterTime(touchData.masterSequence,
                        touchData.sequence, sourceClip.endTime);

                for (int i = 0; i < sourceClip.branchingPaths.Count; i++)
                {
                    BranchingPath branchingPath = sourceClip.branchingPaths[i]; 
                    BranchingPathSwitchData branchingPathSwitchData;
                    
                    // If the branching path is equal to our current touch data, that means this sequence's
                    // playable asset is where the fork originated, and we can flag this as the origin point
                    // and set the touch data accordingly
                    if (branchingPath.sequence == touchData.sequence)
                    {
                        branchingPathSwitchData = new BranchingPathSwitchData(branchingPath.sequence, branchingPath.branchType, true, touchData);
                    }
                    // Otherwise, we must set the origin value to false and retrieve the touch data from the touch controller 
                    else
                    {
                        TouchController.TouchData siblingTouchData =
                            GetTouchDataFromBranch(touchController, branchingPath);
                        branchingPathSwitchData = new BranchingPathSwitchData(branchingPath.sequence, branchingPath.branchType, false, siblingTouchData);
                        
                    }

                    inputData.branchDictionary.Add(branchingPathSwitchData.branchType, branchingPathSwitchData);
                }

                return inputData;
            }

            public static ForkSwitch CheckActivateSwitch(BaseAxisSwitch axisSwitch, double masterTime)
            {
                ForkSwitch forkSwitch = axisSwitch as ForkSwitch;
                AxisModifier axisModifier = forkSwitch.touchController.axisModifier;

                if (IsBranchActive(forkSwitch.branchDictionary, out var activeBranch) == true)
                {
                    // Activate the fork if we're within the correct thresholds of the inflection point. If active branch is the origin,
                    // then this happens at the specified forkInflectionPoint, modified by the spread. If active branch is one of the destinations,
                    // then the fork activates at the branch's first frame, aka at 0 plus the spread.
                    if (activeBranch.isOrigin == true &&
                        masterTime >= forkSwitch.inflectionPoint - axisModifier.forkTransitionSpread
                        || activeBranch.isOrigin == false && masterTime <= axisModifier.forkTransitionSpread)
                    {
                        forkSwitch.touchController.forkActive = true;
                        forkSwitch.touchController.ySwipeAxis.active = true;
                        forkSwitch.touchController.xSwipeAxis.active = true;
                        
                        Vector2 swipeForce =
                            forkSwitch.touchController.GetDominantSwipeForce(forkSwitch.touchController.swipeForce);
                        
                        // Determine which direction we're swiping, then activate the new path accordingly
                        if (Mathf.Abs(swipeForce.y) > Mathf.Abs(swipeForce.x)) {
                            ActivateXBranch(forkSwitch, activeBranch);
                        } else if (Mathf.Abs(swipeForce.x) > Mathf.Abs(swipeForce.y)) {
                            ActivateYBranch(forkSwitch, activeBranch);
                        }

                    } else if (
                        masterTime < forkSwitch.inflectionPoint - axisModifier.forkTransitionSpread
                         && activeBranch.sequence.currentTime >= forkSwitch.inflectionPoint - (axisModifier.forkTransitionSpread + axisModifier.swipeResetSpread)
                            && activeBranch.isOrigin == true ||
                        activeBranch.sequence.currentTime > axisModifier.forkTransitionSpread &&
                         activeBranch.sequence.currentTime <= axisModifier.forkTransitionSpread + axisModifier.swipeResetSpread
                            && activeBranch.isOrigin == false) {
                            
                        ResetBranchStates(forkSwitch, new[] { BranchType.yPos, BranchType.yNeg, BranchType.xPos, BranchType.xNeg });

                        forkSwitch.touchController.forkActive = false;

                        // Flip axes accordingly
                        if (activeBranch.branchType == BranchType.yPos ||
                            activeBranch.branchType == BranchType.yNeg) {

                            forkSwitch.touchController.ySwipeAxis.active = true;
                            forkSwitch.touchController.yMomentumAxis.active = true;
                            forkSwitch.touchController.xSwipeAxis.active = false;
                            forkSwitch.touchController.xMomentumAxis.active = false;

                        } else {

                            forkSwitch.touchController.ySwipeAxis.active = false;
                            forkSwitch.touchController.yMomentumAxis.active = false;
                            forkSwitch.touchController.xSwipeAxis.active = true;
                            forkSwitch.touchController.xMomentumAxis.active = true;
                        }

                    }
                }

                return forkSwitch;
            }
            
            private static ForkSwitch ActivateYBranch(ForkSwitch forkSwitch, BranchingPathSwitchData activeBranch)
            {
                ResetBranchStates(forkSwitch, new[] { BranchType.yPos, BranchType.yNeg });

                // Due to the peculiarities of working with a timeline, we need to force
                // the input on any opposing branches to be positive for the fork to work properly,
                // otherwise erroneous negative input will prevent us from moving through it
                if (activeBranch.branchType == BranchType.xPos ||
                   activeBranch.branchType == BranchType.xNeg)
                {
                    if (activeBranch.isOrigin == true) {
                        activeBranch.touchData.forceForward = true;
                    } else {
                        activeBranch.touchData.forceBackward = true;
                    }
                }

                TouchController touchController = forkSwitch.touchController;

                touchController.yMomentumAxis.active = true;
                touchController.xMomentumAxis.active = false;

                // Subtract the value of the x axis, otherwise our swipe is not smooth
                float sign = touchController.swipeModifierOutput >= 0 ? 1 : -1;
                touchController.swipeModifierOutput = (Mathf.Abs(touchController.swipeModifierOutput) - Mathf.Abs(touchController.swipeForce.x)) * sign;

                sign = touchController.momentumModifierOutput >= 0 ? 1 : -1;
                touchController.momentumModifierOutput = (Mathf.Abs(touchController.momentumModifierOutput) - Mathf.Abs(touchController.momentumForce.x)) * sign;

                // If we're on the origin branch, then we need to set the sequence and director that follow
                // the current sequence. If we're on a subsequent branch, then we need to set the previous sequence
                // and director so that we can rewind correctly
                if (activeBranch.isOrigin == true) {

                    if (touchController.swipeForce.y < 0) {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.yPos)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.yPos, SequenceUpdateType.Next);
                        }
                    } else {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.yNeg)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.yNeg, SequenceUpdateType.Next);
                        }
                    }

                } else {

                    if (touchController.swipeForce.y < 0) {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.yPos)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.yPos, SequenceUpdateType.Previous);
                        }
                    } else {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.yNeg)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.yNeg, SequenceUpdateType.Previous);
                        }
                    }

                }

                return forkSwitch;
            }

            private static ForkSwitch ActivateXBranch(ForkSwitch forkSwitch, BranchingPathSwitchData activeBranch)
            {
                ResetBranchStates(forkSwitch, new[] { BranchType.xPos, BranchType.xNeg });

                // Due to the peculiarities of working with a timeline, we need to force
                // the input on any opposing branches to be positive for the fork to work properly,
                // otherwise erroneous negative input will prevent us from moving through it
                if (activeBranch.branchType == BranchType.yPos ||
                    activeBranch.branchType == BranchType.yNeg) {
                    if (activeBranch.isOrigin == true) {
                        activeBranch.touchData.forceForward = true;
                    } else {
                        activeBranch.touchData.forceBackward = true;
                    }
                }

                TouchController touchController = forkSwitch.touchController;
                
                touchController.xMomentumAxis.active = true;
                touchController.yMomentumAxis.active = false;

                // Subtract the value of the y axis, otherwise our swipe is not smooth
                float sign = touchController.swipeModifierOutput >= 0 ? 1 : -1;
                touchController.swipeModifierOutput = (Mathf.Abs(touchController.swipeModifierOutput) - Mathf.Abs(touchController.swipeForce.y)) * sign;

                sign = touchController.momentumModifierOutput >= 0 ? 1 : -1;
                touchController.momentumModifierOutput = (Mathf.Abs(touchController.momentumModifierOutput) - Mathf.Abs(touchController.momentumForce.y)) * sign;

                // If we're on the origin branch, then we need to set the sequence and director that follow
                // the current sequence. If we're on a subsequent branch, then we need to set the previous sequence
                // and director so that we can rewind correctly
                if (activeBranch.isOrigin == true) {

                    if (touchController.swipeForce.x > 0) {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.xPos)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.xPos, SequenceUpdateType.Next);
                        }
                    } else {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.xNeg)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.xNeg, SequenceUpdateType.Next);
                        }
                    }

                } else {

                    if (touchController.swipeForce.x > 0) {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.xPos)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.xPos, SequenceUpdateType.Previous);
                        }
                    } else {
                        if (forkSwitch.branchDictionary.ContainsKey(BranchType.xNeg)) {
                            UpdateBranchSibling(activeBranch, forkSwitch.branchDictionary,BranchType.xNeg, SequenceUpdateType.Previous);
                        }
                    }

                }

                return forkSwitch;
            }
            
            private static BranchingPath UpdateBranchSibling(BranchingPathSwitchData targetBranch, Dictionary<BranchType, BranchingPathSwitchData> branchDictionary, BranchType newBranchType, SequenceUpdateType updateType)
            {
                // A branch's previous / next sibling cannot be itself, so cancel the update if this occurs
                if (targetBranch.branchType == newBranchType) {
                    return targetBranch;
                }

                if (updateType == SequenceUpdateType.Next)
                {
                    ProcessModifySequence.SetNextSequence(
                        targetBranch.touchData.sequence.sequenceConfig.processModifySequence,
                        branchDictionary[newBranchType].sequence);
                    
                } else {
                    ProcessModifySequence.SetPreviousSequence(
                        targetBranch.touchData.sequence.sequenceConfig.processModifySequence,
                        branchDictionary[newBranchType].sequence);
                }

                return targetBranch;
            }

            public static TouchController.TouchData GetTouchDataFromBranch(TouchController touchController, BranchingPath branchingPath)
            {
                for (int i = 0; i < touchController.touchDataList.Count; i++)
                {
                    TouchController.TouchData touchData = touchController.touchDataList[i];
                    if (touchData.sequence == branchingPath.sequence)
                    {
                        return touchData;
                    }
                }
                
                throw new System.Exception("Target branching path not found in touch controller's data list.");
            }
            
            
            private static ForkSwitch ResetBranchStates(ForkSwitch forkSwitch, BranchType[] targetBranchTypes)
            {
                foreach (KeyValuePair<BranchType, BranchingPathSwitchData> branchData in forkSwitch.branchDictionary) {
                    for (int q = 0; q < targetBranchTypes.Length; q++) {
                        if (branchData.Key == targetBranchTypes[q])
                        {
                            branchData.Value.touchData.forceForward = false;
                            branchData.Value.touchData.forceBackward = false;
                        }
                    }
                }

                return forkSwitch;
            }

            private static bool IsBranchActive(Dictionary<BranchType, BranchingPathSwitchData> branchingPaths,
                out BranchingPathSwitchData activeBranch)
            {
                activeBranch = null;

                foreach (KeyValuePair<BranchType, BranchingPathSwitchData> branchData in branchingPaths)
                {
                    if (branchData.Value.sequence.active == true)
                    {
                        activeBranch = branchData.Value;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}