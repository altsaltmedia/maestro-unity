using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.6981132f, 0f, 0.1065063f)]
    [TrackClipType(typeof(ColorClip))]
    [TrackBindingType(typeof(TMP_Text))]
    public class TMProWordColorTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<TMProWordColorMixerBehaviour> trackPlayable = ScriptPlayable<TMProWordColorMixerBehaviour>.Create(graph, inputCount);
            TMProWordColorMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);

            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            TMP_Text trackBinding = director.GetGenericBinding(this) as TMP_Text;
            if (trackBinding == null)
                return;
            
            driver.AddFromName<TextMeshPro>(trackBinding.gameObject, "m_fontColor");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}