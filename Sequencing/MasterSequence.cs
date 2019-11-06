using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
{
    [ExecuteInEditMode]
    public class MasterSequence : MonoBehaviour
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

        [SerializeField]
        [ReadOnly]
        private ActiveInputModuleData _activeInputModule = new ActiveInputModuleData();

        private ActiveInputModuleData activeInputModule
        {
            get => _activeInputModule;
            set => _activeInputModule = value;
        }

        [SerializeField]
        private List<Sequence_Config> _sequenceConfigs = new List<Sequence_Config>();

        public List<Sequence_Config> sequenceConfigs
        {
            get => _sequenceConfigs;
        }
        
        [SerializeField]
        private List<MasterTimeData> _masterTimeDataList = new List<MasterTimeData>();

        public List<MasterTimeData> masterTimeDataList
        {
            get => _masterTimeDataList;
            private set => _masterTimeDataList = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private double _elapsedTime;

        public double elapsedTime
        {
            get => _elapsedTime;
            private set => _elapsedTime = value;
        }

        public double masterTime {
            get {
                return elapsedTime;
            }
            set {
                if (masterTimeDataList.Count < 1) {
                    Init();
                }
                SetMasterTime(value, masterTimeDataList);
            }
        }

        double _masterTotalTime;
        
        public double masterTotalTime {
            get {
                if (masterTimeDataList.Count < 1) {
                    Init();
                }
                return _masterTotalTime;
            }
        }

        private void Start()
        {
            for (int i = 0; i < sequenceConfigs.Count; i++) {
                sequenceConfigs[i].sequence.SetDefaults();
            }
        }

        public void Init()
        {
            // Generate master times for sequences
            masterTimeDataList = GenerateSequenceData(sequenceConfigs);

            for (int i = 0; i < sequenceConfigs.Count; i++) {
                sequenceConfigs[i].SetMasterSequence(this);
                sequenceConfigs[i].Init();
            }

            // Get total time by adding last sequences master time end threshold with its duration
            _masterTotalTime = masterTimeDataList[masterTimeDataList.Count - 1].masterTimeEnd + masterTimeDataList[masterTimeDataList.Count - 1].sequence.sourcePlayable.duration;
        }

        public void ProcessModifyRequest(EventPayload eventPayload)
        {
            Sequence targetSequence = eventPayload.GetScriptableObjectValue() as Sequence;
            Sequence_Config sequenceConfig = sequenceConfigs.Find(x => x.sequence == targetSequence);
            
            if (sequenceConfig == null || sequenceConfig.DependenciesLoaded() == false) return;

            int requestPriority = eventPayload.GetIntValue();
            string moduleName = eventPayload.GetStringValue();
                
            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(activeInputModule, moduleName, requestPriority);
                
                sequenceConfig.processModify.ModifySequence(eventPayload.GetFloatValue());
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

        static List<Sequence> RefreshSequences(List<Sequence> sequences, List<Sequence_Config> sequenceConfigs)
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
            if (masterTimeDataList.Count < 1) {
                Init();
            }
            return SetMasterTime(targetTime, masterTimeDataList);
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

        public void RefreshElapsedTime(EventPayload eventPayload)
        {
            Sequence targetSequence = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as Sequence;
            MasterTimeData masterTimeData = masterTimeDataList.Find(x => x.sequence == targetSequence);
                
            if(masterTimeData != null) {
                elapsedTime = masterTimeData.masterTimeStart + targetSequence.currentTime;
            }
        }

        static List<MasterTimeData> GenerateSequenceData(List<Sequence_Config> sourceSequenceConfigs)
        {
            List<MasterTimeData> sequenceData = new List<MasterTimeData>();
            for (int i = 0; i < sourceSequenceConfigs.Count; i++)
            {
                double masterTimeStart = 0d;
                double masterTimeEnd = 0d;

                if (i == 0)  {
                    masterTimeEnd = sourceSequenceConfigs[i].sequence.sourcePlayable.duration;
                } else
                {
                    masterTimeStart = sequenceData[i - 1].masterTimeEnd;
                    masterTimeEnd = sourceSequenceConfigs[i].sequence.sourcePlayable.duration + sequenceData[i - 1].masterTimeEnd;
                }

                MasterTimeData newSequenceData = new MasterTimeData(sourceSequenceConfigs[i].sequence, masterTimeStart, masterTimeEnd);
                sequenceData.Add(newSequenceData);
            }

            return sequenceData;
        }

        public static double LocalToMasterTime(MasterSequence masterSequence, Sequence sourceSequence, double localTime)
        {
            masterSequence.Init();
            List<MasterTimeData> masterTimeData = masterSequence.masterTimeDataList;
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
            get => _masterTimeStart;
            set => _masterTimeStart = value;
        }

        [SerializeField]
        private double _masterTimeEnd;
        
        public double masterTimeEnd {
            get => _masterTimeEnd;
            set => _masterTimeEnd = value;
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
        [SerializeField]
        private string _name;

        public string name
        {
            get => _name;
            set => _name = value;
        }

        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
            set => _priority = value;
        }
    }
}