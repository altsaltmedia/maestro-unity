using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(ResponsiveVector3Clip))]
    [TrackBindingType(typeof(RectTransform))]
    public class RectTransformRotationTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<RectTransformRotationMixerBehaviour> trackPlayable = ScriptPlayable<RectTransformRotationMixerBehaviour>.Create(graph, inputCount);
            RectTransformRotationMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as RectTransform;
            if (comp == null)
                return;

            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalRotation.w");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalRotation.x");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalRotation.y");
            driver.AddFromName<RectTransform>(comp.gameObject, "m_LocalRotation.z");
#endif
            base.GatherProperties(director, driver);
        }
    }   
}