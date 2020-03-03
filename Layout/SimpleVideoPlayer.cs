using UnityEngine;
using UnityEngine.Video;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(VideoPlayer))]
    public class SimpleVideoPlayer : MonoBehaviour
    {
        [SerializeField]
        private double _startTime;

        private double startTime => _startTime;

        [SerializeField]
        private GameObjectGenericAction _startedCallback = new GameObjectGenericAction();

        private GameObjectGenericAction startedCallback => _startedCallback;

        [SerializeField]
        private GameObjectGenericAction _pausedCallback = new GameObjectGenericAction();

        private GameObjectGenericAction pausedCallback => _pausedCallback;

        [SerializeField]
        private GameObjectGenericAction _loopPointReachedCallback = new GameObjectGenericAction();

        private GameObjectGenericAction loopPointReachedCallback => _loopPointReachedCallback;

        private VideoPlayer _videoPlayer;

        private VideoPlayer videoPlayer
        {
            get => _videoPlayer;
            set => _videoPlayer = value;
        }

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.prepareCompleted += PrepareCompletedCallback; 
            videoPlayer.started += FireStartedCallback;
            videoPlayer.loopPointReached += FireLoopPointReachedCallback;
            videoPlayer.Prepare();
        }

        private void PrepareCompletedCallback (VideoPlayer source)
        {
            videoPlayer.time = startTime;
        }
        
        private void FireStartedCallback (VideoPlayer source)
        {
            startedCallback.Invoke(this.gameObject);
        }
        
        private void FireLoopPointReachedCallback (VideoPlayer source)
        {
            loopPointReachedCallback.Invoke(this.gameObject);
        }

		public void TogglePlay()
		{
            if(videoPlayer.isPlaying) {
                videoPlayer.Pause();
                pausedCallback.Invoke(this.gameObject);
            } else {
                videoPlayer.Play();
            }
		}
    }
}