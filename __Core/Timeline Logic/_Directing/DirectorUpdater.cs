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

namespace AltSalt
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif

    [System.Serializable]
    public class SequenceGroup
    {
        public Sequence sequence;
        public GameObject directorObject;
        public bool forkOriginActive;
    }

    public class DirectorUpdater : MonoBehaviour
    {
    
        [Required]
        public Sequence sequence;
        [Required]
        public AppSettings appSettings;
        [Required]
        public SimpleEventTrigger boundaryReached;

        [ValidateInput("IsPopulated", "Note: Previous sequence group not populated. Is this intentional?", InfoMessageType.Warning)]
        public SequenceGroup previousSequenceGroup;

        [ValidateInput("IsPopulated", "Note: Next sequence group not populated. Is this intentional?", InfoMessageType.Warning)]
        public SequenceGroup nextSequenceGroup;

        private PlayableDirector playableDirector;

        // Use this for initialization
        void Start()
        {
            GetPlayableDirector();
            //playableDirector.RebuildGraph();
            //playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            //playableDirector.Play();
         //   playableDirector.Pause();
        }

        void GetPlayableDirector()
        {
            playableDirector = gameObject.GetComponent<PlayableDirector>();
        }

        // Update is called once per frame
        public void RefreshSequence()
        {
            if (appSettings.paused == true || sequence.Active == false) {
                return;
            }

            // If the stored clip time is longer or short
            // than the actual clip, we fix that nonsense
            if (sequence.currentTime < playableDirector.initialTime) {
                if (previousSequenceGroup.sequence != null) {
                    sequence.Active = false;
                    if(previousSequenceGroup.forkOriginActive == true) {
                        previousSequenceGroup.sequence.currentTime = previousSequenceGroup.directorObject.GetComponent<PlayableDirector>().playableAsset.duration;
                    } else {
                        previousSequenceGroup.sequence.currentTime = 0.0000f;
                    }
                    previousSequenceGroup.sequence.Active = true;
                    previousSequenceGroup.directorObject.SetActive(true);
                    previousSequenceGroup.directorObject.GetComponent<DirectorUpdater>().ForceEvaluate();
                    gameObject.SetActive(false);
                } else {
                    boundaryReached.RaiseEvent(this.gameObject);
                }
                sequence.currentTime = playableDirector.initialTime;
                return;
            } else if (sequence.currentTime > playableDirector.duration || Mathf.Approximately((float)sequence.currentTime, (float)playableDirector.duration)) {
                sequence.currentTime = playableDirector.duration;

                // Putting this in for now so we can get to the end of the sequence smoothly, should revise for the future
                playableDirector.time = sequence.currentTime;
                playableDirector.Evaluate();
                //////

                if (nextSequenceGroup.sequence != null) {
                    sequence.Active = false;
                    nextSequenceGroup.sequence.currentTime = 0.0000f;
                    nextSequenceGroup.sequence.Active = true;
                    nextSequenceGroup.directorObject.SetActive(true);
                    nextSequenceGroup.directorObject.GetComponent<DirectorUpdater>().ForceEvaluate();
                    gameObject.SetActive(false);
                } else {
                    boundaryReached.RaiseEvent(this.gameObject);
                }
                return;
            }
            // Otherwise, update clip time accordingly
            else {
                playableDirector.time = sequence.currentTime;
            }

            playableDirector.Evaluate();
        }

        public void ForceEvaluate()
        {
            playableDirector.time = sequence.currentTime;
            playableDirector.Evaluate();
        }

        public void SetPreviousSequenceGroup(Sequence newSequence, GameObject newDirectorObject, bool isForkOrigin)
        {
            previousSequenceGroup.sequence = newSequence;
            previousSequenceGroup.directorObject = newDirectorObject;
            previousSequenceGroup.forkOriginActive = isForkOrigin;
        }

        public void SetNextSequenceGroup(Sequence newSequence, GameObject newDirectorObject, bool isForkOrigin)
        {
            nextSequenceGroup.sequence = newSequence;
            nextSequenceGroup.directorObject = newDirectorObject;
            nextSequenceGroup.forkOriginActive = isForkOrigin;
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
            if(playableDirector == null) {
                GetPlayableDirector();
            }

            if (editorListenerCreated == false && appSettings.debugEventsActive.Value == true) {
                simpleEventListener = new SimpleEventListener(screenResized, this.gameObject);
                simpleEventListener.OnTargetEventExecuted += playableDirector.RebuildGraph;
                editorListenerCreated = true;
            }

            if (editorListenerCreated == true && appSettings.debugEventsActive == false) {
                DisableListener();
            }
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
        public static bool IsPopulated(SequenceGroup attribute) {
            if(attribute.sequence == null || attribute.directorObject == null) {
                return false;
            } else {
                return true;
            }
        }

    }

}