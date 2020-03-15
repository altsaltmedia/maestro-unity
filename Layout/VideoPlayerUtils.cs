using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class VideoPlayerUtils : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;

        private VideoPlayer videoPlayer
        {
            get => _videoPlayer;
            set => _videoPlayer = value;
        }

        private void Awake()
        {
            GetVideoPlayer();
        }
        
        void GetVideoPlayer()
        {
            if (videoPlayer == null) {
                videoPlayer = GetComponent<VideoPlayer>();
            }
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.8f, 0.1f)]
        [InfoBox("Prepares the video for playback.")]
        public void PrepareVideo()
        {
            GetVideoPlayer();
            videoPlayer.Prepare();
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
    }
}