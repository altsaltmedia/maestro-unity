using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Sequencing
{
    [RequireComponent(typeof(Sequence_Config))]
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

        public void ModifySequence(float timeModifier)
        {
            sequence.currentTime += timeModifier;

            RootConfig rootConfig = sequence.sequenceConfig.masterSequence.rootConfig;
            
            if (sequence.currentTime < 0) {
                sequence.currentTime = 0;
                sequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                sequence.sequenceConfig.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                rootConfig.joiner.ActivatePreviousSequence(sequence);
                
            } else if (sequence.currentTime > sequence.sourcePlayable.duration) {
                sequence.currentTime = sequence.sourcePlayable.duration;
                sequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                sequence.sequenceConfig.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                rootConfig.joiner.ActivateNextSequence(sequence);
                
            } else  {
                sequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                sequence.sequenceConfig.masterSequence.RefreshElapsedTime(sequence);
                rootConfig.sequenceModified.RaiseEvent(this.gameObject);
            }
        }
    }
}