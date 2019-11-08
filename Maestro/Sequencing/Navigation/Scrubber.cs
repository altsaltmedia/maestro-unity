using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Navigate
{

    public class Scrubber : Input_Module
    {

        [SerializeField]
        SimpleEventTrigger sequenceScrubbed;

        Slider slider;

        [SerializeField]
        MasterSequence _masterSequence;

        public List<Sequence_SyncTimeline> sequenceDirectors = new List<Sequence_SyncTimeline>();

        [ReadOnly]
        public List<double> sequenceThresholds = new List<double>();

        double maxTime;
        float previousValue;

        // Use this for initialization
        void Start()
        {
            slider = GetComponent<Slider>();
            slider.maxValue = (float)_masterSequence.masterTotalTime;
        }

        public void SetScrubberTime()
        {
            slider.value = (float)_masterSequence.elapsedTime;
        }

        private void OnMouseUpAsButton()
        { 
            TriggerInputActionComplete();
        }

        // TO DO - Clean this up.
        public void ScrubSequence(float newValue)
        {
            _masterSequence.masterTime = newValue;
            sequenceScrubbed.RaiseEvent(this.gameObject);
            return;

            Sequence activeSequence = _masterSequence.UpdateMasterTime(newValue);
            for (int i = 0; i < _masterSequence.sequenceConfigs.Count; i++) {

                int activeSequenceIndex = 0;

                if (_masterSequence.sequenceConfigs[i] == activeSequence) {
                    activeSequenceIndex = i;
                }

                if (newValue > previousValue) {
                    for (int z = 0; z < sequenceDirectors.Count; z++) {
                        if (z < activeSequenceIndex) {
                            //sequenceDirectors[z].SetToEnd();
                            sequenceDirectors[z].gameObject.SetActive(false);
                            //if (sequenceDirectors[z].playableDirector.playableGraph.IsValid()) {
                            //    sequenceDirectors[z].playableDirector.playableGraph.Destroy();
                            //}
                        } else if (z == activeSequenceIndex) {
                            sequenceDirectors[z].gameObject.SetActive(true);
                            sequenceDirectors[z].ForceEvaluate();
                            //sequenceDirectors[z].gameObject.GetComponent<PlayableDirector>().enabled = true;
                        } else if (z > activeSequenceIndex) {
                            sequenceDirectors[z].gameObject.SetActive(false);
                            //if (sequenceDirectors[z].playableDirector.playableGraph.IsValid()) {
                            //    sequenceDirectors[z].playableDirector.playableGraph.Destroy();
                            //}
                            //sequenceDirectors[z].gameObject.GetComponent<PlayableDirector>().enabled = false;
                        }
                    }
                } else if (newValue < previousValue) {
                    for (int z = sequenceDirectors.Count - 1; z >= 0; z--) {
                        if (z < activeSequenceIndex) {
                            sequenceDirectors[z].gameObject.SetActive(false);
                            //sequenceDirectors[z].gameObject.GetComponent<PlayableDirector>().enabled = false;
                        } else if (z == activeSequenceIndex) {
                            sequenceDirectors[z].gameObject.SetActive(true);
                            sequenceDirectors[z].ForceEvaluate();
                            //sequenceDirectors[z].gameObject.GetComponent<PlayableDirector>().enabled = true;
                        } else if (z > activeSequenceIndex) {
                            sequenceDirectors[z].gameObject.SetActive(false);
                            //if (sequenceDirectors[z].playableDirector.playableGraph.IsValid()) {
                            //    sequenceDirectors[z].playableDirector.playableGraph.Destroy();
                            //}
                            //sequenceDirectors[z].ForceEvaluate();
                            //sequenceDirectors[z].SetToBeginning();
                            //sequenceDirectors[z].gameObject.GetComponent<PlayableDirector>().enabled = false;
                        }
                    }
                }
            }

            previousValue = newValue;

            //sequenceList.MasterTime = newValue;
            //sequenceScrubbed.RaiseEvent(this.gameObject);

            //for (int i = 0; i < sequenceThresholds.Count; i++) {
            //    if(newValue < sequenceThresholds[i]) {
            //        sequences[i].Active = true;
            //        sequenceDirectors[i].enabled = true;

            //        // Modify all sequences
            //        if (i > 0) {
            //            sequences[i].currentTime = newValue - sequenceThresholds[i - 1];
            //            sequences[i - 1].currentTime = sequenceDirectors[i - 1].duration;
            //        } else {
            //            sequences[i].currentTime = newValue;
            //        }

            //        if (sequenceThresholds.Count > i + 1) {
            //            sequences[i + 1].currentTime = 0;
            //        }

            //        sequenceScrubbed.RaiseEvent(this.gameObject);


            //        // Deactivate adjacent sequences
            //        if(i > 0) {
            //            sequences[i - 1].Active = false;
            //        }

            //        if(sequenceThresholds.Count > i + 1) {
            //            sequences[i + 1].Active = false;
            //        }

            //        break;
            //    }
            //}
        }
    }

}