using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

namespace AltSalt
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

        public void SetTime(Sequence sequence)
        {
            playableDirector.time = sequence.currentTime;
        }

        public void ChangeExrapolationMode (DirectorWrapMode wrapMode)
        {
            playableDirector.extrapolationMode = wrapMode;
        }
        
    }
    
}