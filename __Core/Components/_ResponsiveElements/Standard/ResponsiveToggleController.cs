using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class ResponsiveToggleController : ResponsiveElement
    {
        [SerializeField]
        List<ToggleTargetList> targetLists = new List<ToggleTargetList>();

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            UpdateTargetList(breakpointIndex);
        }

        void UpdateTargetList(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= targetLists.Count) {
                LogBreakpointWarning();
                return;
            }
#endif
            targetLists[activeIndex].UpdateTargets();
        }

        [Serializable]
        class ToggleTargetList
        {
            [SerializeField]
            public List<ToggleTarget> toggleTargets = new List<ToggleTarget>();

            public void UpdateTargets()
            {
                PopulateToggleTargetObjects();
                for(int i=0; i<toggleTargets.Count; i++) {
                    toggleTargets[i].gameObjectReference.gameObject.SetActive(toggleTargets[i].targetStatus);
                }
            }

            void PopulateToggleTargetObjects()
            {
                // Get list of all elements that we could reference
                ReferableElement[] referableElements = Resources.FindObjectsOfTypeAll<ReferableElement>();

                // Loop through the referable elements and compare to stored ToggleTargets.
                for(int q=0; q<toggleTargets.Count; q++) {
                    int idCounter = 0;
                    for(int i=0; i<referableElements.Length; i++) {
                        if(referableElements[i].GetID() == toggleTargets[q].GetLookupID()) {
                            idCounter++;
                            if (idCounter > 1) {
                                // If this is the first duplicate, log the current game object reference
                                // (otherwise we lose reference to it by nature of how the loop is set up)
                                if(idCounter == 2) {
                                    Debug.LogError("Duplicate ID detected on serialized object reference", toggleTargets[q].gameObjectReference.gameObject);
                                }
                                Debug.LogError("Duplicate ID detected on serialized object reference", referableElements[i].gameObject);
                            }
                            toggleTargets[q].gameObjectReference = referableElements[i];
                        }
                    }
                }
            }
        }


        [Serializable]
        public class ToggleTarget : SerializableElementReference
        {
            [SerializeField]
            public bool targetStatus;
        }
    }

}