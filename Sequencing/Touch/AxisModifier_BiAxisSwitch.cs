using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class BiAxisSwitch
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
            
             public static BiAxisSwitch CheckActivateSwitch(BaseAxisSwitch axisSwitch, double masterTime)
            {
                BiAxisSwitch biAxisSwitch = axisSwitch as BiAxisSwitch;
                AxisModifier axisModifier = biAxisSwitch.touchController.axisModifier;

                if (masterTime >= biAxisSwitch.inflectionPoint - axisModifier.swipeTransitionSpread -  axisModifier.swipeResetSpread
                    && masterTime < biAxisSwitch.inflectionPoint - axisModifier.swipeTransitionSpread)
                {
                    biAxisSwitch.swipeOrigin.active = true;
                    biAxisSwitch.swipeDestination.active = false;
                }
                else if (masterTime >= biAxisSwitch.inflectionPoint -  axisModifier.swipeTransitionSpread
                         && masterTime < biAxisSwitch.inflectionPoint + axisModifier.swipeTransitionSpread)
                {
                    biAxisSwitch.swipeOrigin.active = true;
                    biAxisSwitch.swipeDestination.active = true;
                }
                else if (masterTime >= biAxisSwitch.inflectionPoint + axisModifier.swipeTransitionSpread
                           && masterTime <= biAxisSwitch.inflectionPoint + axisModifier.swipeTransitionSpread + axisModifier.swipeResetSpread)
                {
                    biAxisSwitch.swipeOrigin.active = false;
                    biAxisSwitch.swipeDestination.active = true;
                }
                
                if (masterTime >= biAxisSwitch.inflectionPoint - axisModifier.momentumTransitionSpread - axisModifier.swipeResetSpread
                    && masterTime < biAxisSwitch.inflectionPoint - axisModifier.swipeResetSpread) {

                    biAxisSwitch.momentumOrigin.active = true;
                    biAxisSwitch.momentumDestination.active = false;
                } else if (masterTime >= biAxisSwitch.inflectionPoint - axisModifier.momentumTransitionSpread
                           && masterTime < biAxisSwitch.inflectionPoint + axisModifier.momentumTransitionSpread) {
                    
                    EventPayload eventPayload = EventPayload.CreateInstance();

                    if (biAxisSwitch.touchController.isReversing == false) {
                        eventPayload.Set(AxisDestination.fromAxis, nameof(biAxisSwitch.momentumOrigin.axisType));
                        eventPayload.Set(AxisDestination.toAxis, nameof(biAxisSwitch.momentumDestination.axisType));
                        biAxisSwitch.momentumOrigin.active = false;
                        biAxisSwitch.momentumDestination.active = true;
                    } else {
                        eventPayload.Set(AxisDestination.fromAxis, nameof(biAxisSwitch.momentumDestination.axisType));
                        eventPayload.Set(AxisDestination.toAxis, nameof(biAxisSwitch.momentumOrigin.axisType));
                        biAxisSwitch.momentumOrigin.active = true;
                        biAxisSwitch.momentumDestination.active = false;
                    }

                    axisModifier.convertMomentum.RaiseEvent(axisModifier.gameObject, eventPayload);

                } else if (masterTime >= biAxisSwitch.inflectionPoint + axisModifier.momentumTransitionSpread
                           && masterTime <= biAxisSwitch.inflectionPoint + axisModifier.momentumTransitionSpread + axisModifier.swipeResetSpread) {
                    biAxisSwitch.momentumOrigin.active = false;
                    biAxisSwitch.momentumDestination.active = true;
                }

                return biAxisSwitch;
            }
        }
    }
}