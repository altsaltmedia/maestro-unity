using UnityEngine;

namespace AltSalt.Sequencing.Touch
{

    public static class AxisModifier_SimpleSwitch
    {
        public static AxisModifier_AxisExtents ActivateSwitch(double masterTime, AxisModifier_AxisExtents activeExtents)
        {
            // Since we are currently within the extents, set the swipe to true
            activeExtents.swipeAxis.inverted = activeExtents.inverted;
            activeExtents.momentumAxis.inverted = activeExtents.inverted;
            activeExtents.swipeAxis.active = true;

            // If we are beyond the transition thresholds, that means this switch
            // is fully active; we can just activate the momentum, deactivate the
            // adjacent extents and avoid further calculation
            if (masterTime > activeExtents.startTransitionThreshold &&
                masterTime < activeExtents.endTransitionThreshold) {
                activeExtents.momentumAxis.active = true;

                if (activeExtents.previousAxisExtents != null) {
                    activeExtents.previousAxisExtents.swipeAxis.active = false;
                    activeExtents.previousAxisExtents.momentumAxis.active = false;
                }

                if (activeExtents.nextAxisExtents != null) {
                    activeExtents.nextAxisExtents.swipeAxis.active = false;
                    activeExtents.nextAxisExtents.momentumAxis.active = false;
                }
                    
                return activeExtents;
            }

            // Check transition thresholds to either activate or deactivate adjacent extents
            if (activeExtents.previousAxisExtents != null) {
                    
                if (masterTime <= activeExtents.startTransitionThreshold) {

                    activeExtents.previousAxisExtents.swipeAxis.active = true;
                    activeExtents.previousAxisExtents.swipeAxis.inverted =
                        activeExtents.previousAxisExtents.inverted;
                    activeExtents.previousAxisExtents.momentumAxis.inverted =
                        activeExtents.previousAxisExtents.inverted;

                    EventPayload eventPayload = EventPayload.CreateInstance();

                    if (activeExtents.axisModifier.touchController.isReversing == false) {
                        eventPayload.Set(AxisDestination.fromAxis, activeExtents.momentumAxis);
                        eventPayload.Set(AxisDestination.toAxis, activeExtents.previousAxisExtents.momentumAxis);
                        activeExtents.momentumAxis.active = false;
                        activeExtents.previousAxisExtents.momentumAxis.active = true;
                    }
                    else {
                        eventPayload.Set(AxisDestination.fromAxis, activeExtents.previousAxisExtents.momentumAxis);
                        eventPayload.Set(AxisDestination.toAxis, activeExtents.momentumAxis);
                        activeExtents.momentumAxis.active = true;
                        activeExtents.previousAxisExtents.momentumAxis.active = false;
                    }

                    activeExtents.axisModifier.convertMomentum.RaiseEvent(activeExtents.axisModifier.gameObject, eventPayload);
                }
            } 
                

            if (activeExtents.nextAxisExtents != null) {
                    
                if (masterTime >= activeExtents.endTransitionThreshold) {
                    activeExtents.nextAxisExtents.swipeAxis.active = true;
                    activeExtents.nextAxisExtents.swipeAxis.inverted =
                        activeExtents.nextAxisExtents.inverted;
                    activeExtents.nextAxisExtents.momentumAxis.inverted =
                        activeExtents.nextAxisExtents.inverted;
                        
                    EventPayload eventPayload = EventPayload.CreateInstance();
                        
                    if (activeExtents.axisModifier.touchController.isReversing == false) {
                        eventPayload.Set(AxisDestination.fromAxis, activeExtents.momentumAxis);
                        eventPayload.Set(AxisDestination.toAxis, activeExtents.nextAxisExtents.momentumAxis);
                        activeExtents.momentumAxis.active = false;
                        activeExtents.nextAxisExtents.momentumAxis.active = true;
                    } else {
                        eventPayload.Set(AxisDestination.fromAxis, activeExtents.nextAxisExtents.momentumAxis);
                        eventPayload.Set(AxisDestination.toAxis, activeExtents.momentumAxis.axisType);
                        activeExtents.momentumAxis.active = true;
                        activeExtents.nextAxisExtents.momentumAxis.active = false;
                    }
                        
                    activeExtents.axisModifier.convertMomentum.RaiseEvent(activeExtents.axisModifier.gameObject, eventPayload);
                        
                }
            }

            return activeExtents;
        }
    }
}