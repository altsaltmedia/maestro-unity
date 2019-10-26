using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace AltSalt.Sequencing.Touch
{
    public class ForkAxis : AxisModifierMarker
    {
        [SerializeField]
        [InfoBox("Be sure to define the current sequence, as well as adjacent sequences," +
                 "otherwise the fork won't work properly.'")]
        private List<BranchingPath> _branchingPaths;

        public List<BranchingPath> branchingPaths
        {
            get => _branchingPaths;
            set => _branchingPaths = value;
        }

    }
}