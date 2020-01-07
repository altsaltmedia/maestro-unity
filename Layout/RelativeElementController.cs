using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class RelativeElementController : MonoBehaviour
    {
        RelativeElement[] relativeElements;
        Dictionary<int, RelativeElement> elementCollection = new Dictionary<int, RelativeElement>();

        int elementCount;
        List<int> elementKeys = new List<int>();
        
        [ShowInInspector]
        [ReadOnly]
        List<RelativeElement> sortedElements = new List<RelativeElement>();

        void Start()
        {
            RefreshElements();
        }

        public void RefreshElements()
        {
            GetUnsortedElements();
            StoreSortedElements();
            ExecuteRelativity();
        }

        void GetUnsortedElements()
        {
            relativeElements = GetComponentsInChildren<RelativeElement>();
        }

        void StoreSortedElements()
        {
            elementCollection.Clear();
            for (int i = 0; i < relativeElements.Length; i++) {
                elementCollection.Add(relativeElements[i].gameObject.transform.GetSiblingIndex(), relativeElements[i]);
            }
            elementKeys = elementCollection.Keys.ToList();
            elementKeys.Sort();

            // Clear our list before populating it again
            sortedElements.Clear();

            for (int i = 0; i < elementKeys.Count; i++) {
                sortedElements.Add(elementCollection[elementKeys[i]]);
            }
        }

        [InfoBox("Trigger relative action.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(8)]
        public void ExecuteRelativity()
        {
            for (int i = 0; i < sortedElements.Count; i++) {
                sortedElements[i].ExecuteRelativeAction();
            }
        }

#if UNITY_EDITOR

        void OnGUI()
        {
            if(ElementsChanged()) {
                StoreSortedElements();
                ExecuteRelativity();
            }
        }

        bool ElementsChanged()
        {
            GetUnsortedElements();
            if (relativeElements.Length != elementCount) {
                return true;
            } else {
                return false;
            }
        }
#endif

    }
}