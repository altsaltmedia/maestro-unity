using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ResponsiveAutoWidthHeight))]
    public class ScrollSnapController : DraggableBase, IDragHandler, IEndDragHandler,
        IDynamicLayoutElement, ISceneDimensionListener
    {
        [SerializeField]
        [FormerlySerializedAs("readyForUpdate")]
        private bool _readyForUpdate = false;

        private bool readyForUpdate
        {
            get => _readyForUpdate;
            set => _readyForUpdate = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("content")]
        private RectTransform _content;
        
        public RectTransform content => _content;

        [SerializeField]
        [FormerlySerializedAs("snapUtils")]
        private ScrollSnapUtils _snapUtils;
        
        public ScrollSnapUtils snapUtils => _snapUtils;

        [SerializeField]
        public List<ScrollSnapElement> scrollSnapElements = new List<ScrollSnapElement>();

        [SerializeField]
        private EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;

        private EasingFunction.Function _easingFunction;

        private EasingFunction.Function easingFunction
        {
            get
            {
                if (_easingFunction == null) {
                    _easingFunction = EasingFunction.GetEasingFunction(ease);
                }

                return _easingFunction;
            }
            set => _easingFunction = value;
        }

        [SerializeField]
        [Range(.1f, 1f)]
        float lerpModifier = .1f;

        // Utility vars
        bool isLerping = false;
        float baseWidth;
        float maxPosition;
        float lerpValue;
        float easingModifier;

        [ShowInInspector]
        [ReadOnly]
        int activeElementID;

        [ShowInInspector]
        [ReadOnly]
        private float previousPosition;
        
        public string elementName => this.gameObject.name;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _enableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger enableDynamicElement => _enableDynamicElement;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _disableDynamicElement = new ComplexEventManualTrigger();

        public ComplexEventManualTrigger disableDynamicElement => _disableDynamicElement;

        public Scene parentScene => this.gameObject.scene;

        [SerializeField]
        private int _priority;

        public int priority => _priority;

        [SerializeField]
        private bool _logElementOnLayoutUpdate;

        public bool logElementOnLayoutUpdate => _logElementOnLayoutUpdate;
        
        [ShowInInspector]
        [ReadOnly]
        private float _sceneWidth = 4.65f;

        public float sceneWidth
        {
            get => _sceneWidth;
            set => _sceneWidth = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneHeight = 4.594452f;

        public float sceneHeight
        {
            get => _sceneHeight;
            set => _sceneHeight = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private float _sceneAspectRatio = 1.33f;

        public float sceneAspectRatio
        {
            get => _sceneAspectRatio;
            set => _sceneAspectRatio = value;
        }

        public delegate void AddPanelCallback(ScrollSnapElement scrollSnapElement);

#if UNITY_EDITOR
        private void Awake()
        {
            if (string.IsNullOrEmpty(_enableDynamicElement.referenceName) == true) {
                _enableDynamicElement.referenceName = nameof(enableDynamicElement).Capitalize();
            }
            _enableDynamicElement.PopulateVariable(this, nameof(_enableDynamicElement));
            if (string.IsNullOrEmpty(_disableDynamicElement.referenceName) == true) {
                _disableDynamicElement.referenceName = nameof(disableDynamicElement).Capitalize();
            }
            _disableDynamicElement.PopulateVariable(this, nameof(_disableDynamicElement));
        }
#endif

        private void OnEnable()
        {
            enableDynamicElement.RaiseEvent(this.gameObject, this);
        }

        protected override void Start()
        {
            base.Start();
            easingFunction = EasingFunction.GetEasingFunction(ease);
        }
        
        public void CallExecuteLayoutUpdate(Object callingObject)
        {
            baseWidth = (float)Utils.GetResponsiveWidth(sceneHeight, sceneWidth);
            maxPosition = (baseWidth * scrollSnapElements.Count - 1) * -1f;
            readyForUpdate = true;
        }

        public void Activate()
        {
            if (readyForUpdate == false) {
                LogMessage();
                return;
            }
            StartCoroutine(LerpToElement(activeElementID));
        }
        
        public void Deactivate()
        {
            readyForUpdate = false;
        }

        public override void OnDrag(PointerEventData data)
        {
            if(readyForUpdate == false) {
                LogMessage();
                return;
            }

            Vector2 modifier = GetDragModifier(horizontalDrag, verticalDrag, dragSensitivity.GetValue(), data);
            content.anchoredPosition = GetNewPosition(content, modifier);

            if (content.anchoredPosition.x > 0) {
                content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
            }

            if(content.anchoredPosition.x < maxPosition) {
                content.anchoredPosition = new Vector2(maxPosition, content.anchoredPosition.y);
            }
        }

        public override void OnEndDrag(PointerEventData data)
        {
            if (readyForUpdate == false || isLerping == true) {
                LogMessage();
                return;
            }

            if (content.anchoredPosition.x > previousPosition) {
                CallLerpToPreviousElement();
            } else if (content.anchoredPosition.x < previousPosition) {
                CallLerpToNextElement();
            }
        }

        public void CallLerpToNextElement()
        {
            if (readyForUpdate == false) {
                LogMessage();
                return;
            }

            activeElementID += 1;
            if (activeElementID >= scrollSnapElements.Count) {
                activeElementID = scrollSnapElements.Count - 1;
            }

            StartCoroutine(LerpToElement(activeElementID));
        }

        public void CallLerpToPreviousElement()
        {
            if (readyForUpdate == false) {
                LogMessage();
                return;
            }

            activeElementID -= 1;
            if (activeElementID < 0) {
                activeElementID = 0;
            }
            StartCoroutine(LerpToElement(activeElementID));
        }

        private IEnumerator LerpToElement(int elementID)
        {
            float initialPosition = content.anchoredPosition.x;
            float targetPosition = baseWidth * elementID * -1f;

            isLerping = true;
            while (isLerping == true) {

                // Generate a modifier with a little momentum already started
                easingModifier = easingFunction(.2f, 1f, lerpValue);

                // Calculate and set the new position
                float newX = Mathf.Lerp(content.anchoredPosition.x, targetPosition, easingModifier);
                Vector2 newPosition = new Vector2(newX, content.anchoredPosition.y);
                content.anchoredPosition = newPosition;

                // Increment our lerp value for the next loop
                lerpValue += lerpModifier;

                if (Mathf.Approximately(Mathf.Abs(content.anchoredPosition.x), Mathf.Abs(targetPosition)) == true) {
                    content.anchoredPosition = new Vector2(targetPosition, content.anchoredPosition.y);
                    LerpToElementCallback();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void LerpToElementCallback()
        {
            for (int i = 0; i < scrollSnapElements.Count; i++) {
                if (i != activeElementID) {
                    scrollSnapElements[i].Deactivate();
                }
            }
            scrollSnapElements[activeElementID].Activate();
            previousPosition = content.anchoredPosition.x;
            lerpValue = 0f;
            isLerping = false;
        }

        void LogMessage()
        {
            Debug.Log("Scroll snap controller must be activated before utilization", this);
        }

#if UNITY_EDITOR
        //void OnValidate()
        //{
        //    if(content.childCount != scrollSnapElements.Length) {

        //        for(int i=0; i<content.childCount; i++) {
        //            scrollSnapElements[i] = new ScrollSnapElement();
        //            scrollSnapElements[i].rectTransform = content.GetChild(i).GetComponent<RectTransform>();
        //        }

        //    }
        //}
#endif

        [Serializable]
        [ExecuteInEditMode]
        public class ScrollSnapElement
        {
            [Header("Scroll Snap Element")]
            [SerializeField]
            public RectTransform sourceTransform;

            [SerializeField]
            public SpriteExtensions elementIcon;

            [SerializeField]
            public UnityEvent activateEvent;

            [SerializeField]
            public UnityEvent deactivateEvent;

            bool active = false;

            [SerializeField]
            public Color iconEnabledColor = new Color(0, 0, 0, 1);

            [SerializeField]
            public Color iconDisabledColor = new Color(1, 1, 1, 1);

            public ScrollSnapElement(RectTransform rectTransform, SpriteExtensions elementIcon = null)
            {
                this.sourceTransform = rectTransform;
                if (elementIcon != null) {
                    this.elementIcon = elementIcon;
                }
                this.iconEnabledColor = new Color(0, 0, 0, 1);
                this.iconDisabledColor = new Color(1, 1, 1, 1);
            }

            public void Activate()
            {
                if (active == false) {
                    active = true;
                    if (elementIcon != null) {
                        elementIcon.SetColor(iconEnabledColor);
                    }
                    activateEvent.Invoke();
                }
            }

            public void Deactivate()
            {
                if (active == true) {
                    active = false;
                    if (elementIcon != null) {
                        elementIcon.SetColor(iconDisabledColor);
                    }
                    deactivateEvent.Invoke();
                }
            }
        }

        private static bool IsPopulated(RectTransform attribute)
        {
            if(attribute == null) {
                return false;
            }

            return true;
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}