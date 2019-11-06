using UnityEngine;

namespace AltSalt.Sequencing
{
    public class JoinTools_JoinSettings
    {
        [SerializeField]
        private ScriptableObject _previousDestination;

        public ScriptableObject previousDestination
        {
            get =>  _previousDestination;
            set =>  _previousDestination= value;
        }

        [SerializeField]
        private ScriptableObject _nextDestination;

        public ScriptableObject nextDestination
        {
            get => _nextDestination;
            set => _nextDestination = value;
        }
    }
}