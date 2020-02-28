using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Animation
{
    [ExecuteInEditMode]
    public class ForwardReverseVideoPlayer : ManualVideoPlayer
    {
        [SerializeField]
        private AppSettingsReference _appSettings;

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        [SerializeField]
        private InputGroupKeyReference _inputGroupKey;

        private InputGroupKey inputGroupKey => _inputGroupKey.GetVariable() as InputGroupKey;
        
        private bool isReversing => appSettings.GetIsReversing(this, inputGroupKey);

        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        private ManualVideoPlayerItem _reverseVideoInstance;

        private ManualVideoPlayerItem reverseVideoInstance => _reverseVideoInstance;

        private bool _playbackOverriden;

        public bool playbackOverriden
        {
            get => _playbackOverriden;
            set => _playbackOverriden = value;
        }

        private bool _pauseInProgress;

        private bool pauseInProgress
        {
            get => _pauseInProgress;
            set => _pauseInProgress = value;
        }

        private bool _executeAndroidHandling;

        private bool executeAndroidHandling
        {
            get => _executeAndroidHandling;
            set => _executeAndroidHandling = value;
        }

        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            _inputGroupKey.PopulateVariable(this, nameof(_inputGroupKey));
#endif
            
#if UNITY_ANDROID
            executeAndroidHandling = true;
#endif
            if (executeAndroidHandling == true) {
                reverseVideoInstance.Init();
                reverseVideoInstance.hide = true;
            }
            Debug.Log("EXECUTE ANDROID OVERRIDES " + executeAndroidHandling);
        }

        [Button(ButtonSizes.Large)]
        public override void CallPrepareVideo()
        {
            base.CallPrepareVideo();
            if (executeAndroidHandling == true) {
                reverseVideoInstance.ForcePrepareVideo();
            }
        }

        public override void CallSetTime(double targetTime)
        {
            if (executeAndroidHandling == false || timelineInstanceConfig.connectedToSequence == false) {
                videoInstance.SetTime(targetTime);
            }
            else {
                AndroidReverseVideoHandling(targetTime);
            }
        }

        public override void SetToStart()
        {
            videoInstance.SetTime(0);
            reverseVideoInstance.SetTime(reverseVideoInstance.videoLength);
        }
        
        public void SetToEnd()
        {
            videoInstance.SetTime(videoInstance.videoLength);
            reverseVideoInstance.SetTime(0);
        }

        private void AndroidReverseVideoHandling(double targetTime)
        {
            if(playbackOverriden == true && pauseInProgress == false) {
                pauseInProgress = true;
                StartCoroutine(PauseSequence());
            }
            
            videoInstance.SetTime(targetTime);
            reverseVideoInstance.SetTime(reverseVideoInstance.videoLength - targetTime);
        }

        private IEnumerator PauseSequence()
        {
            timelineInstanceConfig.GetPauseSequenceHandler(this.gameObject).Invoke(this);
#if UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
            Handheld.StartActivityIndicator();
#endif
            yield return new WaitForSeconds(2);

            if (isReversing == false) {
                timelineInstanceConfig.GetResumeSequenceHandler(this.gameObject).Invoke(this, true);
                videoInstance.hide = false;
                reverseVideoInstance.hide = true;
                videoInstance.RefreshVideoColor();
                reverseVideoInstance.RefreshVideoColor();
            }
            else {
                timelineInstanceConfig.GetResumeSequenceHandler(this.gameObject).Invoke(this, false);
                videoInstance.hide = true;
                reverseVideoInstance.hide = false;
                reverseVideoInstance.RefreshVideoColor();
                videoInstance.RefreshVideoColor();
            }
#if UNITY_ANDROID
//            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.DontShow);
            Handheld.StopActivityIndicator();
#endif
            pauseInProgress = false;
            playbackOverriden = false;
        }

        public override void LogTime()
        {
            Debug.Log("Current time (main): " + videoInstance.localTime.ToString("F4"));

            if (executeAndroidHandling == true) {
                Debug.Log("Current time (reverse): " + reverseVideoInstance.localTime.ToString("F4"));
            }
        }
        
        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}