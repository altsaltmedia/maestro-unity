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

        //        private ValueDropdownList<BranchType> branchTypeValues = new ValueDropdownList<BranchType>(){
        //            {"Y Negative", BranchType.yNeg },
        //            {"Y Positive", BranchType.yPos },
        //            {"X Negative", BranchType.xNeg },
        //            {"X Positive", BranchType.xPos }
        //        };

        public TouchBranchingPathData(BranchingPath branchingPath, Touch_Data touchData, AxisMonitor axisMonitor) : base(
            branchingPath.branchKey, branchingPath.sequence, branchingPath.invert)
        {
            this.touchData = axisMonitor.touchController.touchDataList.Find(matchedTouchData => matchedTouchData.sequence == branchingPath.sequence);
            if (this.touchData == null) {
                throw new DataMisalignedException("Sequence for branching path not found on Touch Controller. Please make sure all paths of a fork use the same root config.");
            }
            
//            if (branchKey == axisModifier.yNorthKey) {
//                branchType = BranchType.yNorth;
//            }
//            else if (branchKey == axisModifier.ySouthKey) {
//                branchType = BranchType.ySouth;
//            }
//            else if (branchKey == axisModifier.xEastKey) {
//                branchType = BranchType.xEast;
//            }
//            else if (branchKey == axisModifier.xWestKey) {
//                branchType = BranchType.xWest;
//            }
        }
        
    }
}