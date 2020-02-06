using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventTimelineTriggerClip : TimelineTriggerClip
    {
        public ComplexEventTimelineTriggerBehaviour template = new ComplexEventTimelineTriggerBehaviour ();

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
#if UNITY_EDITOR
            string complexEventTriggersListPath = $"{nameof(template)}._{nameof(template.complexEventTriggerPackagers)}";
            
            for (int i = 0; i < template.complexEventTriggerPackagers.Count; i++) {
            
                // Make sure that the 'search attempted' bool is reset every time the graph starts
                template.complexEventTriggerPackagers[i].ResetSearchAttempted();
                template.complexEventTriggerPackagers[i].ResetReferencesSearchAttempted();
                
                string complexTriggerPath = complexEventTriggersListPath;
                complexTriggerPath += $".{i.ToString()}";
                
                /// NOTE: In some cases, repopulating will not take effect until the project has been reloaded.
                /// This happen when previously successfully populated reference loses its variable, and the
                /// variable is reimported / recreated with the same GUID. Workaround: just close and open the project.
                ///
                /// Hours spent trying to fix this: 3 hours
                template.complexEventTriggerPackagers[i].PopulateVariable(this, complexTriggerPath);
                template.complexEventTriggerPackagers[i].PopulateReferences(this, complexTriggerPath);
            }
#endif

            template.startTime = startTime;
            template.endTime = endTime;
            template.trackAssetConfig = trackAssetConfig;
            template._isReversingVariable = isReversingVariable;

            var playable = ScriptPlayable<ComplexEventTimelineTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}