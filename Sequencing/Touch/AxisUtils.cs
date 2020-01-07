using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{

    public static class AxisUtils
    {
        public static TouchExtents ActivateAxisExtents(double masterTime, AxisExtents activeExtents)
        {
            // Since we are currently within the extents, set the swipe to true
            activeExtents.swipeAxis.inverted = activeExtents.inverted;
            activeExtents.momentumAxis.inverted = activeExtents.inverted;
            activeExtents.swipeAxis.active = true;
            activeExtents.momentumAxis.active = true;
            
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

        public static AxisExtents SetDefaultState(AxisExtents activeExtents)
        {
            activeExtents.axisMonitor.SetTransitionStatus(false);
            Touch_Controller touchController = activeExtents.axisMonitor.touchController;
            
            // Deactivate the opposing swipe axis
            if (activeExtents.swipeAxis == touchController.ySwipeAxis) {
                touchController.xSwipeAxis.active = false;
            } else if (activeExtents.swipeAxis == touchController.xSwipeAxis) {
                touchController.ySwipeAxis.active = false;
            }

            if (activeExtents.momentumAxis == touchController.yMomentumAxis) {
                touchController.xMomentumAxis.active = false;
                
            } else if (activeExtents.momentumAxis == touchController.xMomentumAxis) {
                touchController.yMomentumAxis.active = false;
            }

            return activeExtents;
        }

        public static AxisExtents SetAxisTransitionState(AxisExtents activeExtents, AxisExtents siblingAxisExtents)
        {
            activeExtents.axisMonitor.SetTransitionStatus(true);
            
            siblingAxisExtents.swipeAxis.active = true;
            siblingAxisExtents.momentumAxis.active = true;
            siblingAxisExtents.swipeAxis.inverted =
                siblingAxisExtents.inverted;
            siblingAxisExtents.momentumAxis.inverted =
                siblingAxisExtents.inverted;

            return activeExtents;
        }

        public static TouchForkExtents SetForkTransitionState(Touch_Controller touchController, TouchForkExtents touchForkExtents)
        {
            touchController.axisMonitor.SetTransitionStatus(true);
            TouchBranchingPathData activeBranch = TouchForkUtils.GetActiveBranch(touchController, touchForkExtents);
            
            if (activeBranch.branchKey == touchForkExtents.axisMonitor.yNorthKey ||
                activeBranch.branchKey == touchForkExtents.axisMonitor.ySouthKey) {
                touchController.ySwipeAxis.active = true;
                touchController.yMomentumAxis.active = true;
            }
            
            else if (activeBranch.branchKey == touchForkExtents.axisMonitor.xEastKey ||
                     activeBranch.branchKey == touchForkExtents.axisMonitor.xWestKey) {
                touchController.xSwipeAxis.active = true;
                touchController.xMomentumAxis.active = true;
            }

            return touchForkExtents;
        }
    }
}