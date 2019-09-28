using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Sequence List")]
    public class SequenceList : ScriptableObject
    {
        public List<Sequence> sequences = new List<Sequence>();
        List<SequenceListData> sequenceData = new List<SequenceListData>();
        double masterTotalTime;

        public double ElapsedTime {
            get {
                if (sequenceData.Count < 1) {
                    Init();
                }
                return GetElapsedTime(sequenceData);
            }
        }

        public double MasterTime {
            get {
                return ElapsedTime;
            }
            set {
                if (sequenceData.Count < 1) {
                    Init();
                }
                SetMasterTime(value, sequenceData);
            }
        }

        public double MasterTotalTime {
            get {
                if (sequenceData.Count < 1) {
                    Init();
                }
                return masterTotalTime;
            }
        }
        
        void Init()
        {
            // Generate master times for sequences
            sequenceData = GenerateSequenceData(sequences);

            // Get total time by adding last sequences master time end threshold with its duration
            masterTotalTime = sequenceData[sequenceData.Count - 1].MasterTimeEnd + sequenceData[sequenceData.Count - 1].SourceSequence.SourcePlayable.duration;
        }

        public Sequence UpdateMasterTime(double targetTime)
        {
            if (sequenceData.Count < 1) {
                Init();
            }
            return SetMasterTime(targetTime, sequenceData);
        }

        public static Sequence SetMasterTime(double targetTime, List<SequenceListData> sequenceData)
        {
            Sequence activeSequence = null;

            for (int i = 0; i < sequenceData.Count; i++) {

                if (targetTime < sequenceData[i].MasterTimeEnd) {

                    activeSequence = sequenceData[i].SourceSequence;
                    activeSequence.Active = true;
                    activeSequence.Director.gameObject.SetActive(true);

                    // Update the active sequence
                    if (i == 0) {
                        activeSequence.currentTime = targetTime;
                    } else {
                        activeSequence.currentTime = targetTime - sequenceData[i - 1].MasterTimeEnd;
                    }

                    // Prep adjacent sequences
                    if (i > 0) {
                        sequenceData[i - 1].SourceSequence.currentTime = sequenceData[i - 1].SourceSequence.SourcePlayable.duration;
                    }

                    if (sequenceData.Count - 1 > i) {
                        sequenceData[i + 1].SourceSequence.currentTime = 0;
                    }

                    // Deactivate other sequences
                    for(int z=0; z<sequenceData.Count; z++) {

                        if(z == i) {
                            continue;
                        }

                        sequenceData[z].SourceSequence.Active = false;
                        sequenceData[z].SourceSequence.Director.gameObject.SetActive(false);
                    }

                    break;
                }
            }

            return activeSequence;
        }

        static double GetElapsedTime(List<SequenceListData> sequenceData)
        {
            double elapsedTime = 0d;

            for (int i=0; i<sequenceData.Count; i++) {
                if(sequenceData[i].SourceSequence.Active == true) {

                    if(i == 0) {
                        elapsedTime = sequenceData[i].SourceSequence.currentTime;
                    } else {
                        elapsedTime = sequenceData[i].SourceSequence.currentTime + sequenceData[i - 1].MasterTimeEnd;
                    }

                }
            }

            return elapsedTime;
        }

        static List<SequenceListData> GenerateSequenceData(List<Sequence> sourceSequences)
        {
            List<SequenceListData> sequenceData = new List<SequenceListData>();
            for(int i=0; i<sourceSequences.Count; i++) {

                double masterTimeEnd = 0d;

                if(i == 0) {
                    masterTimeEnd = sourceSequences[i].SourcePlayable.duration;
                } else {
                    masterTimeEnd = sourceSequences[i].SourcePlayable.duration + sequenceData[i - 1].MasterTimeEnd;
                }

                SequenceListData newSequenceData = new SequenceListData(sourceSequences[i], masterTimeEnd);
                sequenceData.Add(newSequenceData);
            }

            return sequenceData;
        }
    }

    [Serializable]
    public class SequenceListData
    {
        [SerializeField]
        Sequence sourceSequence;
        public Sequence SourceSequence {
            get {
                return sourceSequence;
            }
        }

        [SerializeField]
        double masterTimeEnd;
        public double MasterTimeEnd {
            get {
                return masterTimeEnd;
            }
        }

        public SequenceListData(Sequence sourceSequence, double masterTimeEnd)
        {
            this.sourceSequence = sourceSequence;
            this.masterTimeEnd = masterTimeEnd;
        }
    }
}