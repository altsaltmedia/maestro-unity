using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    [Serializable]
    public class AutorunExtents : Extents
    {
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _isEnd = false;

        public bool isEnd
        {
            get => _isEnd;
            set => _isEnd = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _isVideoSequence = false;
        
        public bool isVideoSequence {
            get => _isVideoSequence;
            set => _isVideoSequence = value;
        }

        public AutorunExtents(double startTime, double endTime) : base(startTime, endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
        }

        public AutorunExtents(double startTime, double endTime, string description) : base(startTime, endTime, description)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
            this.description = description;
        }
        
        public static bool TimeWithinThreshold(double sourceTime, List<AutorunExtents> intervals)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < intervals.Count; q++) {
                if(sourceTime >= intervals[q].startTime &&
                   sourceTime < intervals[q].endTime) {
                    withinThreshold = true;
                    break;
                }
            }

            return withinThreshold;
        }
        
        public static bool TimeWithinThreshold(double sourceTime, List<AutorunExtents> intervals, out AutorunExtents currentExtents)
        {
            for (int q = 0; q < intervals.Count; q++) {
                if(sourceTime >= intervals[q].startTime &&
                   sourceTime <= intervals[q].endTime) {
                    currentExtents = intervals[q];
                    return true;
                }
            }
            currentExtents = null;
            return false;
        }
    }
}