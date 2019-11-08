using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(RectTransformRelativePosClip))]
    [TrackBindingType(typeof(RectTransform))]
    public class RectTransformRelativePosTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            return ScriptPlayable<RectTransformRelativePosMixerBehaviour>.Create (graph, inputCount);
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

            //var so = new UnityEditor.SerializedObject(comp);
            //var iter = so.GetIterator();
            //while (iter.NextVisible(true)) {
            //    if (iter.hasVisibleChildren)
            //        continue;
            //    driver.AddFromName<RectTransform>(comp.gameObject, iter.propertyPath);
            //}
#endif
            base.GatherProperties(director, driver);
        }
    }   
}