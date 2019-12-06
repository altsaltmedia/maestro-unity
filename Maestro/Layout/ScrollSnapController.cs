using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ResponsiveAutoWidthHeight))]
    public class ScrollSnapController : DraggableBase, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        bool readyForUpdate = false;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        RectTransform content;
        public RectTransform Content {
            get {
                return content;
            }
        }

        [SerializeField]
        ScrollSnapUtils snapUtils;
        public ScrollSnapUtils SnapUtils {
            get {
                return snapUtils;
            }
        }

        [SerializeField]
        public List<ScrollSnapElement> scrollSnapElements = new List<ScrollSnapElement>();

        [SerializeField]
        EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;
        EasingFunction.Function easingFunction;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected FloatReference sceneWidth = new FloatReference();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected FloatReference sceneHeight = new FloatReference();

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
        float previousPosition;

        public delegate void AddPanelCallback(ScrollSnapElement scrollSnapElement);


        protected override void Start()
        {
            base.Start();
            easingFunction = EasingFunction.GetEasingFunction(ease);
        }

        public void Activate()
        {
            baseWidth = (float)Utils.GetResponsiveWidth(sceneHeight.Value, sceneWidth.Value);

            StartCoroutine(LerpToElement(activeElementID));

            maxPosition = (baseWidth * scrollSnapElements.Count - 1) * -1f;
            readyForUpdate = true;
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

            Vector2 modifier = GetDragModifier(horizontalDrag, verticalDrag, dragSensitivity.Value, data);
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

        IEnumerator LerpToElement(int elementID)
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

        void LerpToElementCallback()
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

    }

}