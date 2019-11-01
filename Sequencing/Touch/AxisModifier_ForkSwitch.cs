using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{

    public static class AxisModifier_ForkSwitch
    {
        enum SequenceUpdateType { Next, Previous }

        public static AxisModifier_ForkExtents ActivateSwitch(double masterTime, AxisModifier_ForkExtents forkExtents)
        {
            Touch_Controller touchController = forkExtents.axisModifier.touchController;
                
            touchController.joinTools.SetForkStatus(true);
            touchController.ySwipeAxis.active = true;
            touchController.xSwipeAxis.active = true;

            AxisModifier_TouchBranchData activeBranch = GetActiveBranch(forkExtents);
                
            // If we are beyond the transition thresholds, set the adjacent sequence based on swipe input
            if (forkExtents.markerPlacement == MarkerPlacement.EndOfSequence && masterTime >= forkExtents.startTransitionThreshold ||
                forkExtents.markerPlacement == MarkerPlacement.StartOfSequence && masterTime <= forkExtents.startTransitionThreshold) {

                Vector2 swipeForce = touchController.GetDominantSwipeForce(touchController.swipeForce);
                    
                // Determine which direction we're swiping, then activate the new path accordingly
                if (Mathf.Abs(swipeForce.y) > Mathf.Abs(swipeForce.x)) {
                    ActivateXBranch(forkExtents, activeBranch);
                } else if (Mathf.Abs(swipeForce.x) > Mathf.Abs(swipeForce.y)) {
                    ActivateYBranch(forkExtents, activeBranch);
                }
                    
            }
                
            // Otherwise, reset the fork
            else {
                                                
                ResetBranchStates(forkExtents, new[] { BranchType.yPos, BranchType.yNeg, BranchType.xPos, BranchType.xNeg });

                touchController.joinTools.SetForkStatus(false);

                // Flip axes accordingly
                if (activeBranch.branchType == BranchType.yPos ||
                    activeBranch.branchType == BranchType.yNeg) {

                    touchController.ySwipeAxis.active = true;
                    touchController.yMomentumAxis.active = true;
                    touchController.xSwipeAxis.active = false;
                    touchController.xMomentumAxis.active = false;

                } else {

                    touchController.ySwipeAxis.active = false;
                    touchController.yMomentumAxis.active = false;
                    touchController.xSwipeAxis.active = true;
                    touchController.xMomentumAxis.active = true;
                }

            }
                
            return forkExtents;
        }
    
        private static AxisModifier_ForkExtents ActivateXBranch(AxisModifier_ForkExtents touchForkExtents, AxisModifier_TouchBranchData activeBranch)
        {
            ResetBranchStates(touchForkExtents, new[] { BranchType.xPos, BranchType.xNeg });

            // Due to the peculiarities of working with a timeline, we need to force
            // the input on any opposing branches to be positive for the fork to work properly,
            // otherwise erroneous negative input will prevent us from moving through it
            if (activeBranch.branchType == BranchType.yPos ||
                activeBranch.branchType == BranchType.yNeg) {
                if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
                    activeBranch.touchData.forceForward = true;
                } else {
                    activeBranch.touchData.forceBackward = true;
                }
            }

            Touch_Controller touchController = touchForkExtents.axisModifier.touchController;
                
            touchController.xMomentumAxis.active = true;
            touchController.yMomentumAxis.active = false;

            // Subtract the value of the y axis, otherwise our swipe is not smooth
            float sign = touchController.swipeModifierOutput >= 0 ? 1 : -1;
            touchController.swipeModifierOutput = (Mathf.Abs(touchController.swipeModifierOutput) - Mathf.Abs(touchController.swipeForce.y)) * sign;

            sign = touchController.momentumModifierOutput >= 0 ? 1 : -1;
            touchController.momentumModifierOutput = (Mathf.Abs(touchController.momentumModifierOutput) - Mathf.Abs(touchController.momentumForce.y)) * sign;

            // If the fork marker is at the end of a timeline, then we need to set the sequence and director that follow
            // the current sequence. If the marker is at the beginning of a timeline, then we need to
            // set the previous sequence and director so that we can rewind correctly
            if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {

                if (touchController.swipeForce.x > 0) {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xPos)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xPos, SequenceUpdateType.Next);
                    }
                } else {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xNeg)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xNeg, SequenceUpdateType.Next);
                    }
                }

            } else {

                if (touchController.swipeForce.x > 0) {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xPos)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xPos, SequenceUpdateType.Previous);
                    }
                } else {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xNeg)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xNeg, SequenceUpdateType.Previous);
                    }
                }

            }

            return touchForkExtents;
        }
            
        private static AxisModifier_ForkExtents ActivateYBranch(AxisModifier_ForkExtents touchForkExtents, AxisModifier_TouchBranchData activeBranch)
        {
            ResetBranchStates(touchForkExtents, new[] { BranchType.yPos, BranchType.yNeg });

            // Due to the peculiarities of working with a timeline, we need to force
            // the input on any opposing branches to be positive for the fork to work properly,
            // otherwise erroneous negative input will prevent us from moving through it
            if (activeBranch.branchType == BranchType.xPos ||
                activeBranch.branchType == BranchType.xNeg)
            {
                if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
                    activeBranch.touchData.forceForward = true;
                } else {
                    activeBranch.touchData.forceBackward = true;
                }
            }

            Touch_Controller touchController = touchForkExtents.axisModifier.touchController;

            touchController.yMomentumAxis.active = true;
            touchController.xMomentumAxis.active = false;

            // Subtract the value of the x axis, otherwise our swipe is not smooth
            float sign = touchController.swipeModifierOutput >= 0 ? 1 : -1;
            touchController.swipeModifierOutput = (Mathf.Abs(touchController.swipeModifierOutput) - Mathf.Abs(touchController.swipeForce.x)) * sign;

            sign = touchController.momentumModifierOutput >= 0 ? 1 : -1;
            touchController.momentumModifierOutput = (Mathf.Abs(touchController.momentumModifierOutput) - Mathf.Abs(touchController.momentumForce.x)) * sign;

            // If the fork marker is at the end of a timeline, then we need to set the sequence and director that follow
            // the current sequence. If the marker is at the beginning of a timeline, then we need to
            // set the previous sequence and director so that we can rewind correctly
            if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {

                if (touchController.swipeForce.y < 0) {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.yPos)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.yPos, SequenceUpdateType.Next);
                    }
                } else {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.yNeg)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.yNeg, SequenceUpdateType.Next);
                    }
                }

            } else {

                if (touchController.swipeForce.y < 0) {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.yPos)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.yPos, SequenceUpdateType.Previous);
                    }
                } else {
                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.yNeg)) {
                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.yNeg, SequenceUpdateType.Previous);
                    }
                }

            }

            return touchForkExtents;
        }
            
        private static AxisModifier_TouchBranchData UpdateBranchSibling(AxisModifier_TouchBranchData targetBranch, Dictionary<BranchType, AxisModifier_TouchBranchData> branchDictionary, BranchType newBranchType, SequenceUpdateType updateType)
        {
            // A branch's previous / next sibling cannot be itself, so cancel the update if this occurs
            if (targetBranch.branchType == newBranchType) {
                return targetBranch;
            }

            if (updateType == SequenceUpdateType.Next)
            {
                Sequence_ProcessModify.SetNextSequence(
                    targetBranch.touchData.sequence.sequenceConfig.processModify,
                    branchDictionary[newBranchType].sequence);
                    
            } else {
                Sequence_ProcessModify.SetPreviousSequence(
                    targetBranch.touchData.sequence.sequenceConfig.processModify,
                    branchDictionary[newBranchType].sequence);
            }

            return targetBranch;
        }

            
        private static AxisModifier_ForkExtents ResetBranchStates(AxisModifier_ForkExtents touchForkExtents, BranchType[] targetBranchTypes)
        {
            foreach (KeyValuePair<BranchType, AxisModifier_TouchBranchData> branchData in touchForkExtents.branchDictionary) {
                for (int q = 0; q < targetBranchTypes.Length; q++) {
                    if (branchData.Key == targetBranchTypes[q])
                    {
                        branchData.Value.touchData.forceForward = false;
                        branchData.Value.touchData.forceBackward = false;
                    }
                }
            }

            return touchForkExtents;
        }
            
        private static AxisModifier_TouchBranchData GetActiveBranch(AxisModifier_ForkExtents touchForkExtents)
        {
            foreach (var branchData in touchForkExtents.branchDictionary.Where(branchData => branchData.Value.sequence.active == true)) {
                return branchData.Value;
            }

            throw new SystemException("No active branches found");
        }
                        
    }
}