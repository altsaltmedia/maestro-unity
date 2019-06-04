using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Events;

namespace AltSalt
{
    public class ScrollSnapController : MonoBehaviour
    {
        [SerializeField]
        bool readyForUpdate = false;

        [SerializeField]
        EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;


        [SerializeField]
        [Range(.1f, 1f)]
        float lerpModifier = .1f;

        float lerpValue = 0f;
        float easingModifier = 0f;

        EasingFunction.Function easingFunction;

        [SerializeField]
        RectTransform content;

        [SerializeField]
        ScrollSnapElement[] scrollSnapElements;

        bool isLerping = false;

        [SerializeField]
        int activeElementID;

        [SerializeField]
        float previousPosition;

        void Start()
        {
            easingFunction = EasingFunction.GetEasingFunction(ease);
        }

        public void Activate()
        {
            readyForUpdate = true;
            StartCoroutine(LerpToElement(activeElementID));
        }

        public void Deactivate()
        {
            readyForUpdate = false;
        }

        IEnumerator LerpToElement(int elementID)
        {
            float initialPosition = content.anchoredPosition.x;
            float targetPosition = scrollSnapElements[elementID].rectTransform.anchoredPosition.x * -1f;

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

                if( Mathf.Approximately(Mathf.Abs(content.anchoredPosition.x), Mathf.Abs(targetPosition)) == true) {
                    content.anchoredPosition = new Vector2(targetPosition, content.anchoredPosition.y);
                    LerpToElementCallback();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        void LerpToElementCallback()
        {
            for(int i=0; i<scrollSnapElements.Length; i++) {
                if(i != activeElementID) {
                    scrollSnapElements[i].Deactivate();
                }
            }
            scrollSnapElements[activeElementID].Activate();
            previousPosition = content.anchoredPosition.x;
            lerpValue = 0f;
            isLerping = false;
        }

        public void CallLerpToNextElement()
        {
            activeElementID += 1;
            if (activeElementID >= scrollSnapElements.Length) {
                activeElementID = scrollSnapElements.Length - 1;
            }

            StartCoroutine(LerpToElement(activeElementID));
        }

        public void CallLerpToPreviousElement()
        {
            activeElementID -= 1;
            if (activeElementID < 0) {
                activeElementID = 0;
            }
            StartCoroutine(LerpToElement(activeElementID));
        }

        public void Drag()
        {
            if (activeElementID <= 0 && content.anchoredPosition.x > scrollSnapElements[0].rectTransform.anchoredPosition.x * -1f) {
                content.anchoredPosition = new Vector2(scrollSnapElements[0].rectTransform.anchoredPosition.x * -1f, content.anchoredPosition.y);
            }

            if(activeElementID >= scrollSnapElements.Length - 1 && content.anchoredPosition.x < scrollSnapElements[scrollSnapElements.Length - 1].rectTransform.anchoredPosition.x * -1f) {
                content.anchoredPosition = new Vector2(scrollSnapElements[scrollSnapElements.Length - 1].rectTransform.anchoredPosition.x * -1f, content.anchoredPosition.y);
            }
        }

        public void EndDrag()
        {
            if(isLerping == true) {
                return;
            }

            if (content.anchoredPosition.x > previousPosition) {
                CallLerpToPreviousElement();
            } else if (content.anchoredPosition.x < previousPosition) {
                CallLerpToNextElement();
            }
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
        class ScrollSnapElement
        {
            [Header("Scroll Snap Element")]
            [SerializeField]
            public RectTransform rectTransform;

            [SerializeField]
            public ImageUtils elementIcon;

            [SerializeField]
            public UnityEvent activateEvent;

            [SerializeField]
            public UnityEvent deactivateEvent;

            bool active = false;

            [SerializeField]
            Color iconEnabledColor = new Color(1, 1, 1, 1);

            [SerializeField]
            Color iconDisabledColor = new Color(0, 0, 0, 1);

            public void Activate()
            {
                if(active == false) {
                    active = true;
                    elementIcon.SetColor(iconEnabledColor);
                    activateEvent.Invoke();
                }
            }

            public void Deactivate()
            {
                if (active == true) {
                    active = false;
                    elementIcon.SetColor(iconDisabledColor);
                    deactivateEvent.Invoke();
                }
            }
        }
    }

}