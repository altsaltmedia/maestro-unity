using UnityEngine;

namespace AltSalt.Sequencing
{
    public class JoinTools_JoinSettings
    {
        [SerializeField]
        private Sequence _previousSequence;

        public Sequence previousSequence
        {
            get =>  _previousSequence;
            set =>  _previousSequence= value;
        }

        [SerializeField]
        private Sequence _nextSequence;

        public Sequence nextSequence
        {
            get => _nextSequence;
            set => _nextSequence = value;
        }
    }
}