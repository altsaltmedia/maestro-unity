using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Sequencing
{
    [RequireComponent(typeof(SequenceController))]
    [RequireComponent(typeof(Sequence_SyncTimeline))]
    [RequireComponent(typeof(PlayableDirector))]
    public class Sequence_ProcessModify : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        [InfoBox("This value must be set at runtime by a SequenceConfig component.")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }


    }
}