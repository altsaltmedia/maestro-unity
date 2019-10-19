using System.Collections.Generic;
using AltSalt.Sequencing.Touch;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class ForkSwitchBehaviour : LerpToTargetBehaviour
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

        [HideInInspector]
        private float _midPoint;

        public float midPoint
        {
            get => _midPoint;
            set => _midPoint = value;
        }
    }
}