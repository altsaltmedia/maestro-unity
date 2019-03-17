using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace AltSalt
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class PlayableVideoPlayer : MonoBehaviour
    {
        VideoPlayer videoPlayer;
        public float initialTime;

        MeshRenderer meshRenderer;

        [Required]
        [SerializeField]
        SimpleEvent bufferSequenceCompleted;

        [ValidateInput("IsPopulated")]
        [SerializeField]
        IntReference bufferStepInterval;

        [ValidateInput("IsPopulated")]
        [SerializeField]
        BoolReference bufferPlayback;

        public bool renderVideo = true;

        void Start()
        {
            GetVideoPlayer();
            GetMeshRenderer();
            videoPlayer.prepareCompleted += SetInitialTime;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += TriggerUpdateMaterial;
            videoPlayer.Prepare();
        }


        void SetInitialTime(VideoPlayer param)
        {
            videoPlayer.StepForward();
            videoPlayer.time = initialTime;
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
            if (bufferPlayback.Value == true) {
                StartCoroutine(UpdateMaterial());
            } else {
                if(renderVideo == true) {
                    meshRenderer.sharedMaterial.mainTexture = videoPlayer.texture;
                } else {
                    meshRenderer.sharedMaterial.mainTexture = null;
                }
            } 
        }

        IEnumerator UpdateMaterial() {
            int bufferCount = 0;
            Texture texture = videoPlayer.texture;
            while(bufferCount < bufferStepInterval) {
                bufferCount++;
                yield return new WaitForEndOfFrame();
            }
            meshRenderer.sharedMaterial.mainTexture = texture;
        }

        public void BufferVideo()
        {
            GetVideoPlayer();
            videoPlayer.Prepare();
            videoPlayer.StepForward();
            videoPlayer.loopPointReached += BufferComplete;
            videoPlayer.Play();
        }

        void BufferComplete(VideoPlayer param)
        {
            videoPlayer.time = initialTime;
            bufferSequenceCompleted.Raise();
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
        private static bool IsPopulated(IntReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}