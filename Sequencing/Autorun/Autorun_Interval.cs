using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Autorun
{
    [Serializable]
    public class Autorun_Interval : Input_Extents
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

        public Autorun_Interval(double startTime, double endTime) : base(startTime, endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
        }

        public Autorun_Interval(double startTime, double endTime, string description) : base(startTime, endTime, description)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
            this.description = description;
        }
        
        public static bool TimeWithinThreshold(double sourceTime, List<Autorun_Interval> intervals)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < intervals.Count; q++) {
                if(sourceTime >= intervals[q].startTime &&
                   sourceTime <= intervals[q].endTime) {
                    withinThreshold = true;
                    break;
                }
            }

            return withinThreshold;
        }
        
        public static bool TimeWithinThreshold(double sourceTime, List<Autorun_Interval> intervals, out Autorun_Interval currentInterval)
        {
            for (int q = 0; q < intervals.Count; q++) {
                if(sourceTime >= intervals[q].startTime &&
                   sourceTime <= intervals[q].endTime) {
                    currentInterval = intervals[q];
                    return true;
                }
            }
            currentInterval = null;
            return false;
        }
    }
}