using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class AxisModifier_TouchBranchData : JoinTools_BranchingPath
    {
        //[ValueDropdown("branchTypeValues")]
        [SerializeField]
        [ReadOnly]
        private BranchType _branchType;

        public BranchType branchType
        {
            get => _branchType;
            set => _branchType = value;
        }

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

        public AxisModifier_TouchBranchData(JoinTools_BranchingPath branchingPath, Touch_Data touchData, AxisModifier axisModifier) : base(
            branchingPath.branchKey, branchingPath.sequence, branchingPath.invert)
        {
            this.touchData = touchData;

            if (branchKey == axisModifier.yNorthKey) {
                branchType = BranchType.yNorth;
            }
            else if (branchKey == axisModifier.ySouthKey) {
                branchType = BranchType.ySouth;
            }
            else if (branchKey == axisModifier.xEastKey) {
                branchType = BranchType.xEast;
            }
            else if (branchKey == axisModifier.xWestKey) {
                branchType = BranchType.xWest;
            }
        }
    }
}