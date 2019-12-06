using System;
using System.Collections.Generic;
using System.Linq;
using AltSalt.Maestro.Sequencing.Touch;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    public class Joiner : MonoBehaviour
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
        private BoolReference _forkTransitionActive;

        public bool forkTransitionActive
        {
            get => _forkTransitionActive.Value;
            private set => _forkTransitionActive.Variable.SetValue(value);
        }

        [SerializeField]
        [ReadOnly]
        private JoinerDataCollection _joinerDataCollection;

        private JoinerDataCollection joinerDataCollection
        {
            get => _joinerDataCollection;
            set => _joinerDataCollection = value;
        }

        [SerializeField]
        [ReadOnly]
        private ForkDataCollection _forkDataCollection;

        public ForkDataCollection forkDataCollection
        {
            get => _forkDataCollection;
            private set => _forkDataCollection = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private FloatReference _forkTransitionSpread;

        public float forkTransitionSpread
        {
            get => _forkTransitionSpread.Value;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private SimpleEventTrigger _boundaryReached;

        private SimpleEventTrigger boundaryReached
        {
            get => _boundaryReached;
            set => _boundaryReached = value;
        }

        private void Start()
        {
            forkTransitionActive = false;
        }
        
        public void SetForkStatus(bool targetStatus)
        {
            forkTransitionActive = targetStatus;
            onForkUpdate.RaiseEvent(this.gameObject);
        }

        public Sequence ActivatePreviousSequence(Sequence sourceSequence)
        {
            JoinerData sequenceSettings = joinerDataCollection[sourceSequence];

            if (sequenceSettings.previousDestination != null)
            {
                Sequence previousSequence;
                
                if (sequenceSettings.previousDestination is Sequence sequence) {
                    sourceSequence.active = false;
                    previousSequence = sequence;
                    previousSequence.currentTime = previousSequence.sourcePlayable.duration;
                    previousSequence.active = true;
                    previousSequence.sequenceConfig.gameObject.SetActive(true);
                    previousSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                    rootConfig.sequenceModified.RaiseEvent(this.gameObject, previousSequence);
                    sourceSequence.sequenceConfig.gameObject.SetActive(false);
                }
                else if (sequenceSettings.previousDestination is Fork fork) {
                    if (fork.TryGetDestinationBranch(out BranchingPath destinationBranch) == false) {
                        boundaryReached.RaiseEvent(this.gameObject);
                    }
                    else {
                        previousSequence = destinationBranch.sequence;
                        if (previousSequence != sourceSequence) {
                            for (int i = 0; i < fork.branchingPaths.Count; i++) {
                                fork.branchingPaths[i].sequence.active = false;
                            }
                            ForkData previousForkData = forkDataCollection[previousSequence].Find(x => x.fork == fork);
                            previousSequence.currentTime =  previousForkData.markerPlacement == MarkerPlacement.StartOfSequence  ? 0d : previousSequence.sourcePlayable.duration;
                            previousSequence.active = true;
                            previousSequence.sequenceConfig.gameObject.SetActive(true);
                            previousSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                            rootConfig.sequenceModified.RaiseEvent(this.gameObject, previousSequence);
                            sourceSequence.sequenceConfig.gameObject.SetActive(false);
                        }
                    }
                }

            }
            else
            {
                boundaryReached.RaiseEvent(this.gameObject);
            }

            return sourceSequence;
        }
        
        public Sequence ActivateNextSequence(Sequence sourceSequence)
        {
            JoinerData sequenceSettings = joinerDataCollection[sourceSequence];

            if (sequenceSettings.nextDestination != null) {
                Sequence nextSequence;
                
                if (sequenceSettings.nextDestination is Sequence sequence) {
                    sourceSequence.active = false;
                    nextSequence = sequence;
                    nextSequence.currentTime = 0d;
                    nextSequence.active = true;
                    nextSequence.sequenceConfig.gameObject.SetActive(true);
                    nextSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                    rootConfig.sequenceModified.RaiseEvent(this.gameObject, nextSequence);
                    sourceSequence.sequenceConfig.gameObject.SetActive(false);
                }
                else if (sequenceSettings.nextDestination is Fork fork) {
                    if (fork.TryGetDestinationBranch(out BranchingPath destinationBranch) == false) {
                        boundaryReached.RaiseEvent(this.gameObject);
                    }
                    else {
                        nextSequence = destinationBranch.sequence;  
                        if (nextSequence != sourceSequence) {
                            for (int i = 0; i < fork.branchingPaths.Count; i++) {
                                fork.branchingPaths[i].sequence.active = false;
                            }
                            ForkData nextForkData = forkDataCollection[nextSequence].Find(x => x.fork == fork);
                            nextSequence.currentTime = nextForkData.markerPlacement == MarkerPlacement.StartOfSequence ? 0d : nextSequence.sourcePlayable.duration;
                            nextSequence.active = true;
                            nextSequence.sequenceConfig.gameObject.SetActive(true);
                            nextSequence.sequenceConfig.syncTimeline.RefreshPlayableDirector();
                            rootConfig.sequenceModified.RaiseEvent(this.gameObject, nextSequence);
                            sourceSequence.sequenceConfig.gameObject.SetActive(false);
                        }    
                    }
                }
            }
            else
            {
                boundaryReached.RaiseEvent(this.gameObject);
            }

            return sourceSequence;

        }

//#if UNITY_EDITOR
        
        public void ConfigureData()
        {
            joinerDataCollection.Clear();
            forkDataCollection.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequenceConfigs.Count; q++) {
                    
                    var sequence = masterSequences[i].sequenceConfigs[q].sequence;
                    TimelineAsset rootTimelineAsset = sequence.sourcePlayable as TimelineAsset;
                    
                    ConfigTrack configTrack =
                        Utils.GetTrackFromTimelineAsset(rootTimelineAsset, typeof(ConfigTrack)) as ConfigTrack;
                    
                    if (configTrack == null) continue;
                    
                    IEnumerable<IMarker> markers = configTrack.GetMarkers().OrderBy(s => s.time);

                    GetJoinData(this, sequence, markers);
                }
            }
#if UNITY_EDITOR            
            EditorUtility.SetDirty(this);
#endif
        }

        private static Joiner GetJoinData(Joiner joiner, Sequence sequence, IEnumerable<IMarker> markers)
        {
            // We need to make an entry for every sequence, regardless of whether it
            // has any sibling sequences, so we know when we've reached the end of a path 
            if (joiner.joinerDataCollection.ContainsKey(sequence) == false) {
                joiner.joinerDataCollection.Add(sequence, new JoinerData());
            }
            
            foreach (IMarker marker in markers) {
                SetJoinData(joiner, sequence, marker);
                SetForkData(joiner, sequence, marker);
            }
            
            return joiner;
        }

        private static JoinerDataCollection SetJoinData(Joiner joiner, Sequence sequence, IMarker marker)
        {
            
            if (marker is JoinMarker_IJoinSequence joinSequence && marker is JoinMarker joinMarker) {
                
                ScriptableObject joinDestination = joinSequence.joinDestination;
                
                JoinerData joinerData = joiner.joinerDataCollection[sequence];

                if (joinMarker.markerPlacement == MarkerPlacement.StartOfSequence) {
                    joinerData.previousDestination = joinDestination;
                } else {
                    joinerData.nextDestination = joinDestination;
                }
            }

            return joiner.joinerDataCollection;
        }

        private static ForkDataCollection SetForkData(Joiner joiner, Sequence sequence, IMarker marker)
        {
            if (marker is JoinMarker_IJoinSequence joinSequence && marker is JoinMarker joinMarker && joinSequence.joinDestination is Fork fork) {

                if (joinSequence.joinDestination == null) {
                    Debug.Log("You must add a fork to the marker at " + joinMarker.time + " on sequence " + sequence.sequenceConfig.gameObject.name,
                        sequence.sequenceConfig.gameObject);
                    return joiner.forkDataCollection;
                }

                if (joiner.forkDataCollection.ContainsKey(sequence) == false) {
                    joiner.forkDataCollection.Add(sequence, new List<ForkData>());
                }

                if (joiner.forkDataCollection[sequence].Find(x => x.joinMarker == joinSequence) == null) {
                    joiner.forkDataCollection[sequence].Add(new ForkData(sequence, joinMarker, fork));
                }
            }

            return joiner.forkDataCollection;
        }
        
//#endif
        [Serializable]
        private class JoinerDataCollection : SerializableDictionaryBase<Sequence, JoinerData> { }
        
        [Serializable]
        public class ForkDataCollection : SerializableDictionaryBase<Sequence, List<ForkData>> { }
        
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