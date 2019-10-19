using UnityEngine;
using System.Collections.Generic;

namespace AltSalt.Sequencing.Autorun
{
    public class Interval : StartEndThreshold
    {
        [SerializeField]
        bool _isVideoSequence = false;
        
        public bool isVideoSequence {
            get => _isVideoSequence;
            set => _isVideoSequence = value;
        }

        public Interval(double startTime, double endTime) : base(startTime, endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
        }

        public Interval(double startTime, double endTime, string description) : base(startTime, endTime, description)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.isVideoSequence = isVideoSequence;
            this.description = description;
        }
        
        public static bool TimeWithinThreshold(double sourceTime, List<Interval> intervals)
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
        
        public static bool TimeWithinThreshold(double sourceTime, List<Interval> intervals, out Interval currentInterval)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < intervals.Count; q++) {
                if(sourceTime >= intervals[q].startTime &&
                   sourceTime <= intervals[q].endTime) {
                    currentInterval = intervals[q];
                    withinThreshold = true;
                    break;
                }
            }
            currentInterval = null;
            return withinThreshold;
        }
    }
}