using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;


namespace AltSalt.Maestro.Animation
{

    [ExecuteInEditMode]
    public class PlayableVideoPlayerController : MonoBehaviour
    {
        [Serializable]
        [ExecuteInEditMode]
        public class VideoPlayerItem
        {
            [SerializeField]
            public GameObject videoObject;

            [ReadOnly]
            [SerializeField]
            public PlayableVideoPlayer vpScript;

            [ReadOnly]
            [SerializeField]
            public VideoPlayer vpComponent;

            [ReadOnly]
            [SerializeField]
            public MeshRenderer vpRenderer;

            [SerializeField]
            public bool isHidden;

            public void GetComponents()
            {
                if (vpScript == null) {
                    vpScript = videoObject.GetComponent<PlayableVideoPlayer>();
                }
                if (vpComponent == null) {
                    vpComponent = videoObject.GetComponent<VideoPlayer>();
                }
                if (vpRenderer == null) {
                    vpRenderer = videoObject.GetComponent<MeshRenderer>();
                }
            }
        }

        [SerializeField]
        [Required]
        public VideoPlayerItem mainVideoInstance;

        [SerializeField]
        [Required]
        [BoxGroup("Android Dependencies")]
        public VideoPlayerItem reverseVideoInstance;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [BoxGroup("Android Dependencies")]
        BoolReference isReversing;

        bool internalIsReversingValue = false;
        bool mainVideoSwapLoaded = false;
        bool reverseVideoSwapLoaded = false;

        void Start()
        {
            mainVideoInstance.GetComponents();
#if UNITY_ANDROID
            reverseVideoInstance.GetComponents();

            mainVideoInstance.vpComponent.frameReady += MainVideoFrameHandler;
            reverseVideoInstance.vpComponent.frameReady += ReverseVideoFrameHandler;
#endif
        }

#if UNITY_EDITOR
        void Update()
        {
            mainVideoInstance.GetComponents();
#if UNITY_ANDROID
            reverseVideoInstance.GetComponents();
#endif
        }
#endif

        public void PrepareVideos()
        {
            if (!mainVideoInstance.vpComponent.isPrepared) {
                mainVideoInstance.vpComponent.Prepare();
            }
            mainVideoInstance.vpComponent.StepForward();

#if UNITY_ANDROID
            if (!reverseVideoInstance.vpComponent.isPrepared) {
                reverseVideoInstance.vpComponent.Prepare();
            }

            reverseVideoInstance.vpComponent.StepForward();
#endif
        }
        public void SetTime(double targetTime)
        {
            mainVideoInstance.vpComponent.time = targetTime;
#if UNITY_ANDROID
            reverseVideoInstance.vpComponent.time = reverseVideoInstance.vpComponent.length - targetTime;

            if(isReversing.Value != internalIsReversingValue) {

                if (isReversing.Value == false) {
                
                    mainVideoSwapLoaded = true;
                
                } else {
                
                    reverseVideoSwapLoaded = true;
                }

                internalIsReversingValue = isReversing.Value;
            }
#endif
        }

        public void LogTime()
        {
            Debug.Log("Current time (main): " + mainVideoInstance.vpComponent.time.ToString("F4"));
#if UNITY_ANDROID
            Debug.Log("Current time (reverse): " + reverseVideoInstance.vpComponent.time.ToString("F4"));
#endif
        }

#if UNITY_ANDROID
        void MainVideoFrameHandler(VideoPlayer param, long frameId)
        {
            if(mainVideoSwapLoaded == true) {

                mainVideoInstance.isHidden = false;
                reverseVideoInstance.isHidden = true;
            }
        }

        void ReverseVideoFrameHandler(VideoPlayer param, long frameId)
        {
            if(reverseVideoSwapLoaded == true) {

                mainVideoInstance.isHidden = true;
                reverseVideoInstance.isHidden = false;
            }
        }
#endif

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}