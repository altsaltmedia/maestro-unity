using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    public class DirectorTools : MonoBehaviour
    {
        PlayableDirector playableDirector;

        [SerializeField]
        private UnityEvent _endEvent;

        private UnityEvent endEvent
        {
            get => _endEvent;
            set => _endEvent = value;
        }

        private void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
            playableDirector.stopped += CallEndEvent;
        }

        public void SetTime(float time)
        {
            playableDirector.time = time;
        }

        public void SetSpeed(float targetSpeed)
        {
            if(playableDirector.playableGraph.IsValid() == true) {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(targetSpeed);
            }
        }

        private void CallEndEvent(PlayableDirector playableDirector)
        {
            this.endEvent.Invoke();
        }

        public void Reset()
        {
            playableDirector.Stop();
            playableDirector.time = 0;
            playableDirector.Evaluate();
        }

        public void ChangeExrapolationMode (DirectorWrapMode wrapMode)
        {
            playableDirector.extrapolationMode = wrapMode;
        }
        
    }
    
}