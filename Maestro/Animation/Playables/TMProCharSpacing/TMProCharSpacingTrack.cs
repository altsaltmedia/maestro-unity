using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.2981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(ResponsiveFloatClip))]
    [TrackBindingType(typeof(TMP_Text))]
    public class TMProCharSpacingTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            return ScriptPlayable<TMProCharSpacingMixerBehaviour>.Create (graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            TMP_Text trackBinding = director.GetGenericBinding(this) as TMP_Text;
            if (trackBinding == null)
                return;
            
            driver.AddFromName<TextMeshPro>(trackBinding.gameObject, "m_characterSpacing");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}