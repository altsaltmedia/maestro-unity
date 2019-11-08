using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{

    public static class AxisUtils
    {
        public static AxisExtents ActivateAxisExtents(double masterTime, AxisExtents activeExtents)
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
            if (activeExtents.previousTouchExtents != null && activeExtents.previousTouchExtents is AxisExtents previousAxisExtents) {
                    
                if (masterTime <= activeExtents.startTransitionThreshold) {

                    return SetTransitionState(activeExtents, previousAxisExtents);
                }
            } 
                

            if (activeExtents.nextTouchExtents != null && activeExtents.nextTouchExtents is AxisExtents nextAxisExtents) {
                    
                if (masterTime >= activeExtents.endTransitionThreshold) {

                    return SetTransitionState(activeExtents, nextAxisExtents);
                }
            }

            return activeExtents;
        }

        public static AxisExtents SetDefaultState(AxisExtents activeExtents)
        {
            activeExtents.axisMonitor.SetTransitionStatus(false);

            Touch_Controller touchController = activeExtents.axisMonitor.touchController;
            
            if (activeExtents.swipeAxis == touchController.ySwipeAxis) {
                touchController.xSwipeAxis.active = false;
            } else if (activeExtents.swipeAxis == touchController.xSwipeAxis) {
                touchController.ySwipeAxis.active = false;
            }
            
            if (activeExtents.momentumAxis == touchController.yMomentumAxis) {
                touchController.xMomentumAxis.active = false;
            } else if (activeExtents.swipeAxis == touchController.xSwipeAxis) {
                touchController.yMomentumAxis.active = false;
            }

            return activeExtents;
        }

        public static AxisExtents SetTransitionState(AxisExtents activeExtents, AxisExtents siblingAxisExtents)
        {
            activeExtents.axisMonitor.SetTransitionStatus(true);
            
            siblingAxisExtents.swipeAxis.active = true;
            siblingAxisExtents.momentumAxis.active = true;
            siblingAxisExtents.swipeAxis.inverted =
                siblingAxisExtents.inverted;
            siblingAxisExtents.momentumAxis.inverted =
                siblingAxisExtents.inverted;

            EventPayload eventPayload = EventPayload.CreateInstance();

            if (activeExtents.axisMonitor.touchController.isReversing == false) {
                eventPayload.Set(AxisDestination.fromAxis, activeExtents.momentumAxis);
                eventPayload.Set(AxisDestination.toAxis, siblingAxisExtents.momentumAxis);
            }
            else {
                eventPayload.Set(AxisDestination.fromAxis, siblingAxisExtents.momentumAxis);
                eventPayload.Set(AxisDestination.toAxis, activeExtents.momentumAxis);
            }

            activeExtents.axisMonitor.convertMomentum.RaiseEvent(activeExtents.axisMonitor.gameObject, eventPayload);

            return activeExtents;
        }
    }
}