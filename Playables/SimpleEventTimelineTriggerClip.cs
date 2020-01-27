using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTimelineTriggerClip : TimelineTriggerClip
    {
        public SimpleEventTimelineTriggerBehaviour template = new SimpleEventTimelineTriggerBehaviour ();

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {

#if UNITY_EDITOR
            string simpleEventTriggersListPath = $"{nameof(template)}._{nameof(template.simpleEventTriggers)}";
            
            for (int i = 0; i < template.simpleEventTriggers.Count; i++) {
                
                template.simpleEventTriggers[i].ResetSearchAttempted();
                
                string simpleEventPath = simpleEventTriggersListPath;
                simpleEventPath += $".{i.ToString()}";
                
                /// NOTE: In some cases, repopulating will not take effect until the project has been reloaded.
                /// This happen when previously successfully populated reference loses its variable, and the
                /// variable is reimported / recreated with the same GUID.  Workaround: just close and open the project.
                ///
                /// Hours spent trying to fix this: 3 hours
                template.simpleEventTriggers[i].PopulateVariable(this, simpleEventPath.Split('.'));
            }
#endif

            template.startTime = startTime;
            template.endTime = endTime;
            template.trackAssetConfig = trackAssetConfig;

            var playable = ScriptPlayable<SimpleEventTimelineTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}