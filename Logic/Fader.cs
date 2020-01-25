using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace AltSalt.Maestro.Logic
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private CanvasGroup canvasGroup
        {
            get => _canvasGroup;
            set => _canvasGroup = value;
        }

        private Image _image;

        private Image image
        {
            get => _image;
            set => _image = value;
        }
        
        [ShowInInspector]
        private float _fadeToBlackTime = 0.4f;

        private float fadeToBlackTime => _fadeToBlackTime;
        
        [ShowInInspector]
        private float _fadeInTime = 0.6f;

        private float fadeInTime => _fadeInTime;
        
        [SerializeField]
        private CustomKeyReference _fadeToBlackSpeedKey;

        private CustomKey fadeTimeBlackSpeedKey => _fadeToBlackSpeedKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _fadeInSpeedKey;

        private CustomKey fadeInSpeedKey => _fadeInSpeedKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _fadeColorKey;
        
        private CustomKey fadeColorKey => _fadeColorKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _enableSpinnerKey;

        private CustomKey enableSpinnerKey => _enableSpinnerKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private SimpleEventTrigger _showProgressBar;

        private SimpleEventTrigger showProgressBar => _showProgressBar;
        
        [SerializeField]
        private SimpleEventTrigger _hideProgressBar;

        private SimpleEventTrigger hideProgressBar => _hideProgressBar;

        [SerializeField]
        private CustomKeyReference _showProgressBarKey;

        private CustomKey showProgressBarKey => _showProgressBarKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _eventCallbackKey;

        private CustomKey eventCallbackKey => _eventCallbackKey.GetVariable() as CustomKey;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _fadeToBlackCompleted;

        private SimpleEventReference fadeToBlackCompleted => _fadeToBlackCompleted;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _fadeInCompleted;

        private SimpleEventReference fadeInCompleted => _fadeInCompleted;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            image = GetComponentInChildren<Image>();
        }

        private void OnEnable()
        {
            _fadeToBlackSpeedKey.PopulateVariable(this, nameof(_fadeToBlackSpeedKey));
            _fadeInSpeedKey.PopulateVariable(this, nameof(_fadeInSpeedKey));
            _fadeColorKey.PopulateVariable(this, nameof(_fadeColorKey));
            _enableSpinnerKey.PopulateVariable(this, nameof(_enableSpinnerKey));
            _eventCallbackKey.PopulateVariable(this, nameof(_eventCallbackKey));
        }

        public void FadeToBlack(ComplexPayload complexPayload)
        {
            canvasGroup.blocksRaycasts = true;
            
            // Get the fade time
            float fadeTime = complexPayload.GetFloatValue(fadeTimeBlackSpeedKey);
            if (float.IsNaN(fadeTime) == true) {
                fadeTime = fadeToBlackTime;
            }
            
            image.color = GetFadeColor(complexPayload, fadeColorKey);
            
            // Get utility bools
            bool enableSpinner = complexPayload.GetBoolValue(enableSpinnerKey);
            bool enableProgressBar = complexPayload.GetBoolValue(showProgressBarKey);

            // Get the callback
            SimpleEvent eventCallback = complexPayload.GetScriptableObjectValue(eventCallbackKey) as SimpleEvent;
            if (eventCallback == null) {
                eventCallback = fadeToBlackCompleted.GetVariable() as SimpleEvent;
            }

            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeTime).OnComplete(() =>
            {
                if (enableSpinner == true) {
                    StartCoroutine(ShowActivityIndicator());
                }

                if (enableProgressBar == true) {
                    showProgressBar.RaiseEvent(this.gameObject);
                }
                
                if (eventCallback != null) {
                    eventCallback.StoreCaller(this.gameObject);
                    eventCallback.SignalChange();
                }
            });
        }

        public void FadeIn(ComplexPayload complexPayload)
        {
            StartCoroutine(HideActivityIndicator());
            hideProgressBar.RaiseEvent(this.gameObject);
            
            // Get the fade time
            float fadeTime = complexPayload.GetFloatValue(fadeInSpeedKey);
            if (float.IsNaN(fadeTime) == true) {
                fadeTime = fadeInTime;
            }

            image.color = GetFadeColor(complexPayload, fadeColorKey);
            
            // Get the callback
            SimpleEvent eventCallback = complexPayload.GetScriptableObjectValue(eventCallbackKey) as SimpleEvent;
            if (eventCallback == null) {
                eventCallback = fadeInCompleted.GetVariable() as SimpleEvent;
            }

            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeTime).OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;

                if (eventCallback != null) {
                    eventCallback.StoreCaller(this.gameObject);
                    eventCallback.SignalChange();
                }
            });
        }
        
        
        private static Color GetFadeColor(ComplexPayload complexPayload, CustomKey colorKey)
        {
            // Get the fade color
            Color fadeColor = Color.black;
            
            ColorVariable fadeColorVariable = complexPayload.GetScriptableObjectValue(colorKey) as ColorVariable;
            if (fadeColorVariable != null) {
                fadeColor = fadeColorVariable.value;
            }

            return fadeColor;
        }
        
        private static IEnumerator ShowActivityIndicator()
        {
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(ActivityIndicatorStyle.Gray);
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif
            Handheld.StartActivityIndicator();
            yield return new WaitForSeconds(0);
        }
        
        private static IEnumerator HideActivityIndicator()
        {
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(ActivityIndicatorStyle.DontShow);
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.DontShow);
#endif
            Handheld.StartActivityIndicator();
            yield return new WaitForSeconds(0);
        }

        private static bool IsPopulated(SimpleEventReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}