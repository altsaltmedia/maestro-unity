using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public abstract class AxisModifier_TouchExtents : Input_Extents
    {
        [SerializeField]
        [ShowIf(nameof(ShowStartTransitionThreshold))]
        [PropertyOrder(2)]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        private double _startTransitionThreshold;

        public double startTransitionThreshold
        {
            get => _startTransitionThreshold;
            set => _startTransitionThreshold = value;
        }
            
        [SerializeField]
        [ShowIf(nameof(ShowEndTransitionThreshold))]
        [PropertyOrder(3)]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        private double _endTransitionThreshold;

        public double endTransitionThreshold
        {
            get => _endTransitionThreshold;
            set => _endTransitionThreshold = value;
        }
        
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private AxisModifier _axisModifier;

        public AxisModifier axisModifier
        {
            get => _axisModifier;
            protected set => _axisModifier = value;
        }

        protected abstract bool ShowStartTransitionThreshold();
        
        protected abstract bool ShowEndTransitionThreshold();
        
        public static bool TimeWithinExtents(double sourceTime, List<AxisModifier_TouchExtents> extents, out AxisModifier_TouchExtents currentExtents)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinExtents = false;

            for (int q = 0; q < extents.Count; q++) {
                if (extents[q].sequence.active == false) continue;
                        
                if(sourceTime >= extents[q].startTime &&
                   sourceTime <= extents[q].endTime) {
                    currentExtents = extents[q];
                    return true;
                }
            }
            currentExtents = null;
            return withinExtents;
        }

    }
}