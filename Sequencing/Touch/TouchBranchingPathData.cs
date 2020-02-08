using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Touch
{
    [Serializable]
    public class TouchBranchingPathData : BranchingPath
    {
        [SerializeField]
        private Touch_Data _touchData;

        public Touch_Data touchData
        {
            get => _touchData;
            private set => _touchData = value;
        }
        
        public TouchBranchingPathData(BranchingPath branchingPath, Touch_Data touchData, AxisMonitor axisMonitor) : base(
            branchingPath.branchKey, branchingPath.sequence, branchingPath.invert)
        {
            this.touchData = axisMonitor.touchController.touchDataList.Find(matchedTouchData => matchedTouchData.sequence == branchingPath.sequence);
            if (this.touchData == null) {
                throw new DataMisalignedException("Sequence for branching path not found on Touch Controller. Please make sure all paths of a fork use the same root config.");
            }
        }
    }
}