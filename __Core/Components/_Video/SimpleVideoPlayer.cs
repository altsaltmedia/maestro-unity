using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace AltSalt
{
    public class SimpleVideoPlayer : MonoBehaviour
    {
        public SimpleEvent animatedCoverFinished;
        public SimpleEvent VideoPlayed;
        public SimpleEvent VideoPaused;

        VideoPlayer videoPlayer;

        void Start ()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.Prepare();
            if(videoPlayer.playOnAwake) {
                videoPlayer.Play();
            }
            videoPlayer.loopPointReached += FireCompleteEvent;
        }

        void FireCompleteEvent (VideoPlayer source)
        {
            animatedCoverFinished.Raise();
        }

		public void TogglePlay()
		{
            if(videoPlayer.isPlaying) {
                videoPlayer.Pause();
                VideoPaused.Raise();
            } else {
                videoPlayer.Play();
                VideoPlayed.Raise();
            }
		}
    }
       
}