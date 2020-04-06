using System;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{

    [Serializable]
    public class BranchingPath
    {
        [SerializeField]
        private BranchKey _branchKey;

        public BranchKey branchKey
        {
            get => _branchKey;
            set => _branchKey = value;
        }

        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }
        
        [SerializeField]
        private bool _invert;

        public bool invert
        {
            get => _invert;
            private set => _invert = value;
        }

        public BranchingPath(BranchKey branchKey, Sequence sequence)
        {
            this.branchKey = branchKey;
            this.sequence = sequence;
        }
        
        public BranchingPath(BranchKey branchKey, Sequence sequence, bool invert)
        {
            this.branchKey = branchKey;
            this.sequence = sequence;
            this.invert = invert;
        }

    }
}