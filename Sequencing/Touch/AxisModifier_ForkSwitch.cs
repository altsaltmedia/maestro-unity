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

//            ResetBranchStates(forkExtents, new[] { BranchType.yNorth, BranchType.ySouth, BranchType.xEast, BranchType.xWest });
            
            AxisModifier_TouchBranchData activeBranch = GetActiveBranch(forkExtents);
                
            // If we are beyond the transition thresholds, set the adjacent sequence based on swipe input
            if (forkExtents.markerPlacement == MarkerPlacement.EndOfSequence && masterTime >= forkExtents.startTransitionThreshold ||
                forkExtents.markerPlacement == MarkerPlacement.StartOfSequence && masterTime <= forkExtents.startTransitionThreshold) {

                Vector2 swipeForce = touchController.GetDominantSwipeForce(touchController.swipeForce);
                    
                // Determine which direction we're swiping, then activate the new path accordingly
                if (Mathf.Abs(swipeForce.y) > Mathf.Abs(swipeForce.x)) {
                    ActivateYBranch(forkExtents, activeBranch);
                } else if (Mathf.Abs(swipeForce.x) > Mathf.Abs(swipeForce.y)) {
                    ActivateXBranch(forkExtents, activeBranch);
                }
                    
            }
                
            // Otherwise, reset the fork
            else {

                touchController.joinTools.SetForkStatus(false);

                // Flip axes accordingly
                if (activeBranch.branchType == BranchType.yNorth ||
                    activeBranch.branchType == BranchType.ySouth) {

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
            ResetTargetBranchStates(touchForkExtents, new[] { BranchType.xEast, BranchType.xWest });

            // Due to the peculiarities of working with a timeline, we need to force
            // the input on any opposing branches to be positive for the fork to work properly,
//            // otherwise erroneous negative input will prevent us from moving through it
            if (activeBranch.branchType == BranchType.yNorth ||
                activeBranch.branchType == BranchType.ySouth) {
                if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
                    activeBranch.touchData.forceForward = true;
                } else {
                    activeBranch.touchData.forceBackward = true;
                }
            }

            Touch_Controller touchController = touchForkExtents.axisModifier.touchController;
                
            touchController.xMomentumAxis.active = true;
            touchController.yMomentumAxis.active = false;
            
            if (activeBranch.branchType == BranchType.xEast ||
                activeBranch.branchType == BranchType.xWest) {
                touchController.xSwipeAxis.inverted = activeBranch.invert;
                touchController.xMomentumAxis.inverted = activeBranch.invert;
            }

            // Subtract the value of the y axis, otherwise our swipe is not smooth
            float sign = touchController.swipeModifierOutput >= 0 ? 1 : -1;
            touchController.swipeModifierOutput = (Mathf.Abs(touchController.swipeModifierOutput) - Mathf.Abs(touchController.swipeForce.y)) * sign;

            sign = touchController.momentumModifierOutput >= 0 ? 1 : -1;
            touchController.momentumModifierOutput = (Mathf.Abs(touchController.momentumModifierOutput) - Mathf.Abs(touchController.momentumForce.y)) * sign;

            // If the fork marker is at the end of a timeline, then we need to set the sequence and director that follow
            // the current sequence. If the marker is at the beginning of a timeline, then we need to
            // set the previous sequence and director so that we can rewind correctly
            
            if (touchController.swipeForce.x > 0) {
                if (activeBranch.branchType == BranchType.xEast) {
                    ResetTargetBranchStates(touchForkExtents, new[] { BranchType.xEast });
                    //touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.xWest]);
                }
                else {
                    //SetBranchTransitionStates(BranchType.xEast, touchForkExtents);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.xEast]);
//                    UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary, BranchType.xEast,
//                        touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence
//                            ? SequenceUpdateType.Next
//                            : SequenceUpdateType.Previous);
                }
            }
            else {
                if (activeBranch.branchType == BranchType.xWest) {
                    ResetTargetBranchStates(touchForkExtents, new[] { BranchType.xWest });
                    //touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.xEast]);
                }
                else {
                    //SetBranchTransitionStates(BranchType.xWest, touchForkExtents);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.xWest]);
//                    UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary, BranchType.xWest,
//                        touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence
//                            ? SequenceUpdateType.Next
//                            : SequenceUpdateType.Previous);
                }
            }
            
            
//            if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
//
//                if (touchController.swipeForce.x > 0) {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xEast)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xEast, SequenceUpdateType.Next);
//                    }
//                } else {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xWest)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xWest, SequenceUpdateType.Next);
//                    }
//                }
//
//            } else {
//
//                if (touchController.swipeForce.x > 0) {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xEast)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xEast, SequenceUpdateType.Previous);
//                    }
//                } else {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.xWest)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.xWest, SequenceUpdateType.Previous);
//                    }
//                }
//
//            }

            return touchForkExtents;
        }
            
        private static AxisModifier_ForkExtents ActivateYBranch(AxisModifier_ForkExtents touchForkExtents, AxisModifier_TouchBranchData activeBranch)
        {
            ResetTargetBranchStates(touchForkExtents, new[] { BranchType.yNorth, BranchType.ySouth });

            // Due to the peculiarities of working with a timeline, we need to force
            // the input on any opposing branches to be positive for the fork to work properly,
            // otherwise erroneous negative input will prevent us from moving through it
            
            if (activeBranch.branchType == BranchType.xEast ||
                activeBranch.branchType == BranchType.xWest)
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
            
            if (activeBranch.branchType == BranchType.yNorth ||
                activeBranch.branchType == BranchType.ySouth) {
                touchController.ySwipeAxis.inverted = activeBranch.invert;
                touchController.yMomentumAxis.inverted = activeBranch.invert;
            }

            // Subtract the value of the x axis, otherwise our swipe is not smooth
            float sign = touchController.swipeModifierOutput >= 0 ? 1 : -1;
            touchController.swipeModifierOutput = (Mathf.Abs(touchController.swipeModifierOutput) - Mathf.Abs(touchController.swipeForce.x)) * sign;

            sign = touchController.momentumModifierOutput >= 0 ? 1 : -1;
            touchController.momentumModifierOutput = (Mathf.Abs(touchController.momentumModifierOutput) - Mathf.Abs(touchController.momentumForce.x)) * sign;

//            // If the fork marker is at the end of a timeline, then we need to set the sequence and director that follow
            // the current sequence. If the marker is at the beginning of a timeline, then we need to
            // set the previous sequence and director so that we can rewind correctly
            if (touchController.swipeForce.y < 0) {
                if (activeBranch.branchType == BranchType.yNorth) {
                    ResetTargetBranchStates(touchForkExtents, new[] { BranchType.yNorth });
                    //touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.ySouth]);
                }
                else {
                    //SetBranchTransitionStates(BranchType.yNorth, touchForkExtents);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.yNorth]);
                    
//                    UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary, BranchType.yNorth,
//                        touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence
//                            ? SequenceUpdateType.Next
//                            : SequenceUpdateType.Previous);
                }
            }
            else {
                if (activeBranch.branchType == BranchType.ySouth) {
                    ResetTargetBranchStates(touchForkExtents, new[] { BranchType.ySouth });
                    //touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.yNorth]);
                }
                else {
                    //SetBranchTransitionStates(BranchType.ySouth, touchForkExtents);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.branchDictionary[BranchType.ySouth]);
//                    UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary, BranchType.ySouth,
//                        touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence
//                            ? SequenceUpdateType.Next
//                            : SequenceUpdateType.Previous);
                }
            }


//            if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
//
//                if (touchController.swipeForce.y > 0) {
//                    
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.ySouth)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.ySouth, SequenceUpdateType.Next);
//                    }
//                } else {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.yNorth)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.yNorth, SequenceUpdateType.Next);
//                    }
//                }
//
//            } else {
//
//                if (touchController.swipeForce.y > 0) {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.ySouth)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.ySouth, SequenceUpdateType.Previous);
//                    }
//                } else {
//                    if (touchForkExtents.branchDictionary.ContainsKey(BranchType.yNorth)) {
//                        UpdateBranchSibling(activeBranch, touchForkExtents.branchDictionary,BranchType.yNorth, SequenceUpdateType.Previous);
//                    }
//                }
//
//            }

            return touchForkExtents;
        }

        private static void SetBranchTransitionStates(BranchType activateType, AxisModifier_ForkExtents touchForkExtents)
        {
            for (int i = 0; i < touchForkExtents.axisModifier.touchController.rootConfig.joinTools.forkDataList.Count; i++) {
                JoinTools_ForkJoinData forkJoinData =
                     touchForkExtents.axisModifier.touchController.rootConfig.joinTools.forkDataList[i];
                if (forkJoinData.fork == touchForkExtents.touchFork) {
                    Touch_Data touchData =
                        touchForkExtents.axisModifier.touchController.touchDataList.Find(x =>
                            x.sequence == forkJoinData.sequence);
                    foreach (var branch in touchForkExtents.branchDictionary) {
                        if (branch.Value.sequence == touchData.sequence && branch.Key != activateType) {
                            if (forkJoinData.markerPlacement == MarkerPlacement.EndOfSequence) {
                                touchData.forceForward = true;
                            }
                            else {
                                touchData.forceBackward = true;
                            }
                        }
                    }
                    
                }
            }
            
//            foreach (KeyValuePair<BranchType, AxisModifier_TouchBranchData> branchData in touchForkExtents.branchDictionary) {
//                
////                if (branchData.Value. == targetBranchTypes[q])
////                {
////                    branchData.Value.touchData.forceForward = false;
//                    branchData.Value.touchData.forceBackward = true;
//                //}
//            }
        }
            
        private static AxisModifier_TouchBranchData UpdateBranchSibling(AxisModifier_TouchBranchData targetBranch, Dictionary<BranchType, AxisModifier_TouchBranchData> branchDictionary, BranchType newBranchType, SequenceUpdateType updateType)
        {
            // A branch's previous / next sibling cannot be itself, so cancel the update if this occurs
            if (targetBranch.branchType == newBranchType) {
                return targetBranch;
            }
            
            RootConfig rootConfig = targetBranch.sequence.sequenceConfig.masterSequence.rootConfig;
            if (updateType == SequenceUpdateType.Next)
            {
                rootConfig.joinTools.SetNextSequence(targetBranch.sequence,
                    branchDictionary[newBranchType].sequence);
                    
            } else {
                rootConfig.joinTools.SetPreviousSequence( targetBranch.sequence,
                    branchDictionary[newBranchType].sequence);
            }

            return targetBranch;
        }

        public static AxisModifier_ForkExtents RefreshBranchStates(AxisModifier_ForkExtents touchForkExtents)
        {
            AxisModifier_TouchBranchData activeBranch = GetActiveBranch(touchForkExtents);
            Vector2 swipeForce = touchForkExtents.axisModifier.touchController.GetDominantSwipeForce(touchForkExtents.axisModifier.touchController.swipeForce);
            
            if (Mathf.Abs(swipeForce.y) > Mathf.Abs(swipeForce.x)) {
                
                ResetTargetBranchStates(touchForkExtents, new[] { BranchType.yNorth, BranchType.ySouth });
                
                if (activeBranch.branchType == BranchType.xEast ||
                    activeBranch.branchType == BranchType.xWest)
                {
                    if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
                        activeBranch.touchData.forceForward = true;
                    } else {
                        activeBranch.touchData.forceBackward = true;
                    }
                }
                
            } else if (Mathf.Abs(swipeForce.x) > Mathf.Abs(swipeForce.y)) {
                
                ResetTargetBranchStates(touchForkExtents, new[] { BranchType.xEast, BranchType.xWest });
                
                if (activeBranch.branchType == BranchType.yNorth ||
                    activeBranch.branchType == BranchType.ySouth) {
                    if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence) {
                        activeBranch.touchData.forceForward = true;
                    } else {
                        activeBranch.touchData.forceBackward = true;
                    }
                }
            }

            return touchForkExtents;
        }

        public static AxisModifier_ForkExtents ResetTargetBranchStates(AxisModifier_ForkExtents touchForkExtents, BranchType[] targetBranchTypes)
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