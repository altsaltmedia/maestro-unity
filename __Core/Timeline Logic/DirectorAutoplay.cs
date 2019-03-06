using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

namespace AltSalt
{
    public class DirectorAutoplay : MonoBehaviour
    {
        PlayableDirector playableDirector;
        SimpleEvent VideoPlayed;

        void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        public void HaltAutoplay ()
        {
            playableDirector.extrapolationMode = DirectorWrapMode.None;
        }
        
    }
    
}