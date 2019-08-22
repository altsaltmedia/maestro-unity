using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [System.Serializable]
    public class StartEndThreshold
    {
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
        [ReadOnly]
        public bool ForceForward = false;
        [ReadOnly]
        public bool ForceBackward = false;
        [ReadOnly]
        public bool VideoSequenceActive = false;

#if UNITY_ANDROID
        [ReadOnly]
        public bool MomentumDisabled = false;
#endif

        public bool hasAutoplay = false;
        [ShowIf("hasAutoplay")]
        [ReadOnly]
        public bool autoplayActive = false;
        [ShowIf("hasAutoplay")]
        public List<StartEndThreshold> autoplayThresholds = new List<StartEndThreshold>();

        public bool hasPauseMomentum = false;
        [ShowIf("hasPauseMomentum")]
        [ReadOnly]
        public bool pauseMomentumActive = false;
        [ShowIf("hasPauseMomentum")]
        public List<StartEndThreshold> pauseMomentumThresholds = new List<StartEndThreshold>();

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

    }
}