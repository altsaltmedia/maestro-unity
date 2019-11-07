using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{

    public static class TouchForkUtils
    {
        public static TouchForkExtents ActivateTouchFork(double masterTime, TouchForkExtents touchForkExtents)
        {
            Touch_Controller touchController = touchForkExtents.axisMonitor.touchController;

            TouchBranchingPathData activeBranch = GetActiveBranch(touchForkExtents);
                
            // If we are beyond the transition thresholds, set the adjacent sequence based on swipe input
            if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence && masterTime >= touchForkExtents.startTransitionThreshold ||
                touchForkExtents.markerPlacement == MarkerPlacement.StartOfSequence && masterTime <= touchForkExtents.startTransitionThreshold) {
                
                return SetTransitionState(touchController, touchForkExtents, activeBranch);
                
            }

            return SetDefaultState(touchController, touchForkExtents, activeBranch);
        }

        private static TouchForkExtents SetTransitionState(Touch_Controller touchController, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            touchController.joiner.SetForkStatus(true);
            
            touchController.ySwipeAxis.active = true;
            touchController.yMomentumAxis.active = true;
            touchController.xSwipeAxis.active = true;
            touchController.xMomentumAxis.active = true;
                
            switch (touchController.swipeDirection) {
                    
                case nameof(SwipeDirection.yPositive):
                    UpdateBranchStates(AxisType.Y, touchForkExtents, activeBranch);
                    UpdateTouchVariables(AxisType.Y, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.ySouthKey);
                    break;
                    
                case nameof(SwipeDirection.yNegative):
                    UpdateBranchStates(AxisType.Y, touchForkExtents, activeBranch);
                    UpdateTouchVariables(AxisType.Y, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.yNorthKey);
                    break;
                    
                case nameof(SwipeDirection.xPositive):
                    UpdateBranchStates(AxisType.X, touchForkExtents, activeBranch);
                    UpdateTouchVariables(AxisType.X, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.xEastKey);
                    break;
                    
                case nameof(SwipeDirection.xNegative):
                    UpdateBranchStates(AxisType.X, touchForkExtents, activeBranch);
                    UpdateTouchVariables(AxisType.X, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.xWestKey);
                    break;
            }

            return touchForkExtents;
        }

        private static TouchForkExtents SetDefaultState(Touch_Controller touchController, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            touchController.joiner.SetForkStatus(false);

            // Flip axes accordingly
            if (activeBranch.branchKey == touchForkExtents.axisMonitor.yNorthKey ||
                activeBranch.branchKey == touchForkExtents.axisMonitor.ySouthKey) {

                touchController.ySwipeAxis.active = true;
                touchController.ySwipeAxis.inverted = activeBranch.invert;
                touchController.yMomentumAxis.active = true;
                touchController.yMomentumAxis.inverted = activeBranch.invert;
                
                touchController.xSwipeAxis.active = false;
                touchController.xMomentumAxis.active = false;

            } else {

                touchController.ySwipeAxis.active = false;
                touchController.yMomentumAxis.active = false;
                
                touchController.xSwipeAxis.active = true;
                touchController.xSwipeAxis.inverted = activeBranch.invert;
                touchController.xMomentumAxis.active = true;
                touchController.xMomentumAxis.inverted = activeBranch.invert;
            }

            return touchForkExtents;
        }
        
        private static TouchBranchingPathData GetActiveBranch(TouchForkExtents touchForkExtents)
        {
            foreach (KeyValuePair<BranchKey, TouchBranchingPathData>  branchData in
                touchForkExtents.branchDictionary.Where(branchData => branchData.Value.sequence.active == true)) {
                return branchData.Value;
            }

            throw new DataMisalignedException("No active branches found. Did you set up your fork correctly?");
        }

        private static TouchForkExtents UpdateBranchStates(AxisType axisType, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            // Due to the peculiarities of working with a timeline, we need to force
            // the input on opposing branches to move toward the fork so that a user can swipe
            // through the transition smoothly; i.e. if the user is swiping along the Y axis
            // but we are currently on the X branch, we need to make the X branch advance to
            // the transition point regardless of input; also, if either of the Y axes had previously
            // been put into the transition state, they need to be reset.
            if (axisType == AxisType.Y) {
                SetTransitionState(activeBranch, touchForkExtents,
                    new[] {touchForkExtents.axisMonitor.xEastKey, touchForkExtents.axisMonitor.xWestKey});
                ResetBranches(touchForkExtents,
                    new[] {touchForkExtents.axisMonitor.yNorthKey, touchForkExtents.axisMonitor.ySouthKey});
            }
            else {
                SetTransitionState(activeBranch, touchForkExtents,
                    new[] {touchForkExtents.axisMonitor.yNorthKey, touchForkExtents.axisMonitor.ySouthKey});
                ResetBranches(touchForkExtents, 
                    new[] { touchForkExtents.axisMonitor.xEastKey, touchForkExtents.axisMonitor.xWestKey });
            }

            return touchForkExtents;
        }
        
        private static TouchForkExtents SetTransitionState(TouchBranchingPathData activeBranch, TouchForkExtents touchForkExtents, BranchKey[] targetBranchKeys)
        {
            for (int q = 0; q < targetBranchKeys.Length; q++) {

                if (activeBranch.branchKey == targetBranchKeys[q]) {

                    // Get the placement of the active branch's marker by traversing up to the joiner
                    // and filtering via the active branch's sequence and the fork of our active extents 
                    MarkerPlacement markerPlacement = touchForkExtents.axisMonitor.touchController.joiner.forkDataCollection[activeBranch.sequence]
                        .Find(forkData => forkData.fork == touchForkExtents.touchFork).markerPlacement;
                    
                    if (markerPlacement == MarkerPlacement.EndOfSequence) {
                        activeBranch.touchData.forceForward = true;
                    }
                    else {
                        activeBranch.touchData.forceBackward = true;
                    }
                    
                }
            }

            return touchForkExtents;
        }
        
        public static TouchForkExtents ResetAllBranches(TouchForkExtents touchForkExtents)
        {
            ResetBranches(touchForkExtents, new[] { touchForkExtents.axisMonitor.yNorthKey, touchForkExtents.axisMonitor.ySouthKey,
                touchForkExtents.axisMonitor.xEastKey, touchForkExtents.axisMonitor.xWestKey });

            return touchForkExtents;
        }

        private static TouchForkExtents ResetBranches(TouchForkExtents touchForkExtents, BranchKey[] targetBranchTypes)
        {
            for (int q = 0; q < targetBranchTypes.Length; q++) {
                
                foreach (KeyValuePair<BranchKey, TouchBranchingPathData> branchData in
                    touchForkExtents.branchDictionary.Where(branchData => branchData.Key == targetBranchTypes[q]) ) {

                    branchData.Value.touchData.forceForward = false;
                    branchData.Value.touchData.forceBackward = false;
                
                }
            }

            return touchForkExtents;
        }

        private static TouchForkExtents UpdateTouchVariables(AxisType axisType, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            Touch_Controller touchController = touchForkExtents.axisMonitor.touchController;
            
            if (axisType == AxisType.Y) {
                if (activeBranch.branchKey == touchForkExtents.axisMonitor.yNorthKey ||
                    activeBranch.branchKey == touchForkExtents.axisMonitor.ySouthKey) {
                    touchController.ySwipeAxis.inverted = activeBranch.invert;
                    touchController.yMomentumAxis.inverted = activeBranch.invert;
                }
            }
            
            else {
                if (activeBranch.branchKey == touchForkExtents.axisMonitor.xEastKey ||
                    activeBranch.branchKey == touchForkExtents.axisMonitor.xWestKey) {
                    touchController.xSwipeAxis.inverted = activeBranch.invert;
                    touchController.xMomentumAxis.inverted = activeBranch.invert;
                }
            }
            
            return touchForkExtents;
        }

    }
}