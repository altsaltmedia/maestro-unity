using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
{
    [Serializable]
    public class Input_Data {
            
        [SerializeField]
        [TitleGroup("$"+nameof(dataTitle))]
        [ReadOnly]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        protected virtual string dataTitle => "";
    }
}