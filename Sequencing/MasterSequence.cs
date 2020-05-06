using System;
using System.Linq;
using System.Collections.Generic;
using AltSalt.Maestro.Sequencing.Autorun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing
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

        [FormerlySerializedAs("_sequenceConfigs")]
        [SerializeField]
        [ListDrawerSettings(Expanded = true)]
        private List<SequenceController> _sequenceControllers = new List<SequenceController>();

        public List<SequenceController> sequenceControllers => _sequenceControllers;

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
            private set
            {
                if (masterTimeDataList.Count < 1) {
                    Init();
                }
                _elapsedTime = value;
            }
        }

        [ShowInInspector]
        [ReadOnly]
        private double _duration;
        
        public double duration {
            get {
                if (masterTimeDataList.Count < 1) {
                    Init();
                }
                return _duration;
            }
            private set => _duration = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private bool _hasActiveSequence;

        public bool hasActiveSequence
        {
            get => _hasActiveSequence;
            set => _hasActiveSequence = value;
        }
        
        private event SequenceUpdatedHandler _sequenceUpdated = (sender, updatedSequence) => { };
        
        public event SequenceUpdatedHandler sequenceUpdated
        {
            add
            {
                if (_sequenceUpdated == null
                    || _sequenceUpdated.GetInvocationList().Contains(value) == false) {
                    _sequenceUpdated += value;
                }
            }
            remove => _sequenceUpdated -= value;
        }
        
        private event SequenceUpdatedHandler _sequenceBoundaryReached = (sender, updatedSequence) => { };
        
        public event SequenceUpdatedHandler sequenceBoundaryReached
        {
            add
            {
                if (_sequenceBoundaryReached == null
                    || _sequenceBoundaryReached.GetInvocationList().Contains(value) == false) {
                    _sequenceBoundaryReached += value;
                }
            }
            remove => _sequenceBoundaryReached -= value;
        }

        public void Init()
        {
            // Validate before generating sequence data
            for (int i = 0; i < sequenceControllers.Count; i++) {
                if (sequenceControllers[i] == null) {
                    Debug.LogError("Sequence controller is missing", this);
                    return;
                }
                    
                if (sequenceControllers[i].sequence == null) {
                    Debug.LogError("Sequence controller is missing a target sequence", sequenceControllers[i]);
                    return;
                }
            }
            
            // Generate master times for sequences
            masterTimeDataList = GenerateSequenceData(sequenceControllers);

            for (int i = 0; i < sequenceControllers.Count; i++) {
                sequenceControllers[i].Init(this);
            }
            
            // Get total time from master time of last sequence
            duration = masterTimeDataList[masterTimeDataList.Count - 1].masterTimeEnd;
        }

        /// <summary>
        /// Simple modification requests will simply send details to update the sequence,
        /// then update that sequence manually every frame. 
        /// </summary>
        /// <param name="targetSequence"></param>
        /// <param name="requestPriority"></param>
        /// <param name="moduleName"></param>
        /// <param name="timeModifier"></param>
        /// <returns></returns>
        public Sequence RequestModifySequenceTime(Sequence targetSequence, int requestPriority, string moduleName, float timeModifier)
        {
            if (rootConfig.appUtilsRequested == true) {
                return targetSequence;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(this, moduleName, requestPriority);
                
                sequenceController.ModifySequenceTime(timeModifier);
            }

            return targetSequence;
        }
        
        /// <summary>
        /// Autoplay modules will want to activate autoplay once, and then cache the interval in which
        /// it was activated so it can halt when the end of that interval is reached.
        /// </summary>
        /// <param name="targetSequence"></param>
        /// <param name="requestPriority"></param>
        /// <param name="moduleName"></param>
        /// <param name="targetSpeed"></param>
        /// <param name="requestSuccessful"></param>
        /// <returns></returns>
        public Sequence RequestActivateForwardAutoplay(Sequence targetSequence, int requestPriority, string moduleName, float targetSpeed, out bool requestSuccessful)
        {
            requestSuccessful = false;
            
            if (rootConfig.appUtilsRequested == true) {
                return targetSequence;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                activeInputModule = LockInputModule(this, moduleName, requestPriority);
                
                sequenceController.ActivateForwardAutoplayState(targetSpeed);
                requestSuccessful = true;
            }

            return targetSequence;
        }
        
        /// <summary>
        /// Once the end of an autoplay interval is reached,
        /// we'll want to revert to manual update status.
        /// </summary>
        /// <param name="targetSequence"></param>
        /// <param name="requestPriority"></param>
        /// <param name="moduleName"></param>
        public void RequestDeactivateForwardAutoplay(Sequence targetSequence, int requestPriority, string moduleName)
        {
            if (rootConfig.appUtilsRequested == true) {
                return;
            }
            
            SequenceController sequenceController = sequenceControllers.Find(x => x.sequence == targetSequence);

            if (string.IsNullOrEmpty(activeInputModule.name) || activeInputModule.name == moduleName || requestPriority > activeInputModule.priority)
            {
                sequenceController.ActivateManualUpdateState();
            }
        }

        /// <summary>
        /// We want certain modules to take precedence over others. For example,
        /// the minute a swipe is initiated, we want the swipe to take priority no matter what;
        /// and for autoplaying, we want autoplayers to override momentum the moment an autoplay
        /// interval is reached. 
        /// </summary>
        /// <param name="activeInputModule"></param>
        /// <param name="moduleName"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static ActiveInputModuleData LockInputModule(MasterSequence masterSequence, string moduleName, int priority)
        {
            masterSequence.activeInputModule.name = moduleName;
            masterSequence.activeInputModule.priority = priority;

            return masterSequence.activeInputModule;
        }

        /// <summary>
        /// Note: Not every module should unlock itself when completed. For example,
        /// to prevent momentum from taking over the moment an autorun interval is complete,
        /// we leave the autorun modules locked until a swipe or other high-priority module
        /// takes over.
        /// </summary>
        /// <param name="inputModuleObject"></param>
        public void UnlockInputModule(GameObject inputModuleObject)
        {
            if (activeInputModule.name == inputModuleObject.name) {
                activeInputModule.name = string.Empty;
                activeInputModule.priority = 0;
            }
        }

        public MasterSequence TriggerSequenceUpdated(Sequence modifiedSequence)
        {
            MasterTimeData sequenceTimeData = this.masterTimeDataList.Find(x => x.sequence == modifiedSequence);

            if (sequenceTimeData != null) {
                _sequenceUpdated.Invoke(this.gameObject, modifiedSequence);
            }

            return this;
        }
        
        public MasterSequence TriggerSequenceBoundaryReached(Sequence modifiedSequence)
        {
            MasterTimeData sequenceTimeData = this.masterTimeDataList.Find(x => x.sequence == modifiedSequence);

            if (sequenceTimeData != null) {
                _sequenceBoundaryReached.Invoke(this.gameObject, modifiedSequence);
            }

            return this;
        }
        
        /// <summary>
        /// We need to refresh elapsed time whenever a child sequence is modified
        /// in order to make sure the scrubber and other navigation modules
        /// can initialize with the correct time data
        /// </summary>
        /// <param name="modifiedSequence"></param>
        /// <returns></returns>
        public MasterSequence RefreshElapsedTime(Sequence modifiedSequence)
        {
            MasterTimeData sequenceTimeData = this.masterTimeDataList.Find(x => x.sequence == modifiedSequence);
            
            if(sequenceTimeData != null) {
                this.elapsedTime = sequenceTimeData.masterTimeStart + modifiedSequence.currentTime;
            }

            return this;
        }

        /// <summary>
        /// We need to know if the MasterSequence is currently active -
        /// the AxisMonitor uses that data to make sure we're not inadvertently
        /// activating / deactivating playable directors and sequences the user
        /// hasn't reached yet
        /// </summary>
        public void RefreshHasActiveSequence()
        {
            for (int i = 0; i < sequenceControllers.Count; i++) {
                if (sequenceControllers[i].sequence.active == true) {
                    this.hasActiveSequence = true;
                    return;
                }
            }
            this.hasActiveSequence = false;
        }

        /// <summary>
        /// For now, modules setting elapsed time directly on the MasterSequence
        /// (scrubber and bookmarker) bypass the normal module registration
        /// and priority comparison, as it's not currently needed; we can revise this
        /// in the future if need be.
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="targetTime"></param>
        [Button(ButtonSizes.Large)]
        public void SetElapsedTime(GameObject caller, double targetTime)
        {
            _elapsedTime = targetTime;
            ApplyElapsedTimeToSequences(caller, targetTime, this.masterTimeDataList);
            rootConfig.sequenceModified.RaiseEvent(this.gameObject);
        }
        
        /// <summary>
        /// Making sure that events and animations are executed properly across timelines
        /// requires an elaborate setup. If we want to jump from point 0 in timeline A to
        /// point 50 in timeline Z for example, we need to make sure we execute all intervening
        /// timelines in the correct order to ensure we arrive at the correct state. Not only that,
        /// we need to make sure that it works both going forward *and* backward.
        ///
        /// This method ensures that we do just that, making sure that we evaluate and
        /// disable playable directors in the correct order to ensure smooth evaluation, namely
        /// via scrubbing and bookmarking, when traversing through a MasterSequence's data. 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="targetTime"></param>
        /// <param name="sequenceData"></param>
        /// <returns></returns>
        private static Sequence ApplyElapsedTimeToSequences(GameObject caller, double targetTime, List<MasterTimeData> sequenceData)
        {
            Sequence targetSequence = null;
            
            for (int targetID = 0; targetID < sequenceData.Count; targetID++) {
                
                // Since we're going through the sequences in order
                // from beginning to end, as soon as our target time
                // does not exceed the end time of a sequence, that
                // means we've found the sequence we'll want to target
                if (targetTime < sequenceData[targetID].masterTimeEnd ||
                    Mathf.Approximately((float)targetTime, (float)sequenceData[targetID].masterTimeEnd)) {

                    targetSequence = sequenceData[targetID].sequence;
                    targetSequence.active = true;
                    targetSequence.sequenceController.gameObject.SetActive(true);
                    
                    // If applicable, evaluate all of the preceding sequences,
                    // from first to last, at their last frame
                    if (targetID > 0) {
                        for (int j = 0; j < sequenceData.Count; j++) {
                            if (j < targetID) {
                                sequenceData[j].sequence.sequenceController
                                    .SetSequenceTime(caller, (float)sequenceData[j].sequence.sourcePlayable.duration);
                            }
                        }
                    }

                    // If applicable, evaluate all of the following sequences,
                    // from last to first, at their first frame
                    if (targetID < sequenceData.Count - 1) {
                        for (int j = sequenceData.Count - 1; j >= 0; j--) {
                            if (j > targetID) {
                                sequenceData[j].sequence.sequenceController.SetSequenceTime(caller, 0);
                            }
                        }
                    }

                    // Deactivate all other sequences
                    for (int z = 0; z < sequenceData.Count; z++) {

                        if (z != targetID) {
                            sequenceData[z].sequence.active = false;
                            sequenceData[z].sequence.sequenceController.gameObject.SetActive(false);
                        }
                    }
                    
                    // Finally, update our target sequence
                    if (targetID == 0) {
                        // If our target sequence is the first in the master list,
                        // then the target time is already our local time
                        targetSequence.sequenceController.SetSequenceTime(caller, (float)targetTime);
                    } else {
                        // Otherwise, we need to convert the target time to local time
                        // based on its position in the MasterSequence's sequence list
                        double localTime = targetTime - sequenceData[targetID - 1].masterTimeEnd;
                        targetSequence.sequenceController.SetSequenceTime(caller, (float)localTime);
                    }

                    break;
                }
            }

            return targetSequence;
        }

        /// <summary>
        /// The MasterSequence iterates through its child sequences to convert their local
        /// time into global time, as we need global times to ensure scrubbing, bookmarking,
        /// and axis monitoring can operate consistently across timelines.
        /// </summary>
        /// <param name="sourceSequenceControllers"></param>
        /// <returns></returns>
        private static List<MasterTimeData> GenerateSequenceData(List<SequenceController> sourceSequenceControllers)
        {
            List<MasterTimeData> sequenceData = new List<MasterTimeData>();
            for (int i = 0; i < sourceSequenceControllers.Count; i++)
            {
                double masterTimeStart = 0d;
                double masterTimeEnd = 0d;

                if (i == 0)  {
                    masterTimeEnd = sourceSequenceControllers[i].sequence.sourcePlayable.duration;
                } else
                {
                    masterTimeStart = sequenceData[i - 1].masterTimeEnd;
                    masterTimeEnd = sourceSequenceControllers[i].sequence.sourcePlayable.duration + sequenceData[i - 1].masterTimeEnd;
                }

                MasterTimeData newSequenceData = new MasterTimeData(sourceSequenceControllers[i].sequence, masterTimeStart, masterTimeEnd);
                sequenceData.Add(newSequenceData);
            }

            return sequenceData;
        }

        /// <summary>
        /// Modules need to be able to convert local sequence time to
        /// global time when configuring themselves so they can create the correct
        /// intervals to use when evaluating user input 
        /// </summary>
        /// <param name="masterSequence"></param>
        /// <param name="sourceSequence"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        /// <exception cref="SystemException"></exception>
        public static double LocalToMasterTime(MasterSequence masterSequence, Sequence sourceSequence, double localTime)
        {
            if (masterSequence.masterTimeDataList.Count < 1) {
                masterSequence.Init();
            }
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