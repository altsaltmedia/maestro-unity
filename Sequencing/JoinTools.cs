using System;
using System.Collections.Generic;
using System.Linq;
using AltSalt.Sequencing.Touch;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing
{
    public class JoinTools : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        [Required]
        [InfoBox("This must be populated via a root config component")]
        private RootConfig _rootConfig;

        public RootConfig rootConfig
        {
            get => _rootConfig;
            set => _rootConfig = value;
        }
        
        public List<MasterSequence> masterSequences
        {
            get => rootConfig.masterSequences;
        } 
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventTrigger _onForkUpdate;

        public SimpleEventTrigger onForkUpdate
        {
            get => _onForkUpdate;
            set => _onForkUpdate = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _forkActive;

        public bool forkActive
        {
            get => _forkActive.Value;
            private set => _forkActive.Variable.SetValue(value);
        }

        [SerializeField]
        [ReadOnly]
        private JoinSettingsCollection _joinSettingsCollection;

        private JoinSettingsCollection joinSettingsCollection
        {
            get => _joinSettingsCollection;
            set => _joinSettingsCollection = value;
        }

        [SerializeField]
        [ReadOnly]
        private List<JoinTools_ForkJoinData> _forkDataList;

        public List<JoinTools_ForkJoinData> forkDataList
        {
            get => _forkDataList;
            private set => _forkDataList = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private FloatReference _forkTransitionSpread;

        public float forkTransitionSpread
        {
            get => _forkTransitionSpread.Value;
        }

        public void SetForkStatus(bool targetStatus)
        {
            forkActive = targetStatus;
            onForkUpdate.RaiseEvent(this.gameObject);
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private SimpleEventTrigger _boundaryReached;

        private SimpleEventTrigger boundaryReached
        {
            get => _boundaryReached;
            set => _boundaryReached = value;
        }

        public Sequence ActivatePreviousSequence(Sequence sourceSequence)
        {
            JoinTools_JoinSettings sequenceSettings = joinSettingsCollection[sourceSequence];
            Sequence previousSequence = sequenceSettings.previousSequence;
            
            if (previousSequence != null)
            {
                sourceSequence.active = false;

                JoinTools_ForkJoinData currentForkData = forkDataList.Find(x => x.sequence == sourceSequence);
                JoinTools_ForkJoinData previousForkData = forkDataList.Find(x => x.sequence == previousSequence);

                if (currentForkData != null && previousForkData != null) {
                    previousSequence.currentTime = previousForkData.markerPlacement == MarkerPlacement.StartOfSequence ? 0d : previousSequence.sourcePlayable.duration;
                } else {
                    previousSequence.currentTime = previousSequence.sourcePlayable.duration;
                }
                
                previousSequence.active = true;
                previousSequence.sequenceConfig.gameObject.SetActive(true);
                previousSequence.sequenceConfig.gameObject.SetActive(true);
                previousSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                rootConfig.sequenceModified.RaiseEvent(this.gameObject, previousSequence);
                    
                sourceSequence.sequenceConfig.gameObject.SetActive(false);
            }
            else
            {
                boundaryReached.RaiseEvent(this.gameObject);
            }
        }
        
        public Sequence ActivateNextSequence(Sequence sourceSequence)
        {
            JoinTools_JoinSettings sequenceSettings = joinSettingsCollection[sourceSequence];
            Sequence nextSequence = sequenceSettings.previousSequence;
            
            if (nextSequence != null)
            {
                sourceSequence.active = false;
                    
                nextSequence.currentTime = 0;
                nextSequence.active = true;
                nextSequence.sequenceConfig.gameObject.SetActive(true);
                nextSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                rootConfig.sequenceModified.RaiseEvent(this.gameObject, nextSequence);

                sourceSequence.sequenceConfig.gameObject.SetActive(false);
            }
            else
            {
                boundaryReached.RaiseEvent(this.gameObject);
            }
        }
        
        public JoinTools_JoinSettings UpdateForkSibling()
        {
            
        }

#if UNITY_EDITOR
        
        public void ConfigureData()
        {
            forkDataList.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequenceConfigs.Count; q++) {
                    
                    var sequence = masterSequences[i].sequenceConfigs[q].sequence;
                    TimelineAsset rootTimelineAsset = sequence.sourcePlayable as TimelineAsset;
                    
                    Input_Track inputTrack =
                        Utils.GetTrackFromTimelineAsset(rootTimelineAsset, typeof(Input_Track)) as Input_Track;
                    
                    if (inputTrack == null) continue;
                    
                    IEnumerable<IMarker> markers = inputTrack.GetMarkers().OrderBy(s => s.time);

                    GetJoinToolsData(this, sequence, markers);
                }
            }
            
            EditorUtility.SetDirty(this);
        }

        private static JoinTools GetJoinToolsData(JoinTools joinTools, Sequence sequence, IEnumerable<IMarker> markers)
        {
            if (joinTools.joinSettingsCollection.ContainsKey(sequence) == false) {
                joinTools.joinSettingsCollection.Add(sequence, new JoinTools_JoinSettings());
            }
            
            foreach (IMarker marker in markers) {
                SetJoinSettings(joinTools.joinSettingsCollection[sequence], marker);
                SetForkData(joinTools, sequence, marker);
            }
            
            return joinTools;
        }

        private static JoinTools_JoinSettings SetJoinSettings(JoinTools_JoinSettings joinSettings, IMarker marker)
        {
            if (marker is IJoinSequence joinSequence && marker is JoinTools_Marker joinMarker) {
                    
                Sequence siblingSequence = joinSequence.sequenceToJoin;
                
                if (joinMarker.markerPlacement == MarkerPlacement.StartOfSequence) {
                    joinSettings.previousSequence = siblingSequence;
                } else {
                    joinSettings.nextSequence = siblingSequence;
                }
            }

            return joinSettings;
        }

        private static JoinTools SetForkData(JoinTools joinTools, Sequence sequence, IMarker marker)
        {
            if (marker is JoinTools_ForkMarker forkJoinMarker) {
                joinTools.forkDataList.Add(new JoinTools_ForkJoinData(sequence, forkJoinMarker, joinTools.forkTransitionSpread));
            }

            return joinTools;
        }
        
#endif
        [Serializable]
        private class JoinSettingsCollection : SerializableDictionaryBase<Sequence, JoinTools_JoinSettings> { }
        
        private static bool IsPopulated(SimpleEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
    
}