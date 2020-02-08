using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.6981132f, 0f, 0.1065063f)]
    [TrackClipType(typeof(ColorClip))]
    [TrackBindingType(typeof(SpriteRenderer))]
    public class SpriteColorTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<SpriteColorMixerBehaviour> trackPlayable = ScriptPlayable<SpriteColorMixerBehaviour>.Create(graph, inputCount);
            SpriteColorMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            SpriteRenderer trackBinding = director.GetGenericBinding(this) as SpriteRenderer;
            if (trackBinding == null)
                return;

            driver.AddFromName<SpriteRenderer>(trackBinding.gameObject, "m_Color");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}