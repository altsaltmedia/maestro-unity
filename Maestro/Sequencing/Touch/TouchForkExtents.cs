using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    [Serializable]
    public class TouchForkExtents : TouchExtents
    {
        [SerializeField]
        private Dictionary<BranchKey, TouchBranchingPathData> _branchDictionary =
            new Dictionary<BranchKey, TouchBranchingPathData>();

        public Dictionary<BranchKey, TouchBranchingPathData> branchDictionary
        {
            get => _branchDictionary;
            set => _branchDictionary = value;
        }

        [SerializeField]
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
                double localStartTime = forkData.forkMarker.time - forkTransitionSpread;
                this.startTransitionThreshold =
                    MasterSequence.LocalToMasterTime(forkData.sequence.sequenceConfig.masterSequence, forkData.sequence,
                        localStartTime);
                this.startTime = this.startTransitionThreshold - axisMonitor.axisTransitionSpread;
                this.endTransitionThreshold = MasterSequence.LocalToMasterTime(forkData.sequence.sequenceConfig.masterSequence, forkData.sequence,
                    forkData.forkMarker.time);
                this.endTime = this.endTransitionThreshold;
            }
            else {
                this.startTime = 0d;
                this.startTransitionThreshold =
                    this.startTime + forkTransitionSpread;
            }
            
            this.touchFork = forkData.fork;
                
            for (int i = 0; i < touchFork.branchingPaths.Count; i++)
            {
                TouchBranchingPathData branchingPathData = new TouchBranchingPathData(touchFork.branchingPaths[i], touchData, axisMonitor);
                this.branchDictionary.Add(touchFork.branchingPaths[i].branchKey, branchingPathData);
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
        

//        public TouchFork_Extents Configure(TouchExtents previousAxisExtents, TouchExtents nextAxisExtents)
//        {
//            if (this.markerPlacement == MarkerPlacement.EndOfSequence) {
//                this.startTransitionThreshold = MasterSequence.LocalToMasterTime(forkData.sequence.sequenceConfig.masterSequence, forkData.sequence, forkData.extents.startTime);
//                this.startTime = this.startTransitionThreshold - axisMonitor.resetSpread;
//                this.endTime = MasterSequence.LocalToMasterTime(forkData.sequence.sequenceConfig.masterSequence, forkData.sequence, forkData.extents.endTime);
//                this.endTransitionThreshold = this.endTime;
//            } else {
//                this.startTime = MasterSequence.LocalToMasterTime(forkData.sequence.sequenceConfig.masterSequence, forkData.sequence, forkData.extents.startTime);
//                this.startTransitionThreshold = MasterSequence.LocalToMasterTime(forkData.sequence.sequenceConfig.masterSequence, forkData.sequence, forkData.extents.endTime);
//                this.endTime = this.startTransitionThreshold + axisMonitor.resetSpread;;
//                this.endTransitionThreshold = this.endTime;
//            }
//        }
    }
}