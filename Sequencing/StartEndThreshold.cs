using System;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace AltSalt
{
    [Serializable]
    public class StartEndThreshold
    {
        [SerializeField]
        [FormerlySerializedAs("description")]
        private string _description;
        
        public string description {
            get => _description;
            set => _description = value;
        }

        [SerializeField]
        [FormerlySerializedAs("startTime")]
        private double _startTime;
        
        public double startTime {
            get => _startTime;
            set => _startTime = value;
        }

        [SerializeField]
        [FormerlySerializedAs("endTime")]
        double _endTime;
        
        public double endTime {
            get => _endTime;
            set => _endTime = value;
        }

        public StartEndThreshold()
        {
            
        }

        public StartEndThreshold(double startTime, double endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        public StartEndThreshold(double startTime, double endTime, string description)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.description = description;
        }

        public static List<StartEndThreshold> CreateStartEndThresholds(List<double> startTimes, List<double> endTimes)
        {
            List<StartEndThreshold> startEndThresholds = new List<StartEndThreshold>();

            if(startTimes.Count != endTimes.Count) {
                if(startTimes.Count != endTimes.Count + 1) {
                    throw new System.Exception("Start time threshold and end time threshold counts do not match.");
                }
            }

            for(int i=0; i<startTimes.Count; i++) {
                if(i <= endTimes.Count - 1) {
                    startEndThresholds.Add(new StartEndThreshold(startTimes[i], endTimes[i]));
                } else {
                    startEndThresholds.Add(new StartEndThreshold(startTimes[i], 100000));
                }
            }

            return startEndThresholds;
        }

        public static bool TimeWithinThreshold(double sourceTime, List<StartEndThreshold> startEndThresholds)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < startEndThresholds.Count; q++) {
                if(sourceTime >= startEndThresholds[q].startTime &&
                   sourceTime <= startEndThresholds[q].endTime) {
                    withinThreshold = true;
                    break;
                }
            }

            return withinThreshold;
        }
        
        public static bool TimeWithinThreshold(double sourceTime, List<StartEndThreshold> startEndThresholds, out StartEndThreshold currentInterval)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < startEndThresholds.Count; q++) {
                if(sourceTime >= startEndThresholds[q].startTime &&
                   sourceTime <= startEndThresholds[q].endTime) {
                    currentInterval = startEndThresholds[q];
                    withinThreshold = true;
                    break;
                }
            }
            currentInterval = null;
            return withinThreshold;
        }
    }
}