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
        RectTransform content;

        [SerializeField]
        ScrollSnapElement[] scrollSnapElements;

        bool isLerping = false;

        [SerializeField]
        int activeElementID;

        [SerializeField]
        float previousPosition;

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
            float targetPosition = scrollSnapElements[elementID].rectTransform.anchoredPosition.x * -1f;

            isLerping = true;
            while (isLerping == true) {
                float newX = Mathf.Lerp(content.anchoredPosition.x, targetPosition, Time.deltaTime * 10f);
                Vector2 newPosition = new Vector2(newX, content.anchoredPosition.y);

                content.anchoredPosition = newPosition;

                if(Mathf.Abs(content.anchoredPosition.x) >= Mathf.Abs(targetPosition) - 20 && Mathf.Abs(content.anchoredPosition.x) <= Mathf.Abs(targetPosition) + 20) {
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