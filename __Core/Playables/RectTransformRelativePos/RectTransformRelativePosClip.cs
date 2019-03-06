using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class RectTransformRelativePosClip : LerpToTargetClip
    {
        public new RectTransformRelativePosBehaviour template = new RectTransformRelativePosBehaviour ();

        //This allows you to use GameObjects in your Scene
        public ExposedReference<GameObject> originReferenceObject;
        public ExposedReference<GameObject> targetReferenceObject;

        bool isValid;

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.originReferenceObject = originReferenceObject.Resolve(graph.GetResolver());
            template.targetReferenceObject = targetReferenceObject.Resolve(graph.GetResolver());

            var playable = ScriptPlayable<RectTransformRelativePosBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}