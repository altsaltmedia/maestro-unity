using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace AltSalt.Maestro
{
    public class SimpleVideoPlayer : MonoBehaviour
    {
        public SimpleEventTrigger animatedCoverFinished;
        public SimpleEventTrigger VideoPlayed;
        public SimpleEventTrigger VideoPaused;

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
            animatedCoverFinished.RaiseEvent(this.gameObject);
        }

		public void TogglePlay()
		{
            if(videoPlayer.isPlaying) {
                videoPlayer.Pause();
                VideoPaused.RaiseEvent(this.gameObject);
            } else {
                videoPlayer.Play();
                VideoPlayed.RaiseEvent(this.gameObject);
            }
		}
    }
       
}