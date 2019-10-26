using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class ForkAxisExtents : AxisExtents
    {
        [SerializeField]
        private Dictionary<BranchType, BranchingPathSwitchData> _branchDictionary =
            new Dictionary<BranchType, BranchingPathSwitchData>();

        private Dictionary<BranchType, BranchingPathSwitchData> branchDictionary
        {
            get => _branchDictionary;
            set => _branchDictionary = value;
        }

        public ForkAxisExtents(AxisModifier axisModifier, TouchController.TouchData touchData, ForkAxis forkAxis)
        {
            this.axisModifier = axisModifier;
            this.axisModifierMarker = forkAxis;
            
            for (int i = 0; i < forkAxis.branchingPaths.Count; i++)
            {
                BranchingPath branchingPath = forkAxis.branchingPaths[i]; 
                BranchingPathSwitchData branchingPathSwitchData;
                    
                // If the branching path is equal to our current touch data, that means this sequence's
                // playable asset is where the fork originated, and we can flag this as the origin point
                // and set the touch data accordingly
                if (branchingPath.sequence == touchData.sequence)
                {
                    branchingPathSwitchData = new BranchingPathSwitchData(branchingPath.sequence, branchingPath.branchType, true, touchData);
                }
                // Otherwise, we must set the origin value to false and retrieve the touch data from the touch controller 
                else
                {
                    TouchController.TouchData siblingTouchData =
                        AxisModifier.ForkSwitch.GetTouchDataFromBranch(axisModifier.touchController, branchingPath);
                    branchingPathSwitchData = new BranchingPathSwitchData(branchingPath.sequence, branchingPath.branchType, false, siblingTouchData);
                        
                }

                this.branchDictionary.Add(branchingPathSwitchData.branchType, branchingPathSwitchData);
            }

            this.startTime = forkAxis.time;
        }
    }
}