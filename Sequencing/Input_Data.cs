using System;
using UnityEngine;

namespace AltSalt.Sequencing
{
    [Serializable]
    public class Input_Data {
            
        [SerializeField]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

    }
}