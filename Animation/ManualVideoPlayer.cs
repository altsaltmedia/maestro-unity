using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [ExecuteInEditMode]
    public class ManualVideoPlayer : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [FormerlySerializedAs("_forwardVideoInstance")]
        private ManualVideoPlayerItem _videoInstance;

        protected ManualVideoPlayerItem videoInstance => _videoInstance;
        
        public double masterTime => videoInstance.localTime;
        
        private TimelineInstanceConfig _timelineInstanceConfig;

        public TimelineInstanceConfig timelineInstanceConfig
        {
            get => _timelineInstanceConfig;
            set => _timelineInstanceConfig = value;
        }

        protected virtual void Awake()
        {
            videoInstance.Init();
        }
        
        [Button(ButtonSizes.Large)]
        public virtual void CallPrepareVideo()
        {
            videoInstance.ForcePrepareVideo();
        }
        
        public virtual void CallSetTime(double targetTime)
        {
            videoInstance.SetTime(targetTime);
        }

        public virtual void SetToStart()
        {
            videoInstance.SetTime(0);
        }

        public virtual void LogTime()
        {
            Debug.Log("Current time (main): " + videoInstance.localTime.ToString("F4"));
        }
    }
}