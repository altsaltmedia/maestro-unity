using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Sequencing
{
    public class MasterSequence : MonoBehaviour
    {
        [SerializeField]
        private List<InputController> _inputModules = new List<InputController>();

        private List<InputController> inputModules
        {
            get => _inputModules;
        }

        [SerializeField]
        private List<Sequence> _sequences = new List<Sequence>();

        public List<Sequence> sequences
        {
            get => _sequences;
        }

        List<SequenceListData> _sequenceData = new List<SequenceListData>();

        public List<SequenceListData> sequenceData
        {
            get => sequenceData;
            private set => sequenceData = value;
        }

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
            masterTotalTime = sequenceData[sequenceData.Count - 1].masterTimeEnd + sequenceData[sequenceData.Count - 1].sequence.sourcePlayable.duration;
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

                if (targetTime < sequenceData[i].masterTimeEnd) {

                    activeSequence = sequenceData[i].sequence;
                    activeSequence.active = true;
                    activeSequence.syncer.gameObject.SetActive(true);

                    // Update the active sequence
                    if (i == 0) {
                        activeSequence.currentTime = targetTime;
                    } else {
                        activeSequence.currentTime = targetTime - sequenceData[i - 1].masterTimeEnd;
                    }

                    // Prep adjacent sequences
                    if (i > 0) {
                        sequenceData[i - 1].sequence.currentTime = sequenceData[i - 1].sequence.sourcePlayable.duration;
                    }

                    if (sequenceData.Count - 1 > i) {
                        sequenceData[i + 1].sequence.currentTime = 0;
                    }

                    // Deactivate other sequences
                    for (int z = 0; z < sequenceData.Count; z++) {

                        if (z == i) {
                            continue;
                        }

                        sequenceData[z].sequence.active = false;
                        sequenceData[z].sequence.syncer.gameObject.SetActive(false);
                    }

                    break;
                }
            }

            return activeSequence;
        }

        static double GetElapsedTime(List<SequenceListData> sequenceData)
        {
            double elapsedTime = 0d;

            for (int i = 0; i < sequenceData.Count; i++) {
                
                if (sequenceData[i].sequence.active == true) {

                    if (i == 0) {
                        elapsedTime = sequenceData[i].sequence.currentTime;
                    } else {
                        elapsedTime = sequenceData[i].masterTimeStart + sequenceData[i].sequence.currentTime;
                    }

                }
            }

            return elapsedTime;
        }

        static List<SequenceListData> GenerateSequenceData(List<Sequence> sourceSequences)
        {
            List<SequenceListData> sequenceData = new List<SequenceListData>();
            for (int i = 0; i < sourceSequences.Count; i++)
            {
                double masterTimeStart = 0d;
                double masterTimeEnd = 0d;

                if (i == 0)  {
                    masterTimeEnd = sourceSequences[i].sourcePlayable.duration;
                } else  {
                    masterTimeStart = sequenceData[i - 1].sequence.sourcePlayable.duration;
                    masterTimeEnd = sourceSequences[i].sourcePlayable.duration + sequenceData[i - 1].masterTimeEnd;
                }

                SequenceListData newSequenceData = new SequenceListData(sourceSequences[i], masterTimeStart, masterTimeEnd);
                sequenceData.Add(newSequenceData);
            }

            return sequenceData;
        }

        public double LocalToMasterTime(List<SequenceListData> sequenceData, Sequence sourceSequence, double localTime)
        {
            for (int i = 0; i < sequenceData.Count; i++)
            {
                if (sequenceData[i].sequence == sourceSequence)
                {
                    return sequenceData[i].masterTimeStart + localTime;
                }
            }
            
            throw new SystemException("Source sequence not found in sequence data.");
        }
    }

    [Serializable]
    public class SequenceListData
    {
        [SerializeField]
        Sequence _sequence;
        public Sequence sequence {
            get => _sequence;
            set => _sequence = value;
        }
        
        [SerializeField]
        double _masterTimeStart;
        
        public double masterTimeStart {
            get => masterTimeStart;
            set => masterTimeStart = value;
        }

        [SerializeField]
        double _masterTimeEnd;
        
        public double masterTimeEnd {
            get => masterTimeEnd;
            set => masterTimeEnd = value;
        }

        public SequenceListData(Sequence sourceSequence, double masterTimeStart, double masterTimeEnd)
        {
            this.sequence = sourceSequence;
            this.masterTimeStart = masterTimeStart;
            this.masterTimeEnd = masterTimeEnd;
        }
    }
}