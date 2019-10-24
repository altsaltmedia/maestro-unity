using UnityEngine;

namespace AltSalt.Sequencing
{
    public abstract class InputData
    {
        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }
        
    }
}