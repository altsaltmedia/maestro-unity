using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace AltSalt.Maestro.Animation
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(VideoPlayer))]
    public class PlayableVideoPlayer : MonoBehaviour
    {
        VideoPlayer videoPlayer;
        public float initialTime;

        [SerializeField]
        SimpleEventTrigger videoCompleteCallback;

        MeshRenderer meshRenderer;

        void Start()
        {
            GetVideoPlayer();
            GetMeshRenderer();
            videoPlayer.prepareCompleted += SetInitialTime;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += TriggerUpdateMaterial;    
            videoPlayer.loopPointReached += VideoComplete;
            videoPlayer.Prepare();
        }

        void SetInitialTime(VideoPlayer param)
        {
            videoPlayer.StepForward();
            videoPlayer.time = initialTime;
        }

        public void SetCurrentTime(float targetTime)
        {
            videoPlayer.StepForward();
            videoPlayer.time = targetTime;
        }

        void GetVideoPlayer()
        {
            if (videoPlayer == null) {
                videoPlayer = GetComponent<VideoPlayer>();
            }
        }

        void GetMeshRenderer()
        {
            if (meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
        }

        void TriggerUpdateMaterial(VideoPlayer param, long frameId)
        {
            meshRenderer.sharedMaterial.mainTexture = videoPlayer.texture;
        }

        void VideoComplete(VideoPlayer src)
        {
            if(videoCompleteCallback.simpleEvent != null) {
                videoCompleteCallback.RaiseEvent(this.gameObject);
            }
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.8f, 0.1f)]
        [InfoBox("Prepares the video for playback.")]
        public void PrepareVideo()
        {
            GetVideoPlayer();
            videoPlayer.Prepare();
            videoPlayer.StepForward();
        }


        [Button(ButtonSizes.Large), GUIColor(0.6f, 0.3f, 0.3f)]
        [InfoBox("Stops the video.")]
        public void StopVideo()
        {
            GetVideoPlayer();
            videoPlayer.Stop();
        }

        [Button(ButtonSizes.Large), GUIColor(0.1f, 0.9f, 0.3f)]
        [InfoBox("Plays video")]
        public void PlayVideo()
        {
            GetVideoPlayer();
            videoPlayer.StepForward();
            videoPlayer.Play();
        }

        [Button(ButtonSizes.Large), GUIColor(0.5f, 0.5f, 0.9f)]
        [InfoBox("Pauses video")]
        public void PauseVideo()
        {
            GetVideoPlayer();
            videoPlayer.Pause();
        }

        [Button(ButtonSizes.Large), GUIColor(0.1f, 0.5f, 0.9f)]
        [InfoBox("Logs current frame")]
        public void LogFrame()
        {
            GetVideoPlayer();
            Debug.Log(videoPlayer.frame);
        }

#endif
        private static bool IsPopulated(SimpleEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}