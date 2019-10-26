using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public abstract class AxisExtents : StartEndThreshold
    {
        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }
        
        [SerializeField]
        private double _prevTransitionTime;

        public double prevTransitionTime
        {
            get => _prevTransitionTime;
            set => _prevTransitionTime = value;
        }
        
        [SerializeField]
        private double _nextTransitionTime;

        public double nextTransitionTime
        {
            get => _nextTransitionTime;
            set => _nextTransitionTime = value;
        }

        [SerializeField]
        private AxisExtents _previousAxisExtents;

        public AxisExtents previousAxisExtents
        {
            get => _previousAxisExtents;
            protected set => _previousAxisExtents = value;
        }

        [SerializeField]
        private AxisExtents _nextAxisExtents;

        public AxisExtents nextAxisExtents
        {
            get => _nextAxisExtents;
            protected set => _nextAxisExtents = value;
        }
        
        [SerializeField]
        private AxisModifier _axisModifier;

        public AxisModifier axisModifier
        {
            get => _axisModifier;
            protected set => _axisModifier = value;
        }

        [SerializeField]
        private AxisModifierMarker _axisModifierMarker;

        public AxisModifierMarker axisModifierMarker
        {
            get => _axisModifierMarker;
            protected set => _axisModifierMarker = value;
        }

        public static AxisExtents ConfigureAdjacentExtents(AxisExtents targetExtents, AxisExtents previousAxisExtents, AxisExtents nextAxisExtents)
        {
            if (previousAxisExtents != null) {
                targetExtents.previousAxisExtents = previousAxisExtents;

                if (previousAxisExtents is SingleAxisExtents singleAxisExtents) {
                    if (singleAxisExtents.inverted == true) {
                        targetExtents.prevTransitionTime =
                            targetExtents.startTime + targetExtents.axisModifier.invertTransitionSpread;
                    }
                    else {
                        targetExtents.prevTransitionTime =
                            targetExtents.startTime + targetExtents.axisModifier.swipeTransitionSpread;
                    }
                } else if (previousAxisExtents is ForkAxisExtents forkAxisExtents) {
                    targetExtents.prevTransitionTime = targetExtents.axisModifier.forkTransitionSpread;
                }
            }
            
            
            
            targetExtents.nextAxisExtents = nextAxisExtents;
            targetExtents.endTime = nextAxisExtents?.axisModifierMarker.time ?? 10000d;

            return targetExtents;
        }
        
    }
}