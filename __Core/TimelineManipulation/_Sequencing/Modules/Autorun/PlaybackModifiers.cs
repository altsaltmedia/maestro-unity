using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt
{
    [System.Serializable]
    public class PlaybackModifiers
    {
       
        [TitleGroup("Pause Momentum")]
        [SerializeField]
        [FormerlySerializedAs("pauseMomentumThresholds")]
        private List<StartEndThreshold> _pauseMomentumThresholds = new List<StartEndThreshold>();
        
        public List<StartEndThreshold> pauseMomentumThresholds {
            get => _pauseMomentumThresholds;
            set => _pauseMomentumThresholds = value;
        }

        
        public static bool TimeWithinVideoThreshold(double sourceTime, List<StartEndThreshold> startEndThresholds)
        {
            // Check if we're inside a pauseMomentumThreshold
            bool withinThreshold = false;

            for (int q = 0; q < startEndThresholds.Count; q++) {
                if(sourceTime >= startEndThresholds[q].startTime &&
                   sourceTime <= startEndThresholds[q].endTime &&
                   startEndThresholds[q].isVideoSequence) {
                    withinThreshold = true;
                    break;
                }
            }

            return withinThreshold;
        }
        
#if UNITY_EDITOR
        [TitleGroup("Modify Time Thresholds Utils")]
        [SerializeField]
        private float _modifyTargetStart;
        private float modifyTargetStart
        {
            get => _modifyTargetStart;
            set => _modifyTargetStart = value;
        }


        [TitleGroup("Modify Time Thresholds Utils")]
        [SerializeField]
        private float _modifyTargetEnd;
        private float modifyTargetEnd
        {
            get => _modifyTargetEnd;
            set => _modifyTargetEnd = value;
        }
        
        [TitleGroup("Modify Time Thresholds Utils")]
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ModifyAutoplayThresholds(float modifier)
        {
            for(int i=0; i<autoplayThresholds.Count; i++)
            {
                if(autoplayThresholds[i].startTime > modifyTargetStart && autoplayThresholds[i].startTime < modifyTargetEnd) {
                    autoplayThresholds[i].startTime += modifier;
                }

                if (autoplayThresholds[i].endTime > modifyTargetStart && autoplayThresholds[i].endTime < modifyTargetEnd) {
                    autoplayThresholds[i].endTime += modifier;
                }
            }
        }

        [TitleGroup("Modify Time Thresholds Utils")]
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ModifyPauseMomentumThresholds(float modifier)
        {
            for (int i = 0; i < pauseMomentumThresholds.Count; i++) {
                if (pauseMomentumThresholds[i].startTime > _modifyTargetStart && pauseMomentumThresholds[i].startTime < _modifyTargetEnd) {
                    pauseMomentumThresholds[i].startTime += modifier;
                }

                if (pauseMomentumThresholds[i].endTime > _modifyTargetStart && pauseMomentumThresholds[i].endTime < _modifyTargetEnd) {
                    pauseMomentumThresholds[i].endTime += modifier;
                }
            }
        }
#endif

    }
}

