using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{

    public static class AxisUtils
    {
        public static TouchExtents ActivateAxisExtents(double masterTime, AxisExtents activeExtents)
        {
            GameObject axisMonitorObject = activeExtents.axisMonitor.gameObject; 
            
            // Since we are currently within the extents, set the swipe to true
            activeExtents.swipeAxis.SetInverted(axisMonitorObject, activeExtents.inverted);
            activeExtents.momentumAxis.SetInverted(axisMonitorObject, activeExtents.inverted);
            activeExtents.swipeAxis.SetStatus(axisMonitorObject, true);
            activeExtents.momentumAxis.SetStatus(axisMonitorObject,  true);
            
            // If we are beyond the transition thresholds, that means this switch
            // is fully active; we can just activate the momentum, deactivate the
            // adjacent extents and avoid further calculation
            if (masterTime > activeExtents.startTransitionThreshold &&
                masterTime < activeExtents.endTransitionThreshold) {

                return SetDefaultState(activeExtents);
            }

            // Check transition thresholds to either activate or deactivate adjacent extents
            if (activeExtents.previousTouchExtents != null) {
                    
                if (masterTime <= activeExtents.startTransitionThreshold) {

                    switch (activeExtents.previousTouchExtents) {
                        
                        case AxisExtents previousAxisExtents:
                            return SetAxisTransitionState(activeExtents, previousAxisExtents);

                        case TouchForkExtents previousForkExtents:
                            return SetForkTransitionState(activeExtents.axisMonitor.touchController, previousForkExtents);
                        
                    }
                }
            }

            if (activeExtents.nextTouchExtents != null) {
                    
                if (masterTime >= activeExtents.endTransitionThreshold) {

                    switch (activeExtents.nextTouchExtents) {
                        
                        case AxisExtents nextTouchExtents:
                            return SetAxisTransitionState(activeExtents, nextTouchExtents);

                        case TouchForkExtents nextForkExtents:
                            return SetForkTransitionState(activeExtents.axisMonitor.touchController, nextForkExtents);
                        
                    }
                }
            }

            return activeExtents;
        }

        private static AxisExtents SetDefaultState(AxisExtents activeExtents)
        {
            activeExtents.axisMonitor.SetTransitionStatus(false);
            Touch_Controller touchController = activeExtents.axisMonitor.touchController;
            GameObject axisMonitorObject = activeExtents.axisMonitor.gameObject;
            
            // Deactivate the opposing swipe axis
            if (activeExtents.swipeAxis.GetVariable() == touchController.ySwipeAxis.GetVariable()) {
                touchController.xSwipeAxis.SetStatus(axisMonitorObject, false);
                
            } else if (activeExtents.swipeAxis.GetVariable() == touchController.xSwipeAxis.GetVariable()) {
                touchController.ySwipeAxis.SetStatus(axisMonitorObject, false);
            }

            if (activeExtents.momentumAxis.GetVariable() == touchController.yMomentumAxis.GetVariable()) {
                touchController.xMomentumAxis.SetStatus(axisMonitorObject, false);
                
            } else if (activeExtents.momentumAxis.GetVariable() == touchController.xMomentumAxis.GetVariable()) {
                touchController.yMomentumAxis.SetStatus(axisMonitorObject, false);
            }

            return activeExtents;
        }

        private static AxisExtents SetAxisTransitionState(AxisExtents activeExtents, AxisExtents siblingAxisExtents)
        {
            activeExtents.axisMonitor.SetTransitionStatus(true);
            GameObject axisMonitorObject = activeExtents.axisMonitor.gameObject;
            
            siblingAxisExtents.swipeAxis.SetStatus(axisMonitorObject, true);
            siblingAxisExtents.momentumAxis.SetStatus(axisMonitorObject, true);
            siblingAxisExtents.swipeAxis.SetInverted(axisMonitorObject, siblingAxisExtents.inverted);
            siblingAxisExtents.momentumAxis.SetInverted(axisMonitorObject, siblingAxisExtents.inverted);

            return activeExtents;
        }

        private static TouchForkExtents SetForkTransitionState(Touch_Controller touchController, TouchForkExtents touchForkExtents)
        {
            touchController.axisMonitor.SetTransitionStatus(true);
            TouchBranchingPathData activeBranch = TouchForkUtils.GetActiveBranch(touchController, touchForkExtents);
            GameObject axisMonitorObject = touchForkExtents.axisMonitor.gameObject;
            
            if (activeBranch.branchKey == touchForkExtents.axisMonitor.yNorthKey ||
                activeBranch.branchKey == touchForkExtents.axisMonitor.ySouthKey) {
                touchController.ySwipeAxis.SetStatus(axisMonitorObject, true);
                touchController.yMomentumAxis.SetStatus(axisMonitorObject, true);
            }
            
            else if (activeBranch.branchKey == touchForkExtents.axisMonitor.xEastKey ||
                     activeBranch.branchKey == touchForkExtents.axisMonitor.xWestKey) {
                touchController.xSwipeAxis.SetStatus(axisMonitorObject, true);
                touchController.xMomentumAxis.SetStatus(axisMonitorObject, true);
            }

            return touchForkExtents;
        }
    }
}