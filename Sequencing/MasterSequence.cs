using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
{
    public class MasterSequence : MonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        private ActiveInputModuleData _activeInputModule = new ActiveInputModuleData();

        private ActiveInputModuleData activeInputModule
        {
            get => _activeInputModule;
            set => _activeInputModule = value;
        }


        [SerializeField]
        private List<InputController> _inputModules = new List<InputController>();

        private List<InputController> inputModules
        {
            get => _inputModules;
        }

        [SerializeField]
        private List<SequenceConfig> _sequenceConfigs = new List<SequenceConfig>();

        public List<SequenceConfig> sequenceConfigs
        {
            get => _sequenceConfigs;
        }
        
        List<MasterTimeData> _masterTimeData = new List<MasterTimeData>();

        public List<MasterTimeData> masterTimeData
        {
            get => masterTimeData;
            private set => masterTimeData = value;
        }

        double masterTotalTime;

        public double ElapsedTime {
            get {
                if (masterTimeData.Count < 1) {
                    Init();
                }
                return GetElapsedTime(masterTimeData);
            }
        }

        public double MasterTime {
            get {
                return ElapsedTime;
            }
            set {
                if (masterTimeData.Count < 1) {
                    Init();
                }
                SetMasterTime(value, masterTimeData);
            }
        }

        public double MasterTotalTime {
            get {
                if (masterTimeData.Count < 1) {
                    Init();
                }
                return masterTotalTime;
            }
        }

        void Init()
        {
            // Generate master times for sequences
            masterTimeData = GenerateSequenceData(sequenceConfigs);

            // Get total time by adding last sequences master time end threshold with its duration
            masterTotalTime = masterTimeData[masterTimeData.Count - 1].masterTimeEnd + masterTimeData[masterTimeData.Count - 1].sequence.sourcePlayable.duration;
        }

        public void ProcessModifyRequest(EventPayload eventPayload)
        {
            Sequence targetSequence = eventPayload.GetScriptableObjectValue() as Sequence;
            SequenceConfig sequenceConfig = sequenceConfigs.Find(x => x.sequence == targetSequence);
            
            if (sequenceConfig == null) return;

            int requestPriority = eventPayload.GetIntValue();
            if (string.IsNullOrEmpty(activeInputModule.name) || requestPriority > activeInputModule.priority)
            {
                LockInputModule(activeInputModule, eventPayload.GetStringValue(), requestPriority);
                
                Sequence.ModifySequence(sequenceConfig.sequence, eventPayload.GetFloatValue());
            }
        }

        private static ActiveInputModuleData LockInputModule(ActiveInputModuleData activeInputModule, string moduleName, int priority)
        {
            activeInputModule.name = moduleName;
            activeInputModule.priority = priority;

            return activeInputModule;
        }

        public void UnlockInputeModule(EventPayload eventPayload)
        {
            if (activeInputModule.name != eventPayload.GetStringValue()) return;
            
            activeInputModule.name = string.Empty;
            activeInputModule.priority = 0;
        }

        static List<Sequence> RefreshSequences(List<Sequence> sequences, List<SequenceConfig> sequenceConfigs)
        {
            sequences.Clear();

            for (int i = 0; i < sequenceConfigs.Count; i++)
            {
                sequences.Add(sequenceConfigs[i].sequence);
            }

            return sequences;
        }

        public Sequence UpdateMasterTime(double targetTime)
        {
            if (masterTimeData.Count < 1) {
                Init();
            }
            return SetMasterTime(targetTime, masterTimeData);
        }

        public static Sequence SetMasterTime(double targetTime, List<MasterTimeData> sequenceData)
        {
            Sequence activeSequence = null;

            for (int i = 0; i < sequenceData.Count; i++) {

                if (targetTime < sequenceData[i].masterTimeEnd) {

                    activeSequence = sequenceData[i].sequence;
                    activeSequence.active = true;
                    activeSequence.sequenceConfig.gameObject.SetActive(true);

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
                        sequenceData[z].sequence.sequenceConfig.gameObject.SetActive(false);
                    }

                    break;
                }
            }

            return activeSequence;
        }

        static double GetElapsedTime(List<MasterTimeData> sequenceData)
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

        static List<MasterTimeData> GenerateSequenceData(List<SequenceConfig> sourceSequenceConfigs)
        {
            List<MasterTimeData> sequenceData = new List<MasterTimeData>();
            for (int i = 0; i < sourceSequenceConfigs.Count; i++)
            {
                double masterTimeStart = 0d;
                double masterTimeEnd = 0d;

                if (i == 0)  {
                    masterTimeEnd = sourceSequenceConfigs[i].sequence.sourcePlayable.duration;
                } else  {
                    masterTimeStart = sequenceData[i - 1].sequence.sourcePlayable.duration;
                    masterTimeEnd = sourceSequenceConfigs[i].sequence.sourcePlayable.duration + sequenceData[i - 1].masterTimeEnd;
                }

                MasterTimeData newSequenceData = new MasterTimeData(sourceSequenceConfigs[i].sequence, masterTimeStart, masterTimeEnd);
                sequenceData.Add(newSequenceData);
            }

            return sequenceData;
        }

        public static double LocalToMasterTime(List<MasterTimeData> masterTimeData, Sequence sourceSequence, double localTime)
        {
            for (int i = 0; i < masterTimeData.Count; i++)
            {
                if (masterTimeData[i].sequence == sourceSequence)
                {
                    return masterTimeData[i].masterTimeStart + localTime;
                }
            }
            
            throw new SystemException("Source sequence not found in sequence data.");
        }
    }

    [Serializable]
    public class MasterTimeData
    {
        [SerializeField]
        private Sequence _sequence;
        
        public Sequence sequence {
            get => _sequence;
            set => _sequence = value;
        }
        
        [SerializeField]
        private double _masterTimeStart;
        
        public double masterTimeStart {
            get => masterTimeStart;
            set => masterTimeStart = value;
        }

        [SerializeField]
        private double _masterTimeEnd;
        
        public double masterTimeEnd {
            get => masterTimeEnd;
            set => masterTimeEnd = value;
        }

        public MasterTimeData(Sequence sourceSequence, double masterTimeStart, double masterTimeEnd)
        {
            this.sequence = sourceSequence;
            this.masterTimeStart = masterTimeStart;
            this.masterTimeEnd = masterTimeEnd;
        }
    }

    [Serializable]
    public class ActiveInputModuleData
    {
        private string _name;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        private int _priority;

        public int priority
        {
            get => _priority;
            set => _priority = value;
        }
    }
}