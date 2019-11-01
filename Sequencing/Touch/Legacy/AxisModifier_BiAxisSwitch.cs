using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class BiAxisSwitchLegacy2
        {
            [SerializeField]
            private Axis _swipeOrigin;

            public Axis swipeOrigin
            {
                get => _swipeOrigin;
                set => _swipeOrigin = value;
            }

            [SerializeField]
            private Axis _swipeDestination;

            public Axis swipeDestination
            {
                get => _swipeDestination;
                set => _swipeDestination = value;
            }

            [SerializeField]
            private Axis _momentumOrigin;

            public Axis momentumOrigin
            {
                get => _momentumOrigin;
                set => _momentumOrigin = value;
            }

            [SerializeField]
            private Axis _momentumDestination;

            public Axis momentumDestination
            {
                get => _momentumDestination;
                set => _momentumDestination = value;
            }
            
             public static BiAxisSwitchLegacy2 CheckActivateSwitch(BaseAxisSwitchLegacy2 axisSwitch, double masterTime)
            {
                BiAxisSwitchLegacy2 biAxisSwitchLegacy2 = axisSwitch as BiAxisSwitchLegacy2;
                AxisModifier axisModifier = biAxisSwitchLegacy2.touchController.axisModifier;

                if (masterTime < biAxisSwitchLegacy2.inflectionPoint - axisModifier.swipeTransitionSpread)
                {
                    biAxisSwitchLegacy2.swipeOrigin.active = true;
                    biAxisSwitchLegacy2.swipeDestination.active = false;
                }
                else if (masterTime >= biAxisSwitchLegacy2.inflectionPoint -  axisModifier.swipeTransitionSpread
                         && masterTime < biAxisSwitchLegacy2.inflectionPoint + axisModifier.swipeTransitionSpread)
                {
                    biAxisSwitchLegacy2.swipeOrigin.active = true;
                    biAxisSwitchLegacy2.swipeDestination.active = true;
                }
                else if (masterTime >= biAxisSwitchLegacy2.inflectionPoint + axisModifier.swipeTransitionSpread)
                {
                    biAxisSwitchLegacy2.swipeOrigin.active = false;
                    biAxisSwitchLegacy2.swipeDestination.active = true;
                }
                
                if (masterTime < biAxisSwitchLegacy2.inflectionPoint - axisModifier.momentumTransitionSpread) {

                    biAxisSwitchLegacy2.momentumOrigin.active = true;
                    biAxisSwitchLegacy2.momentumDestination.active = false;
                    
                } else if (masterTime >= biAxisSwitchLegacy2.inflectionPoint - axisModifier.momentumTransitionSpread
                           && masterTime < biAxisSwitchLegacy2.inflectionPoint + axisModifier.momentumTransitionSpread) {
                    
                    EventPayload eventPayload = EventPayload.CreateInstance();

                    if (biAxisSwitchLegacy2.touchController.isReversing == false) {
                        eventPayload.Set(AxisDestination.fromAxis, nameof(biAxisSwitchLegacy2.momentumOrigin.axisType));
                        eventPayload.Set(AxisDestination.toAxis, nameof(biAxisSwitchLegacy2.momentumDestination.axisType));
                        biAxisSwitchLegacy2.momentumOrigin.active = false;
                        biAxisSwitchLegacy2.momentumDestination.active = true;
                    } else {
                        eventPayload.Set(AxisDestination.fromAxis, nameof(biAxisSwitchLegacy2.momentumDestination.axisType));
                        eventPayload.Set(AxisDestination.toAxis, nameof(biAxisSwitchLegacy2.momentumOrigin.axisType));
                        biAxisSwitchLegacy2.momentumOrigin.active = true;
                        biAxisSwitchLegacy2.momentumDestination.active = false;
                    }

                    axisModifier.convertMomentum.RaiseEvent(axisModifier.gameObject, eventPayload);

                } else if (masterTime >= biAxisSwitchLegacy2.inflectionPoint + axisModifier.momentumTransitionSpread
                           && masterTime <= biAxisSwitchLegacy2.inflectionPoint + axisModifier.momentumTransitionSpread + axisModifier.resetSpread) {
                    biAxisSwitchLegacy2.momentumOrigin.active = false;
                    biAxisSwitchLegacy2.momentumDestination.active = true;
                }

                return biAxisSwitchLegacy2;
            }
        }
    }
}