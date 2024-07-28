using System;
using System.Collections.Generic;
using System.Linq;
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

        private InputGroupKey inputGroupKey => rootConfig.inputGroupKey;
        
        public bool forkTransitionActive
        {
            get => rootConfig.appSettings.GetForkTransitionActive(this.gameObject, inputGroupKey);
            private set => rootConfig.appSettings.SetForkTransitionActive(this.gameObject, inputGroupKey, value);
        }
        
        public float forkTransitionSpread =>
            rootConfig.appSettings.GetForkTransitionSpread(this.gameObject, inputGroupKey);

        private SimpleEventTrigger autoplayActivate =>
            rootConfig.appSettings.GetAutoplayActivate(this.gameObject, inputGroupKey);
        
        private ComplexEventManualTrigger boundaryReached =>
            rootConfig.appSettings.GetBoundaryReached(this.gameObject, inputGroupKey);

        private List<MasterSequence> masterSequences => rootConfig.masterSequences;

        [SerializeField]
        [ReadOnly]
        private JoinerDataCollection _joinerDataCollection;

        private JoinerDataCollection joinerDataCollection => _joinerDataCollection;

        [SerializeField]
        [ReadOnly]
        private ForkDataCollection _forkDataCollection;

        public ForkDataCollection forkDataCollection => _forkDataCollection;

        [SerializeField]
        [Required]
        private CustomKeyReference _forkKey = new CustomKeyReference();

        private CustomKey forkKey => _forkKey.GetVariable() as CustomKey;

        [SerializeField]
        [Required]
        private CustomKeyReference _updateForkViaBranchKey = new CustomKeyReference();

        private CustomKey updateForkViaBranchKey => _updateForkViaBranchKey.GetVariable() as CustomKey;

        [SerializeField]
        [Required]
        private CustomKeyReference _updateForkViaSequence = new CustomKeyReference();

        private CustomKey updateForkViaSequence => _updateForkViaSequence.GetVariable() as CustomKey;

        private void Start()
        {
            forkTransitionActive = false;
        }
        
        public void SetForkStatus(bool targetStatus)
        {
            forkTransitionActive = targetStatus;
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
                    previousSequence.active = true;
                    previousSequence.sequenceController.gameObject.SetActive(true);
                    previousSequence.sequenceController.SetSequenceTime(this, (float)previousSequence.sourcePlayable.duration);
                    previousSequence.sequenceController.masterSequence.RefreshElapsedTime(previousSequence);
                    rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                    
                    // In some cases, like looping, we don't
                    // want to deactivate the playable director
                    if (previousSequence != sourceSequence) {
                        sourceSequence.sequenceController.gameObject.SetActive(false);
                    }
                    autoplayActivate.RaiseEvent(this.gameObject);
                }
                else if (sequenceSettings.previousDestination is Fork fork) {
                    if (fork.active == false || fork.TryGetDestinationBranch(out BranchingPath destinationBranch) == false) {
                        boundaryReached.RaiseEvent(this.gameObject, sourceSequence);
                    }
                    else {
                        previousSequence = destinationBranch.sequence;
                        if (previousSequence != sourceSequence) {
                            for (int i = 0; i < fork.branchingPaths.Count; i++) {
                                fork.branchingPaths[i].sequence.active = false;
                            }
                            previousSequence.active = true;
                            previousSequence.sequenceController.gameObject.SetActive(true);
                            ForkData previousForkData = forkDataCollection[previousSequence].Find(x => x.fork == fork);
                            double targetTime = previousForkData.markerPlacement == MarkerPlacement.StartOfSequence  ? 0d : previousSequence.sourcePlayable.duration;
                            previousSequence.sequenceController.SetSequenceTime(this, (float) targetTime);
                            previousSequence.sequenceController.masterSequence.RefreshElapsedTime(previousSequence);
                            rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                            sourceSequence.sequenceController.gameObject.SetActive(false);
                            autoplayActivate.RaiseEvent(this.gameObject);
                        }
                    }
                }
            }
            else
            {
                boundaryReached.RaiseEvent(this.gameObject, sourceSequence);
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
                    nextSequence.active = true;
                    nextSequence.sequenceController.gameObject.SetActive(true);
                    nextSequence.sequenceController.SetSequenceTime(this, 0);
                    nextSequence.sequenceController.masterSequence.RefreshElapsedTime(nextSequence);
                    rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                    
                    // In some cases, namely looping, we don't
                    // want to deactivate the playable director
                    if (nextSequence != sourceSequence) {
                        sourceSequence.sequenceController.gameObject.SetActive(false);
                    }
                    autoplayActivate.RaiseEvent(this.gameObject);
                }
                else if (sequenceSettings.nextDestination is Fork fork) {
                    if (fork.active == false || fork.TryGetDestinationBranch(out BranchingPath destinationBranch) == false) {
                        boundaryReached.RaiseEvent(this.gameObject, sourceSequence);
                    }
                    else {
                        nextSequence = destinationBranch.sequence;  
                        if (nextSequence != sourceSequence) {
                            for (int i = 0; i < fork.branchingPaths.Count; i++) {
                                fork.branchingPaths[i].sequence.active = false;
                            }
                            nextSequence.active = true;
                            nextSequence.sequenceController.gameObject.SetActive(true);
                            ForkData nextForkData = forkDataCollection[nextSequence].Find(x => x.fork == fork);
                            double targetTime = nextForkData.markerPlacement == MarkerPlacement.StartOfSequence ? 0d : nextSequence.sourcePlayable.duration;
                            nextSequence.sequenceController.SetSequenceTime(this, (float)targetTime);
                            nextSequence.sequenceController.masterSequence.RefreshElapsedTime(nextSequence);
                            rootConfig.sequenceModified.RaiseEvent(this.gameObject);
                            
                            sourceSequence.sequenceController.gameObject.SetActive(false);
                            autoplayActivate.RaiseEvent(this.gameObject);
                        }    
                    }
                }
            }
            else
            {
                boundaryReached.RaiseEvent(this.gameObject, sourceSequence);
            }

            return sourceSequence;

        }

        public void CallActivateNextSequence(Sequence sourceSequence)
        {
            ActivateNextSequence(sourceSequence);
        }

        public void UpdateFork(ComplexPayload complexPayload)
        {
            Fork targetFork = complexPayload.GetScriptableObjectValue(forkKey) as Fork;
            BranchKey branchKey = complexPayload.GetScriptableObjectValue(updateForkViaBranchKey) as BranchKey;
            Sequence sequence = complexPayload.GetScriptableObjectValue(updateForkViaSequence) as Sequence;
            
            // Only execute the update if joiner has data for the fork. This is
            // to prevent potential conflicts if we have multiple joiners in one scene

            bool containsTargetForkData = false;
            foreach(KeyValuePair<Sequence, List<ForkData>> forkDatum in forkDataCollection)
            {
                if (forkDatum.Value.Find(x => x.fork == targetFork) != null) {
                    containsTargetForkData = true;
                    break;
                }   
            }
            if (containsTargetForkData == false) {
                return;
            }

            if (branchKey != null && sequence != null) {
                Debug.LogError("Unable to update fork; please use either a branch key or sequence, not both.", this);
                return;
            }

            if (branchKey != null) {
                targetFork.SetDestinationBranch(branchKey);
            }

            if (sequence != null) {
                targetFork.SetDestinationBranch(sequence);
            }

        }

//#if UNITY_EDITOR
        
        public void ConfigureData()
        {
            joinerDataCollection.Clear();
            forkDataCollection.Clear();

            for (int i = 0; i < masterSequences.Count; i++)
            {
                for (int q = 0; q < masterSequences[i].sequenceControllers.Count; q++) {
                    
                    var sequence = masterSequences[i].sequenceControllers[q].sequence;
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
                    Debug.Log("You must add a fork to the marker at " + joinMarker.time + " on sequence " + sequence.sequenceController.gameObject.name,
                        sequence.sequenceController.gameObject);
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
        
    }
}