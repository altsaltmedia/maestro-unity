using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt
{

    public class SequenceScrubber : MonoBehaviour
    {

        public SimpleEventTrigger sequenceModified;

        Slider slider;

        public List<Sequence> sequences = new List<Sequence>();
        public List<PlayableDirector> sequenceDirectors = new List<PlayableDirector>();

        [ReadOnly]
        public List<double> sequenceThresholds = new List<double>();

        double maxTime;

        // Use this for initialization
        void Start () {
            slider = GetComponent<Slider>();
            for (int i = 0; i < sequenceDirectors.Count; i++) {
                maxTime += sequenceDirectors[i].duration;
                sequenceThresholds.Add(maxTime);
            }
            slider.maxValue = (float)maxTime;
        }
        
        // TO DO - Clean this up.
        public void ScrubSequence (float newValue) {
            for (int i = 0; i < sequenceThresholds.Count; i++) {
                if(newValue < sequenceThresholds[i]) {
                    sequences[i].Active = true;
                    sequenceDirectors[i].gameObject.SetActive(true);

                    // Modify all sequences
                    if (i > 0) {
                        sequences[i].currentTime = newValue - sequenceThresholds[i - 1];
                        sequences[i - 1].currentTime = sequenceDirectors[i - 1].duration;
                    } else {
                        sequences[i].currentTime = newValue;
                    }

                    if (sequenceThresholds.Count > i + 1) {
                        sequences[i + 1].currentTime = 0;
                    }

                    sequenceModified.RaiseEvent(this.gameObject);


                    // Deactivate adjacent sequences
                    if(i > 0) {
                        sequences[i - 1].Active = false;
                    }

                    if(sequenceThresholds.Count > i + 1) {
                        sequences[i + 1].Active = false;
                    }
                       
                    break;
                }
            }
        }
    }
    
}