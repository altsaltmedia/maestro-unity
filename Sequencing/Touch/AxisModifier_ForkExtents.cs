using System;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class AxisModifier_ForkExtents : AxisModifier_TouchExtents
    {
        [SerializeField]
        private Dictionary<BranchType, AxisModifier_TouchBranchData> _branchDictionary =
            new Dictionary<BranchType, AxisModifier_TouchBranchData>();

        public Dictionary<BranchType, AxisModifier_TouchBranchData> branchDictionary
        {
            get => _branchDictionary;
            set => _branchDictionary = value;
        }

        [SerializeField]
        private AxisModifier_TouchFork _touchFork;

        public AxisModifier_TouchFork touchFork
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
        
        protected override bool ShowStartTransitionThreshold()
        {
            return true;
        }
        
        protected override bool ShowEndTransitionThreshold()
        {
            return true;
        }

        public AxisModifier_ForkExtents(AxisModifier axisModifier, Touch_Data touchData, JoinTools_ForkJoinData forkJoinData)
        {
            this.axisModifier = axisModifier;
            this.sequence = touchData.sequence;
            this.markerPlacement = forkJoinData.markerPlacement;
            this.description = forkJoinData.description;
                
            if (this.markerPlacement == MarkerPlacement.EndOfSequence) {
                this.startTransitionThreshold = MasterSequence.LocalToMasterTime(forkJoinData.sequence.sequenceConfig.masterSequence, forkJoinData.sequence, forkJoinData.extents.startTime);
                this.startTime = this.startTransitionThreshold - axisModifier.resetSpread;
                this.endTime = MasterSequence.LocalToMasterTime(forkJoinData.sequence.sequenceConfig.masterSequence, forkJoinData.sequence, forkJoinData.extents.endTime);
                this.endTransitionThreshold = this.endTime;
            } else {
                this.startTime = MasterSequence.LocalToMasterTime(forkJoinData.sequence.sequenceConfig.masterSequence, forkJoinData.sequence, forkJoinData.extents.startTime);
                this.startTransitionThreshold = MasterSequence.LocalToMasterTime(forkJoinData.sequence.sequenceConfig.masterSequence, forkJoinData.sequence, forkJoinData.extents.endTime);
                this.endTime = this.startTransitionThreshold + axisModifier.resetSpread;;
                this.endTransitionThreshold = this.endTime;
            }
            
            this.touchFork = forkJoinData.fork;
                
            for (int i = 0; i < touchFork.branchingPaths.Count; i++)
            {
                AxisModifier_TouchBranchData touchBranchData = new AxisModifier_TouchBranchData(touchFork.branchingPaths[i], touchData, axisModifier);
                this.branchDictionary.Add(touchBranchData.branchType, touchBranchData);
            }

        }
    }
}