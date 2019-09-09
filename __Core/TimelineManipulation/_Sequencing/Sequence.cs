using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [System.Serializable]
    public class StartEndThreshold
    {
        [SerializeField]
        string description;
        public double startTime;
        public double endTime;
        public bool isVideoSequence = false;
    }

    [CreateAssetMenu(menuName = "AltSalt/Sequence")]
    public class Sequence : ScriptableObject
    {
        public bool Active = true;

        [SerializeField]
        protected bool defaultStatus;

        public double currentTime = 0f;

        [SerializeField]
        protected double defaultTime;

        public bool Invert = false;

        [TitleGroup("Autopopulated Fields")]
        public bool ForceForward = false;

        [TitleGroup("Autopopulated Fields")]
        public bool ForceBackward = false;

        [TitleGroup("Autopopulated Fields")]
        public bool VideoSequenceActive = false;

#if UNITY_ANDROID
        [ReadOnly]
        [TitleGroup("Android Dependencies")]
        public bool MomentumDisabled = false;
#endif
        // Autoplay

        [TitleGroup("Autoplay")]
        public bool hasAutoplay = false;
        
        [ShowIf(nameof(hasAutoplay))]
        [ReadOnly]
        [TitleGroup("Autoplay")]
        public bool isLerping = false;

        [ShowIf(nameof(hasAutoplay))]
        [TitleGroup("Autoplay")]
        public float lerpInterval = .02f;

        [ShowIf(nameof(hasAutoplay))]
        [ReadOnly]
        [TitleGroup("Autoplay")]
        public bool autoplayActive = false;

        [ShowIf(nameof(hasAutoplay))]
        [TitleGroup("Autoplay")]
        public List<StartEndThreshold> autoplayThresholds = new List<StartEndThreshold>();


        // Pause Momentum

        [TitleGroup("Pause Momentum")]
        public bool hasPauseMomentum = false;

        [ShowIf(nameof(hasPauseMomentum))]
        [ReadOnly]
        [TitleGroup("Pause Momentum")]
        public bool pauseMomentumActive = false;

        [ShowIf(nameof(hasPauseMomentum))]
        [TitleGroup("Pause Momentum")]
        public List<StartEndThreshold> pauseMomentumThresholds = new List<StartEndThreshold>();

        [PropertySpace(10)]

        void Start()
        {
            currentTime = 0f;
        }

        public void SetStatus(bool targetStatus)
        {
            Active = targetStatus;
        }

        public void SetDefaults()
        {

            Active = defaultStatus;
            currentTime = defaultTime;
        }

        public void ModifySequenceTime(float timeModifier)
        {
            currentTime += timeModifier;
        }

#if UNITY_EDITOR
        [TitleGroup("Modify Time Thresholds Utils")]
        [SerializeField]
        public float modifyTargetStart;

        [TitleGroup("Modify Time Thresholds Utils")]
        [SerializeField]
        public float modifyTargetEnd;

        [TitleGroup("Modify Time Thresholds Utils")]
        [ShowIf(nameof(hasAutoplay))]
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
        [ShowIf(nameof(hasPauseMomentum))]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ModifyPauseMomentumThresholds(float modifier)
        {
            for (int i = 0; i < pauseMomentumThresholds.Count; i++) {
                if (pauseMomentumThresholds[i].startTime > modifyTargetStart && pauseMomentumThresholds[i].startTime < modifyTargetEnd) {
                    pauseMomentumThresholds[i].startTime += modifier;
                }

                if (pauseMomentumThresholds[i].endTime > modifyTargetStart && pauseMomentumThresholds[i].endTime < modifyTargetEnd) {
                    pauseMomentumThresholds[i].endTime += modifier;
                }
            }
        }
#endif

    }
}