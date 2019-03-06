using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;
using UnityEngine.Playables;

namespace AltSalt
{
    public class SimpleTimelinePlayer : MonoBehaviour
    {
        public SimpleEvent animatedCoverFinished;

        PlayableDirector playableDirector;

        void Start()
        {
            playableDirector = GetComponent<PlayableDirector>();
            playableDirector.stopped += FireCompleteEvent;
            playableDirector.Play();
        }

        void FireCompleteEvent(PlayableDirector source)
        {
            animatedCoverFinished.Raise();
        }
    }

}