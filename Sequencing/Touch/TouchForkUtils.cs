using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{

    public static class TouchForkUtils
    {
        public static TouchExtents ActivateTouchFork(double masterTime, TouchForkExtents touchForkExtents)
        {
            Touch_Controller touchController = touchForkExtents.axisMonitor.touchController;

            TouchBranchingPathData activeBranch = GetActiveBranch(touchController, touchForkExtents);
                
            // If we are beyond the transition thresholds, set the adjacent sequence based on swipe input
            if (touchForkExtents.markerPlacement == MarkerPlacement.EndOfSequence && masterTime >= touchForkExtents.startTransitionThreshold ||
                touchForkExtents.markerPlacement == MarkerPlacement.StartOfSequence && masterTime <= touchForkExtents.startTransitionThreshold) {
                
                return SetForkTransitionState(touchController, touchForkExtents, activeBranch);
                
            }

            if (touchForkExtents.markerPlacement == MarkerPlacement.StartOfSequence && masterTime >= touchForkExtents.endTransitionThreshold) {

                if (touchForkExtents.nextTouchExtents is AxisExtents nextAxisExtents) {
                    return SetAxisTransitionState(touchController, nextAxisExtents); 
                }

            }

            return SetDefaultState(touchController, touchForkExtents, activeBranch);
        }

        private static TouchForkExtents SetForkTransitionState(Touch_Controller touchController, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            touchController.joiner.SetForkStatus(true);
            
            GameObject axisMonitorObject = touchController.axisMonitor.gameObject;

            touchController.ySwipeAxis.SetStatus(axisMonitorObject, true);
            touchController.yMomentumAxis.SetStatus(axisMonitorObject, true);
            touchController.xSwipeAxis.SetStatus(axisMonitorObject, true);
            touchController.xMomentumAxis.SetStatus(axisMonitorObject, true);
                
            switch (touchController.swipeDirection) {
                    
                case nameof(SwipeDirection.yPositive):
                    UpdateBranchStates(AxisType.Y, touchForkExtents, activeBranch);
                    UpdateTouchVariables(SwipeDirection.yPositive, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.ySouthKey);
                    break;
                    
                case nameof(SwipeDirection.yNegative):
                    UpdateBranchStates(AxisType.Y, touchForkExtents, activeBranch);
                    UpdateTouchVariables(SwipeDirection.yNegative, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.yNorthKey);
                    break;
                    
                case nameof(SwipeDirection.xPositive):
                    UpdateBranchStates(AxisType.X, touchForkExtents, activeBranch);
                    UpdateTouchVariables(SwipeDirection.xPositive, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.xEastKey);
                    break;
                    
                case nameof(SwipeDirection.xNegative):
                    UpdateBranchStates(AxisType.X, touchForkExtents, activeBranch);
                    UpdateTouchVariables(SwipeDirection.xNegative, touchForkExtents, activeBranch);
                    touchForkExtents.touchFork.SetDestinationBranch(touchForkExtents.axisMonitor.xWestKey);
                    break;
            }

            return touchForkExtents;
        }

        private static AxisExtents SetAxisTransitionState(Touch_Controller touchController, AxisExtents axisExtents)
        {
            touchController.axisMonitor.SetTransitionStatus(true);
            
            GameObject axisMonitorObject = touchController.axisMonitor.gameObject;
            
            axisExtents.swipeAxis.SetStatus(axisMonitorObject, true);
            axisExtents.momentumAxis.SetStatus(axisMonitorObject, true);

            return axisExtents;
        }

        private static TouchForkExtents SetDefaultState(Touch_Controller touchController, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            touchController.joiner.SetForkStatus(false);
            touchController.axisMonitor.SetTransitionStatus(false);

            GameObject axisMonitorObject = touchController.axisMonitor.gameObject;

            // Flip axes accordingly
            if (activeBranch.branchKey == touchForkExtents.axisMonitor.yNorthKey ||
                activeBranch.branchKey == touchForkExtents.axisMonitor.ySouthKey) {

                touchController.ySwipeAxis.SetStatus(axisMonitorObject, true);
                touchController.ySwipeAxis.SetInverted(axisMonitorObject, activeBranch.invert);
                touchController.yMomentumAxis.SetStatus(axisMonitorObject, true);
                touchController.yMomentumAxis.SetInverted(axisMonitorObject, activeBranch.invert);
                
                touchController.xSwipeAxis.SetStatus(axisMonitorObject, true);
                touchController.xMomentumAxis.SetStatus(axisMonitorObject, true);

            } else {

                touchController.ySwipeAxis.SetStatus(axisMonitorObject, true);
                touchController.yMomentumAxis.SetStatus(axisMonitorObject, true);
                
                touchController.xSwipeAxis.SetStatus(axisMonitorObject, true);
                touchController.xSwipeAxis.SetInverted(axisMonitorObject, activeBranch.invert);
                touchController.xMomentumAxis.SetStatus(axisMonitorObject, true);
                touchController.xMomentumAxis.SetInverted(axisMonitorObject, activeBranch.invert);
            }

            return touchForkExtents;
        }
        
        public static TouchBranchingPathData GetActiveBranch(Touch_Controller touchController, TouchForkExtents touchForkExtents)
        {
            List<MasterSequence> masterSequences = touchController.rootConfig.masterSequences;
            for (int i = 0; i < masterSequences.Count; i++) {
                if (masterSequences[i].hasActiveSequence == true) {
                    foreach (KeyValuePair<BranchKey, TouchBranchingPathData> branchData in
                        touchForkExtents.branchDictionary.Where(branchData =>
                            masterSequences[i].sequenceConfigs.Find(x => x.sequence == branchData.Value.sequence))) {
                        return branchData.Value;
                    }
                }
            }
            
            throw new DataMisalignedException("No active sequence found. Did you set up your fork correctly?");
        }

        private static TouchForkExtents UpdateBranchStates(AxisType axisType, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            // Due to the peculiarities of working with a timeline, we need to force
            // the input on opposing branches to move toward the fork so that a user can swipe
            // through the transition smoothly; i.e. if the user is swiping along the Y axis
            // but we are currently on the X branch, we need to make the X branch advance to
            // the transition point regardless of input; also, if either of the Y axes had previously
            // been put into the transition state, they need to be reset. The opposite applies if the user
            // is moving along the X axis.
            if (axisType == AxisType.Y) {
                SetBranchOverrides(activeBranch, touchForkExtents,
                    new[] {touchForkExtents.axisMonitor.xEastKey, touchForkExtents.axisMonitor.xWestKey});
                ResetBranches(touchForkExtents,
                    new[] {touchForkExtents.axisMonitor.yNorthKey, touchForkExtents.axisMonitor.ySouthKey});
            }
            else {
                SetBranchOverrides(activeBranch, touchForkExtents,
                    new[] {touchForkExtents.axisMonitor.yNorthKey, touchForkExtents.axisMonitor.ySouthKey});
                ResetBranches(touchForkExtents, 
                    new[] { touchForkExtents.axisMonitor.xEastKey, touchForkExtents.axisMonitor.xWestKey });
            }

            return touchForkExtents;
        }
        
        private static TouchForkExtents SetBranchOverrides(TouchBranchingPathData activeBranch, TouchForkExtents touchForkExtents, BranchKey[] targetBranchKeys)
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

        private static TouchForkExtents UpdateTouchVariables(SwipeDirection swipeDirection, TouchForkExtents touchForkExtents, TouchBranchingPathData activeBranch)
        {
            Touch_Controller touchController = touchForkExtents.axisMonitor.touchController;
            GameObject axisMonitorObject = touchController.axisMonitor.gameObject;
            
            if (activeBranch.branchKey == touchForkExtents.axisMonitor.yNorthKey ||
                activeBranch.branchKey == touchForkExtents.axisMonitor.ySouthKey) {
                touchController.ySwipeAxis.SetInverted(axisMonitorObject, activeBranch.invert);
                touchController.yMomentumAxis.SetInverted(axisMonitorObject, activeBranch.invert);

                Touch_Controller.RefreshIsReversing(touchController, swipeDirection, touchController.yMomentumAxis.GetVariable() as Axis);
            }
            
            else if (activeBranch.branchKey == touchForkExtents.axisMonitor.xEastKey ||
                activeBranch.branchKey == touchForkExtents.axisMonitor.xWestKey) {
                touchController.xSwipeAxis.SetInverted(axisMonitorObject, activeBranch.invert);
                touchController.xMomentumAxis.SetInverted(axisMonitorObject, activeBranch.invert);
                
                Touch_Controller.RefreshIsReversing(touchController, swipeDirection, touchController.xMomentumAxis.GetVariable() as Axis);
            }

            return touchForkExtents;
        }

    }
}