using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.UI;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.6981132f, 0f, 0.1065063f)]
    [TrackClipType(typeof(ColorClip))]
    [TrackBindingType(typeof(Image))]
    public class ImageUIColorTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<ImageUIColorMixerBehaviour> trackPlayable = ScriptPlayable<ImageUIColorMixerBehaviour>.Create(graph, inputCount);
            ImageUIColorMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            Image trackBinding = director.GetGenericBinding(this) as Image;
            if (trackBinding == null)
                return;

            driver.AddFromName<SpriteRenderer>(trackBinding.gameObject, "m_Color");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}