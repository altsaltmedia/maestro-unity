using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    [Serializable]
    public class TouchForkExtents : TouchExtents
    {
        [SerializeField]
        [HideReferenceObjectPicker]
        private Dictionary<BranchKey, TouchBranchingPathData> _branchDictionary =
            new Dictionary<BranchKey, TouchBranchingPathData>();

        public Dictionary<BranchKey, TouchBranchingPathData> branchDictionary
        {
            get => _branchDictionary;
            set => _branchDictionary = value;
        }

        [SerializeField]
        [HideReferenceObjectPicker]
        private TouchFork _touchFork;

        public TouchFork touchFork
        {
            get => _touchFork;
            set => _touchFork = value;
        }
            
        [SerializeField]
        private MarkerPlacement _markerPlacement;

        public MarkerPlacement markerPlacement
        {
            get => _markerPlacement;
            private set => _markerPlacement = value;
        }
        
        protected override string timeThresholdsTitle => "Fork Thresholds";

        public TouchForkExtents(AxisMonitor axisMonitor, Touch_Data touchData, ForkData forkData)
        {
            this.axisMonitor = axisMonitor;
            this.sequence = touchData.sequence;
            this.markerPlacement = forkData.markerPlacement;
            this.description = forkData.description;

            double forkTransitionSpread = axisMonitor.touchController.rootConfig.joiner.forkTransitionSpread;
            
            if (this.markerPlacement == MarkerPlacement.EndOfSequence) {
                double localStartTime = forkData.joinMarker.time - forkTransitionSpread;
                this.startTransitionThreshold =
                    MasterSequence.LocalToMasterTime(forkData.sequence.sequenceController.masterSequence, forkData.sequence,
                        localStartTime);
                this.startTime = this.startTransitionThreshold;
                this.endTransitionThreshold = MasterSequence.LocalToMasterTime(forkData.sequence.sequenceController.masterSequence, forkData.sequence,
                    forkData.joinMarker.time);
                this.endTime = this.endTransitionThreshold;
            }
            else {
                this.startTime = 0d;
                this.startTransitionThreshold =
                    this.startTime + forkTransitionSpread;
            }
            
            this.touchFork = forkData.fork as TouchFork;
            this.branchDictionary.Clear();
                
            for (int i = 0; i < touchFork.branchingPaths.Count; i++)
            {
                TouchBranchingPathData branchingPathData = new TouchBranchingPathData(touchFork.branchingPaths[i], touchData, axisMonitor);
                if (this.branchDictionary.ContainsKey(touchFork.branchingPaths[i].branchKey) == false) {
                    this.branchDictionary.Add(touchFork.branchingPaths[i].branchKey, branchingPathData);
                }
            }

        }

        public override TouchExtents Configure(TouchExtents previousTouchExtents, TouchExtents nextTouchExtents)
        {
            this.previousTouchExtents = previousTouchExtents;
            this.nextTouchExtents = nextTouchExtents;
            
            if (this.markerPlacement == MarkerPlacement.EndOfSequence) {
                return this;
            }

            if (nextTouchExtents == null) {
                this.endTransitionThreshold = double.MaxValue;
                this.endTime = double.MaxValue;
            } else if (nextTouchExtents is AxisExtents axisExtents) {
                this.endTransitionThreshold = axisExtents.markerMasterTime - axisMonitor.axisTransitionSpread;
                this.endTime = axisExtents.markerMasterTime;
            } else if (nextTouchExtents is TouchForkExtents forkExtents) {
                this.endTransitionThreshold = forkExtents.startTime;
                this.endTime = forkExtents.startTime;
            }

            return this;
        }
    }
}