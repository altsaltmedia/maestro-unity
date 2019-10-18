/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{

    public class SimpleSwitchLegacy : AxisSwitchLegacy
    {
        [Required]
        [PropertyOrder(1)]
        public Sequence parentClip;

        [PropertyOrder(1)]
        [ShowInInspector]
        [ReadOnly]
        [InfoBox("The switch is enabled at runtime when connected to an AxisSwitchTrigger playable")]
        bool switchEnabled;
        public bool SwitchEnabled {
            get {
                return switchEnabled;
            }

            set {
                switchEnabled = value;
            }
        }

        [PropertyOrder(1)]
        [ValidateInput("IsPopulated")]
        public BoolReference lockAxis;

        [Required]
        [PropertyOrder(1)]
        [ValidateInput("IsPopulated")]
        public BoolReference isReversing;

        // Swipe variables
        [Required]
        [FoldoutGroup("Swipe Variables", 3)]
        public Axis swipeOriginAxis;

        [Required]
        [FoldoutGroup("Swipe Variables", 3)]
        public Axis swipeDestAxis;

        [Required]
        [FoldoutGroup("Swipe Variables", 3)]
        public Axis transitionAxis;

        // An additional threshold, beyond the transition threshold, that we use to update the status of
        // the new active and inactive axes after a transition
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables", 3)]
        public FloatReference swipeOriginDestSpread = new FloatReference();


        // The threshold from the inflection point, going in both the positive and negative directions,
        // during which we'll accept input from both axes
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables", 3)]
        public FloatReference swipeTransitionSpread = new FloatReference();


        // Momentum variables
        [Required]
        [FoldoutGroup("Momentum Variables", 3)]
        public ComplexEventTrigger ConvertMomentum;

        [Required]
        [FoldoutGroup("Momentum Variables", 3)]
        public Axis momentumOriginAxis;

        [Required]
        [FoldoutGroup("Momentum Variables", 3)]
        public Axis momentumDestAxis;

        // The threshold from the inflection point, in both positive and negative direction, during
        // which we'll convert the momentum to the new axis, allowing us to maintain momentum around corners
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables", 3)]
        public FloatReference momentumTransitionSpread = new FloatReference();


        public virtual void UpdateActiveAxes()
        {
            if (lockAxis.Value == true || SwitchEnabled == false || parentClip.active == false) {
                return;
            }

            // Update swipe axes

            // Finalize transition to origin axis, and disable the transition and destination axis. Thresholds are
            // set up and executed in this way because, if you have multiples Axis Switches in a scene, you need to
            // make it so they only execute at certain times - hence why we use a combination of swipeTransitionSpread
            // (when both axes are active) and swipeOriginDestSpread (used to figure out when the transition should
            // be finalized.
            if (parentClip.currentTime >= (_inflectionPoint.Value - swipeTransitionSpread.Value - swipeOriginDestSpread.Value)
               && parentClip.currentTime < (_inflectionPoint.Value - swipeTransitionSpread.Value)) {

                //Debug.Log("origin update is firing");
                transitionAxis.active = false;
                swipeOriginAxis.active = true;
                swipeDestAxis.active = false;

                //momentumOriginAxis.Active = true;
                //momentumDestAxis.Active = false;

                // Activate both axes during the transition
            } else if (parentClip.currentTime >= (_inflectionPoint.Value - swipeTransitionSpread.Value)
                      && parentClip.currentTime < (_inflectionPoint.Value + swipeTransitionSpread.Value)) {

                //Debug.Log("transition update is firing");
                transitionAxis.active = true;
                swipeOriginAxis.active = true;
                swipeDestAxis.active = true;

                //momentumOriginAxis.Active = true;
                //momentumDestAxis.Active = true;

                // Finalize transition to destination axis
            } else if (parentClip.currentTime >= (_inflectionPoint.Value + swipeTransitionSpread.Value)
                       && parentClip.currentTime <= (_inflectionPoint.Value + swipeTransitionSpread.Value + swipeOriginDestSpread.Value)) {

                //Debug.Log("destination update is firing");
                transitionAxis.active = false;
                swipeOriginAxis.active = false;
                swipeDestAxis.active = true;

                //momentumOriginAxis.Active = false;
                //momentumDestAxis.Active = true;
            }

            // Update momentum axes
            if (parentClip.currentTime >= (_inflectionPoint.Value - momentumTransitionSpread.Value - swipeOriginDestSpread.Value)
                && parentClip.currentTime < (_inflectionPoint.Value - swipeOriginDestSpread.Value)) {

                momentumOriginAxis.active = true;
                momentumDestAxis.active = false;

            } else if (parentClip.currentTime >= (_inflectionPoint.Value - momentumTransitionSpread.Value) && parentClip.currentTime < _inflectionPoint.Value + momentumTransitionSpread.Value) {

                EventPayload eventPayload = EventPayload.CreateInstance();

                if (isReversing.Value == false) {
                    eventPayload.Set(AxisDestination.fromAxis, momentumOriginAxis.Name);
                    eventPayload.Set(AxisDestination.toAxis, momentumDestAxis.Name);
                    momentumOriginAxis.active = false;
                    momentumDestAxis.active = true;
                } else {
                    eventPayload.Set(AxisDestination.fromAxis, momentumDestAxis.Name);
                    eventPayload.Set(AxisDestination.toAxis, momentumOriginAxis.Name);
                    momentumOriginAxis.active = true;
                    momentumDestAxis.active = false;
                }

                ConvertMomentum.RaiseEvent(this.gameObject, eventPayload);

            } else if (parentClip.currentTime >= (_inflectionPoint.Value + momentumTransitionSpread.Value)
                       && parentClip.currentTime <= (_inflectionPoint.Value + momentumTransitionSpread.Value + swipeOriginDestSpread.Value)) {

                momentumOriginAxis.active = false;
                momentumDestAxis.active = true;
            }

            //else if (parentClip.currentTime > axisInflectionPoint.Value && parentClip.currentTime < (axisInflectionPoint.Value + momentumTransitionSpread.Value)) {

            //    EventPayload eventPayload = ScriptableObject.CreateInstance("EventPayload") as EventPayload;
            //    eventPayload.Set("fromAxis", momentumDestAxis.Name);
            //    eventPayload.Set("toAxis", momentumOriginAxis.Name);
            //    ConvertMomentum.Raise(eventPayload);
            //    Destroy(eventPayload);

            //    momentumOriginAxis.Active = false;
            //    momentumDestAxis.Active = true;
            //}

            //if(parentClip.currentTime >= (axisInflectionPoint.Value - momentumTransitionSpread.Value) && parentClip.currentTime < axisInflectionPoint.Value) {

            //    EventPayload eventPayload = ScriptableObject.CreateInstance("EventPayload") as EventPayload;
            //    eventPayload.Set("fromAxis", momentumOriginAxis.Name);
            //    eventPayload.Set("toAxis", momentumDestAxis.Name);
            //    ConvertMomentum.Raise(eventPayload);
            //    Destroy(eventPayload);

            //    momentumOriginAxis.Active = true;
            //    momentumDestAxis.Active = false;

            //} else if(parentClip.currentTime > axisInflectionPoint.Value && parentClip.currentTime < (axisInflectionPoint.Value + momentumTransitionSpread.Value)) {

            //    EventPayload eventPayload = ScriptableObject.CreateInstance("EventPayload") as EventPayload;
            //    eventPayload.Set("fromAxis", momentumDestAxis.Name);
            //    eventPayload.Set("toAxis", momentumOriginAxis.Name);
            //    ConvertMomentum.Raise(eventPayload);
            //    Destroy(eventPayload);

            //    momentumOriginAxis.Active = false;
            //    momentumDestAxis.Active = true;
            //}
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}