using AltSalt.Maestro.Animation;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    public class TimelineTriggerTrack : TrackAsset
    {
        protected void StoreClipProperties(GameObject directorObject)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as TimelineTriggerClip;
                if (myAsset) {
                    myAsset.startTime = clip.start;
                    myAsset.endTime = clip.end;
                    myAsset.parentTrack = this;
                    myAsset.timelineInstanceConfig = directorObject.GetComponent<TimelineInstanceConfig>();
                    myAsset.isReversingVariable = myAsset.timelineInstanceConfig.isReversingVariable;
                }
            }
        }
        
        protected LerpToTargetMixerBehaviour StoreMixerProperties(GameObject directorObject, LerpToTargetMixerBehaviour trackMixer)
        {
            var trackAssetConfig = directorObject.GetComponent<TimelineInstanceConfig>();
            trackMixer.timelineInstanceConfig = trackAssetConfig;
            trackMixer.appSettings = trackAssetConfig.appSettings;
            trackMixer.inputGroupKey = trackAssetConfig.inputGroupKey;
            trackMixer.parentTrack = this;
            return trackMixer;
        }
    }
}