using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    public class Extents
    {
        [SerializeField]
        [FormerlySerializedAs("description")]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        [PropertyOrder(0)]
        private string _description;
            
        public string description {
            get => _description;
            set => _description = value;
        }

        [SerializeField]
        [FormerlySerializedAs("startTime")]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        [PropertyOrder(1)]
        private double _startTime;
            
        public double startTime {
            get => _startTime;
            set => _startTime = value;
        }

        [SerializeField]
        [FormerlySerializedAs("endTime")]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        [PropertyOrder(4)]
        double _endTime;
            
        public double endTime {
            get => _endTime;
            set => _endTime = value;
        }

        protected virtual string timeThresholdsTitle => "Time Thresholds";
        protected virtual string propertiesTitle => "Properties";

        public Extents()
        {
        }

        public Extents(double startTime, double endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public Extents(double startTime, double endTime, string description)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.description = description;
        }

        public static List<Extents> CreateExtentsList(List<double> startTimes, List<double> endTimes)
        {
            List<Extents> extents = new List<Extents>();

            if(startTimes.Count != endTimes.Count) {
                if(startTimes.Count != endTimes.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }

            for(int i=0; i<startTimes.Count; i++) {
                if(i <= endTimes.Count - 1) {
                    extents.Add(new Extents(startTimes[i], endTimes[i]));
                } else {
                    extents.Add(new Extents(startTimes[i], Double.MaxValue));
                }
            }

            return extents;
        }

        public static bool TimeWithinThresholdExclusive(double sourceTime, List<Extents> extents)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinExtents = false;

            for (int q = 0; q < extents.Count; q++) {
                if(sourceTime > extents[q].startTime &&
                   sourceTime < extents[q].endTime) {
                    withinExtents = true;
                    break;
                }
            }

            return withinExtents;
        }
            
        public static bool TimeWithinThresholdExclusive(double sourceTime, List<Extents> extents, out Extents currentExtents)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinExtents = false;

            for (int q = 0; q < extents.Count; q++) {
                if(sourceTime > extents[q].startTime &&
                   sourceTime < extents[q].endTime) {
                    currentExtents = extents[q];
                    withinExtents = true;
                    break;
                }
            }
            currentExtents = null;
            return withinExtents;
        }
        
        public static bool TimeBeyondThresholdExclusive(double sourceTime, Extents interval)
        {
            if (sourceTime > interval.endTime
                || sourceTime < interval.startTime) {
                return true;
            }

            return false;
        }

        public static bool TimeBeyondThresholdBothBoundsInclusive(double sourceTime, Extents interval)
        {
            // Special case for 0 - we don't want to count beginnings
            // of sequences as beyond the threshold bounds
            if (sourceTime == 0 && interval.startTime == 0) {
                return false;
            }
            
            if (Mathf.Approximately((float) sourceTime, (float) interval.endTime)
                || sourceTime > interval.endTime
                || Mathf.Approximately((float) sourceTime, (float) interval.startTime)
                || sourceTime < interval.startTime) {
                return true;
            }

            return false;
        }
    }
}