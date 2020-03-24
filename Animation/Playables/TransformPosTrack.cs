using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(ResponsiveVector3Clip))]
    [TrackBindingType(typeof(Transform))]
    public class TransformPosTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<TransformPosMixerBehaviour> trackPlayable = ScriptPlayable<TransformPosMixerBehaviour>.Create(graph, inputCount);
            TransformPosMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as RectTransform;
            if (comp == null)
                return;

            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalPosition.x");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalPosition.y");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalPosition.z");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}