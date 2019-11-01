using System;
using UnityEngine;

namespace AltSalt.Sequencing
{

    [Serializable]
    public class JoinTools_BranchingPath
    {
        [SerializeField]
        private JoinTools_BranchKey _branchKey;

        public JoinTools_BranchKey branchKey
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

        public JoinTools_BranchingPath(JoinTools_BranchKey branchKey, Sequence sequence)
        {
            this.branchKey = branchKey;
            this.sequence = sequence;
        }

    }
}