using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    public class DirectorTools : MonoBehaviour
    {
        PlayableDirector playableDirector;

        void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
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