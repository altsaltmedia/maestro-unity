using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(ResponsiveVector3Clip))]
    [TrackBindingType(typeof(RectTransform))]
    public class RectTransformPosTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<RectTransformPosMixerBehaviour> trackPlayable = ScriptPlayable<RectTransformPosMixerBehaviour>.Create(graph, inputCount);
            RectTransformPosMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as RectTransform;
            if (comp == null)
                return;

            driver.AddFromName<RectTransform>(comp.gameObject, "m_AnchoredPosition.x");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_AnchoredPosition.y");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalPosition.x");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalPosition.y");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalPosition.z");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}