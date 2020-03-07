using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class RelativeElementController : MonoBehaviour, IDynamicLayoutElement
    {
        private RelativeElement[] _unsortedElements;

        private RelativeElement[] unsortedElements
        {
            get => _unsortedElements;
            set => _unsortedElements = value;
        }

        private Dictionary<int, RelativeElement> _elementCollection = new Dictionary<int, RelativeElement>();

        private Dictionary<int, RelativeElement> elementCollection => _elementCollection;
        

        private List<int> _elementKeys = new List<int>();

        private List<int> elementKeys
        {
            get => _elementKeys;
            set => _elementKeys = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private List<RelativeElement> _sortedElements = new List<RelativeElement>();

        private List<RelativeElement> sortedElements => _sortedElements;

        public string elementName => gameObject.name;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _enableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger enableDynamicElement => _enableDynamicElement;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _disableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger disableDynamicElement => _disableDynamicElement;

        public Scene parentScene => gameObject.scene;
        
        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        private int _priority;

        public int priority => _priority;
        
        [SerializeField]
        private bool _logElementOnLayoutUpdate = false;

        public bool logElementOnLayoutUpdate {
            get
            {
                if (_logElementOnLayoutUpdate == true || AppSettings.logGlobalResponsiveElementActions == true) {
                    return true;
                }

                return false;
            }
        }

        private bool _initialized = false;

        public bool initialized
        {
            get => _initialized;
            set => _initialized = value;
        }

        private void Start()
        {
            RefreshElements();
            initialized = true;
        }
        
        private void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(_enableDynamicElement.referenceName) == true) {
                _enableDynamicElement.referenceName = nameof(enableDynamicElement).Capitalize();
            }
            _enableDynamicElement.PopulateVariable(this, nameof(_enableDynamicElement));
            if (string.IsNullOrEmpty(_disableDynamicElement.referenceName) == true) {
                _disableDynamicElement.referenceName = nameof(disableDynamicElement).Capitalize();
            }
            _disableDynamicElement.PopulateVariable(this, nameof(_disableDynamicElement));
#endif
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }

        private void RefreshElements()
        {
            GetUnsortedElements();
            StoreSortedElements();
        }

        private void GetUnsortedElements()
        {
            unsortedElements = GetComponentsInChildren<RelativeElement>();
        }

        private void StoreSortedElements()
        {
            elementCollection.Clear();
            for (int i = 0; i < unsortedElements.Length; i++) {
                elementCollection.Add(unsortedElements[i].gameObject.transform.GetSiblingIndex(), unsortedElements[i]);
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
        public void CallExecuteLayoutUpdate(UnityEngine.Object callingObject)
        {
            if (initialized == false) {
                RefreshElements();
                initialized = true;
            }
            if (logElementOnLayoutUpdate == true || AppSettings.logGlobalResponsiveElementActions == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + elementName, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }
            
            for (int i = 0; i < sortedElements.Count; i++) {
                if (logElementOnLayoutUpdate == true || AppSettings.logGlobalResponsiveElementActions == true) {
                    Debug.Log("Relative object updated: " + sortedElements[i].gameObject.name, sortedElements[i].gameObject);
                }
                sortedElements[i].ExecuteRelativeAction();
            }
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if(ElementsChanged()) {
                StoreSortedElements();
                CallExecuteLayoutUpdate(this.gameObject);
            }
        }

        private bool ElementsChanged()
        {
            GetUnsortedElements();
            if (unsortedElements.Length != elementCollection.Count) {
                return true;
            }

            return false;
        }
        
        private void ResetResponsiveElementData()
        {
            disableDynamicElement.RaiseEvent(this.gameObject, this);
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }
#endif
        
        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}