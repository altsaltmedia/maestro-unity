using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    [Serializable]
    public abstract class TouchExtents : Extents
    {
        [SerializeField]
        [PropertyOrder(2)]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        private double _startTransitionThreshold;

        public double startTransitionThreshold
        {
            get => _startTransitionThreshold;
            set => _startTransitionThreshold = value;
        }
            
        [SerializeField]
        [PropertyOrder(3)]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        private double _endTransitionThreshold;

        public double endTransitionThreshold
        {
            get => _endTransitionThreshold;
            set => _endTransitionThreshold = value;
        }
        
        [SerializeField]
        [FoldoutGroup("$"+nameof(debugTitle))]
        [PropertyOrder(6)]
        private TouchExtents _previousTouchExtents;

        public TouchExtents previousTouchExtents
        {
            get => _previousTouchExtents;
            protected set => _previousTouchExtents = value;
        }

        [SerializeField]
        [FoldoutGroup("$"+nameof(debugTitle))]
        [PropertyOrder(7)]
        private TouchExtents _nextTouchExtents;

        public TouchExtents nextTouchExtents
        {
            get => _nextTouchExtents;
            protected set => _nextTouchExtents = value;
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
        private AxisMonitor _axisMonitor;

        public AxisMonitor axisMonitor
        {
            get => _axisMonitor;
            protected set => _axisMonitor = value;
        }

        protected virtual string debugTitle => "Debug";

        public static bool TimeWithinExtents(double sourceTime, List<TouchExtents> extents, out TouchExtents currentExtents)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinExtents = false;

            for (int q = 0; q < extents.Count; q++) {
                if(sourceTime >= extents[q].startTime &&
                   sourceTime <= extents[q].endTime) {
                    currentExtents = extents[q];
                    return true;
                }
            }
            currentExtents = null;
            return withinExtents;
        }

        public abstract TouchExtents Configure(TouchExtents previousTouchExtents, TouchExtents nextTouchExtents);

    }
}