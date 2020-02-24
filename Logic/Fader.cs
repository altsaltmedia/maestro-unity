using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
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
    [ExecuteInEditMode]
    public class Fader : MonoBehaviour
    {
        [SerializeField]
        private AppSettingsReference _appSettings;

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;

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
        private CustomKeyReference _fadeToBlackSpeedKey = new CustomKeyReference();

        private CustomKey fadeToBlackSpeedKey => _fadeToBlackSpeedKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _fadeInSpeedKey = new CustomKeyReference();

        private CustomKey fadeInSpeedKey => _fadeInSpeedKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _fadeColorKey = new CustomKeyReference();
        
        private CustomKey fadeColorKey => _fadeColorKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private CustomKeyReference _enableSpinnerKey = new CustomKeyReference();

        private CustomKey enableSpinnerKey => _enableSpinnerKey.GetVariable() as CustomKey;
        
        [SerializeField]
        [FormerlySerializedAs("_showProgressBarKey")]
        private CustomKeyReference _enableProgressBarKey = new CustomKeyReference();

        private CustomKey enableProgressBarKey => _enableProgressBarKey.GetVariable() as CustomKey;
        
        [SerializeField]
        private SimpleEventTrigger _triggerShowProgressBar = new SimpleEventTrigger();

        private SimpleEventTrigger triggerShowProgressBar => _triggerShowProgressBar;

        private bool progressBarVisible => appSettings.GetProgressBarVisible(this);
        
        [SerializeField]
        private ComplexEventManualTrigger _triggerHideProgressBar = new ComplexEventManualTrigger();

        private ComplexEventManualTrigger triggerHideProgressBar => _triggerHideProgressBar;

        [SerializeField]
        private CustomKeyReference _eventCallbackKey = new CustomKeyReference();

        private CustomKey eventCallbackKey => _eventCallbackKey.GetVariable() as CustomKey;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _fadeToBlackCompleted = new SimpleEventReference();

        private SimpleEventReference fadeToBlackCompleted => _fadeToBlackCompleted;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _fadeInCompleted = new SimpleEventReference();

        private SimpleEventReference fadeInCompleted => _fadeInCompleted;

        private ComplexPayload _fadeInPayloadCache;

        private ComplexPayload fadeInPayloadCache
        {
            get => _fadeInPayloadCache;
            set => _fadeInPayloadCache = value;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            image = GetComponentInChildren<Image>();

#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            _fadeToBlackSpeedKey.PopulateVariable(this, nameof(_fadeToBlackSpeedKey));
            _fadeInSpeedKey.PopulateVariable(this, nameof(_fadeInSpeedKey));
            _fadeColorKey.PopulateVariable(this, nameof(_fadeColorKey));
            _enableSpinnerKey.PopulateVariable(this, nameof(_enableSpinnerKey));
            _enableProgressBarKey.PopulateVariable(this, nameof(_enableProgressBarKey));
            _triggerShowProgressBar.PopulateVariable(this, nameof(_triggerShowProgressBar));
            _triggerHideProgressBar.PopulateVariable(this, nameof(_triggerHideProgressBar));
            _eventCallbackKey.PopulateVariable(this, nameof(_eventCallbackKey));
            _fadeToBlackCompleted.PopulateVariable(this, nameof(_fadeToBlackCompleted));
            _fadeInCompleted.PopulateVariable(this, nameof(_fadeInCompleted));
#endif            
        }

        public void FadeToBlack(ComplexPayload complexPayload)
        {
            canvasGroup.blocksRaycasts = true;
            
            // Get the fade time
            float fadeTime = complexPayload.GetFloatValue(fadeToBlackSpeedKey);
            if (float.IsNaN(fadeTime) == true) {
                fadeTime = fadeToBlackTime;
            }
            
            image.color = GetFadeColor(complexPayload, fadeColorKey);
            
            // Get utility bools
            bool enableSpinner = complexPayload.GetBoolValue(enableSpinnerKey);
            bool enableProgressBar = complexPayload.GetBoolValue(enableProgressBarKey);

            // Get the callback
            SimpleEvent eventCallback = complexPayload.GetScriptableObjectValue(eventCallbackKey) as SimpleEvent;
            if (eventCallback == null) {
                eventCallback = fadeToBlackCompleted.GetVariable() as SimpleEvent;
            }

            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeTime).SetEase(Ease.Linear).OnComplete(() =>
            {

                if (enableSpinner == true) {
                    Debug.Log("Calling show activity indicator");
                    StartCoroutine(ShowActivityIndicator());
                }

                if (enableProgressBar == true) {
                    triggerShowProgressBar.RaiseEvent(this.gameObject);
                }
                
                if (eventCallback != null) {
                    eventCallback.StoreCaller(this.gameObject);
                    eventCallback.SignalChange();
                }
            });
        }

        public void FadeIn(ComplexPayload complexPayload)
        {
            if (progressBarVisible == true) {
                fadeInPayloadCache = complexPayload;
                triggerHideProgressBar.RaiseEvent(this.gameObject);
            }
            else {
                ExecuteFadeIn(complexPayload);
            }
        }
        
        public void HideProgressBarCompletedCallback()
        {
            ExecuteFadeIn(fadeInPayloadCache);
        }

        private void ExecuteFadeIn(ComplexPayload complexPayload)
        {
            StartCoroutine(HideActivityIndicator());

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
                
                eventCallback.StoreCaller(this.gameObject);
                eventCallback.SignalChange();
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
        
        private IEnumerator ShowActivityIndicator()
        {
            Debug.Log("Showing activity indicator");
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(ActivityIndicatorStyle.WhiteLarge);
            Handheld.StartActivityIndicator();
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
            Handheld.StartActivityIndicator();
#endif
            yield return new WaitForSeconds(0);
        }
        
        private IEnumerator HideActivityIndicator()
        {
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(ActivityIndicatorStyle.DontShow);
            Handheld.StopActivityIndicator();
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.DontShow);
            Handheld.StopActivityIndicator();
#endif
            yield return new WaitForSeconds(0);
        }

        private static bool IsPopulated(SimpleEventReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}