/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Sequencing
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(SequenceConfig))]
    [RequireComponent(typeof(ProcessModifySequence))]
    [RequireComponent(typeof(PlayableDirector))]
    public class SyncTimelineToSequence : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        [InfoBox("This value must be set at runtime by a SequenceConfig component.")]
        private Sequence _sequence;

        public Sequence sequence
        {
            get => _sequence;
            set => _sequence = value;
        }

        [Required]
        public AppSettings appSettings;

        [ValidateInput(nameof(IsPopulated))]
        public BoolReference scrubberActive;

        public void RefreshPlayableDirector()
        {
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }

        // Update is called once per frame
        public void RefreshSequence()
        {
//            if (appSettings.paused == true || sequence.active == false) {
//                return;
//            }
//
//            // If the stored clip time is longer or short
//            // than the actual clip, we fix that nonsense
//            if (sequence.currentTime < playableDirector.initialTime) {
//                if (previousSequenceGroup.sequence != null) {
//                    if (previousSequenceGroup.forkOriginActive == true) {
//                        previousSequenceGroup.sequence.currentTime = previousSequenceGroup.directorObject.GetComponent<PlayableDirector>().playableAsset.duration;
//                    } else {
//                        previousSequenceGroup.sequence.currentTime = 0.0000f;
//                    }
//                    previousSequenceGroup.sequence.active = true;
//                    previousSequenceGroup.directorObject.gameObject.SetActive(true);
//                    previousSequenceGroup.directorObject.GetComponent<SyncTimelineToSequence>().ForceEvaluate();
//                    this.gameObject.SetActive(false);
//                    sequence.active = false;
//                } else {
//                    boundaryReached.RaiseEvent(this.gameObject);
//                }
//                sequence.currentTime = playableDirector.initialTime;
//                return;
//            } else if (sequence.currentTime > playableDirector.duration || Mathf.Approximately((float)sequence.currentTime, (float)playableDirector.duration)) {
//                sequence.currentTime = playableDirector.duration;
//
//                // Putting this in for now so we can get to the end of the sequence smoothly, should revise for the future
//                playableDirector.time = sequence.currentTime;
//                playableDirector.Evaluate();
//                //////
//
//                if (nextSequenceGroup.sequence != null) {
//                    nextSequenceGroup.sequence.currentTime = 0.0000f;
//                    nextSequenceGroup.sequence.active = true;
//                    nextSequenceGroup.directorObject.gameObject.SetActive(true);
//                    nextSequenceGroup.directorObject.GetComponent<SyncTimelineToSequence>().ForceEvaluate();
//                    this.gameObject.SetActive(false);
//                    sequence.active = false;
//                } else {
//                    boundaryReached.RaiseEvent(this.gameObject);
//                }
//                return;
//            }
//            // Otherwise, update clip time accordingly
//            else {
//                playableDirector.time = sequence.currentTime;
//            }
//
//            playableDirector.Evaluate();
        }

        public void ForceEvaluate()
        {
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }

        public void SetToBeginning()
        {
            sequence.currentTime = 0;
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }

        public void SetToEnd()
        {
            sequence.currentTime = sequence.sourcePlayable.duration;
            sequence.sequenceConfig.playableDirector.time = sequence.currentTime;
            sequence.sequenceConfig.playableDirector.Evaluate();
        }


#if UNITY_EDITOR
        [Required]
        public SimpleEvent screenResized;

        protected SimpleEventListener simpleEventListener;
        protected bool editorListenerCreated = false;

        void Reset()
        {
            string[] guids;
            string path;

            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }

            if (screenResized == null) {
                screenResized = Utils.GetSimpleEvent("ScreenResized");
            }
        }

        void OnEnable()
        {
            editorListenerCreated = false;
        }

        protected virtual void OnRenderObject()
        {
//            if (editorListenerCreated == false && appSettings.debugEventsActive.Value == true) {
//                simpleEventListener = new SimpleEventListener(screenResized, this.gameObject);
//                simpleEventListener.OnTargetEventExecuted += sequence.sequenceConfig.playableDirector.RebuildGraph;
//                editorListenerCreated = true;
//            }
//
//            if (editorListenerCreated == true && appSettings.debugEventsActive == false) {
//                DisableListener();
//            }
//            
        }

        void OnDisable()
        {
            if (editorListenerCreated == true && appSettings.debugEventsActive == true) {
                DisableListener();
            }
        }

        void DisableListener()
        {
            if (simpleEventListener != null) {
                simpleEventListener.DestroyListener();
                simpleEventListener = null;
                editorListenerCreated = false;
            }
        }
#endif

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}