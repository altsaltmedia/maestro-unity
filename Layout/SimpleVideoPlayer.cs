using UnityEngine;
using UnityEngine.Video;

namespace AltSalt.Maestro.Layout
{
    public class SimpleVideoPlayer : MonoBehaviour
    {
        [SerializeField]
        private SimpleEventTrigger _startedCallback = new SimpleEventTrigger();

        private SimpleEventTrigger startedCallback => _startedCallback;

        [SerializeField]
        private SimpleEventTrigger _pausedCallback = new SimpleEventTrigger();

        private SimpleEventTrigger pausedCallback => _pausedCallback;

        [SerializeField]
        private SimpleEventTrigger _loopPointReachedCallback = new SimpleEventTrigger();

        private SimpleEventTrigger loopPointReachedCallback => _loopPointReachedCallback;

        private VideoPlayer _videoPlayer;

        private VideoPlayer videoPlayer
        {
            get => _videoPlayer;
            set => _videoPlayer = value;
        }

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        private void Start ()
        {
            videoPlayer.Prepare();

            videoPlayer.started += FireStartedCallback;
            videoPlayer.loopPointReached += FireLoopPointReachedCallback;

            if(videoPlayer.playOnAwake) {
                videoPlayer.Play();
            }
        }

        private void FireStartedCallback (VideoPlayer source)
        {
            startedCallback.RaiseEvent(this.gameObject);
        }
        
        private void FireLoopPointReachedCallback (VideoPlayer source)
        {
            loopPointReachedCallback.RaiseEvent(this.gameObject);
        }

		public void TogglePlay()
		{
            if(videoPlayer.isPlaying) {
                videoPlayer.Pause();
                pausedCallback.RaiseEvent(this.gameObject);
            } else {
                videoPlayer.Play();
            }
		}
    }
       
}