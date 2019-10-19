using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class BranchingPathSwitchData : BranchingPath
    {
        [SerializeField]
        [ReadOnly]
        private bool _isOrigin;

        public bool isOrigin
        {
            get => _isOrigin;
            set => _isOrigin = value;
        }
        
        [SerializeField]
        [HideInInspector]
        private TouchController.TouchData _touchData;

        public TouchController.TouchData touchData
        {
            get => _touchData;
            set => _touchData = value;
        }

        public BranchingPathSwitchData(Sequence sequence, BranchType branchType, bool isOrigin, TouchController.TouchData touchData)
        {
            this.sequence = sequence;
            this.branchType = branchType;
            this.isOrigin = isOrigin;
            this.touchData = touchData;
        }
    }
}