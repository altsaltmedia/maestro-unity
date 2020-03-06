using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Audio
{
    public class PopulateAudioClips : PopulateClips
    {
        public override PlayableAsset PopulateClip(PlayableDirector targetDirector, TrackAsset parentTrack, EasingFunction.Ease easeType, TimelineClip timelineClip)
        {
            UnityEngine.Object sourceObject = null;
            foreach (PlayableBinding playableBinding in parentTrack.outputs) {
                sourceObject = targetDirector.GetGenericBinding(playableBinding.sourceObject);
            }

            switch (parentTrack) {

                case AudioLerpSnapshotTrack audioLerpSnapshotTrack : {
                    
                    AudioLerpSnapshotClip currentClipAsset = timelineClip.asset as AudioLerpSnapshotClip;
                    IEnumerable<TimelineClip> trackClips = audioLerpSnapshotTrack.GetClips();
                    
                    // If there are other clips on this track besides the one we just created,
                    // populate the newly created clip based on the one right before it
                    if (trackClips.Count() > 1) {
                        AudioLerpSnapshotClip previousClipAsset = trackClips.ElementAt(trackClips.Count() - 2).asset as AudioLerpSnapshotClip;
                        currentClipAsset.template.crossfade = previousClipAsset.template.crossfade;
                        currentClipAsset.template.snapshotA = previousClipAsset.template.snapshotA;
                        currentClipAsset.template.snapshotB = previousClipAsset.template.snapshotB;
                        currentClipAsset.template.initialBlend = previousClipAsset.template.targetBlend;
                        currentClipAsset.template.targetBlend = previousClipAsset.template.targetBlend;
                    }
                    else {
                        if (audioLerpSnapshotTrack.defaultSnapshotLerpType == DefaultSnapshotLerpType.Crossfade) {
                            AudioMixer audioMixer = sourceObject as AudioMixer;
                            if (audioMixer != null) {
                                currentClipAsset.template.crossfade = true;
                                currentClipAsset.template.snapshotA = audioMixer.FindSnapshot("Off");
                                currentClipAsset.template.snapshotB = audioMixer.FindSnapshot("On");
                            }
                        }
                    }
                    
                    currentClipAsset.template.ease = easeType;
                    currentClipAsset.template.RefreshLabels();
                    return currentClipAsset;
                }

                default:
                {
                    //Debug.Log("Track type not recognized");
                    return null;
                }
            }
        }
    }
}