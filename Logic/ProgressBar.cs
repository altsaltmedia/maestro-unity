using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt.Maestro.Logic
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private AppSettingsReference _appSettings;

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        private bool progressBarVisible
        {
            set => appSettings.SetProgressBarVisible(this.gameObject, value);
        }

        private float sceneLoadingProgress => appSettings.GetSceneLoadingProgress(this);

        private CanvasGroup _canvasGroup;

        private CanvasGroup canvasGroup
        {
            get => _canvasGroup;
            set => _canvasGroup = value;
        }
        
        private Slider _slider;

        private Slider slider
        {
            get
            {
                if (_slider == null) {
                    _slider = GetComponentInChildren<Slider>();
                }

                return _slider;
            }
            set => _slider = value;
        }

        [SerializeField]
        private float _fadeTime = 0.6f;

        private float fadeTime => _fadeTime;
        
        [SerializeField]
        private CustomKeyReference _eventCallbackKey = new CustomKeyReference();

        private CustomKey eventCallbackKey => _eventCallbackKey.GetVariable() as CustomKey;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _hideProgressBarCompleted = new SimpleEventReference();

        private SimpleEventReference hideProgressBarCompleted => _hideProgressBarCompleted;

        private void Awake()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            _eventCallbackKey.PopulateVariable(this, nameof(_eventCallbackKey));
            _hideProgressBarCompleted.PopulateVariable(this, nameof(_hideProgressBarCompleted));
#endif
            canvasGroup = GetComponent<CanvasGroup>();
            slider = GetComponentInChildren<Slider>();

            if (Application.isPlaying == true) {
                canvasGroup.alpha = 0;
            }            
        }

        public void ShowProgressBar()
        {
            progressBarVisible = true;
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeTime).SetId(this.gameObject.GetInstanceID());
        }
        
        public void HideProgressBar(ComplexPayload complexPayload)
        {
            SimpleEvent eventCallback = complexPayload.GetScriptableObjectValue(eventCallbackKey) as SimpleEvent;
            if (eventCallback == null) {
                eventCallback = hideProgressBarCompleted.GetVariable() as SimpleEvent;
            }

            DOTween.Complete(this.gameObject.GetInstanceID());
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeTime).OnComplete(() =>
            {
                progressBarVisible = false;
                slider.value = 0;
                
                eventCallback.StoreCaller(this.gameObject);
                eventCallback.SignalChange();
            });
        }

        public void UpdateProgress()
        {
            slider.value = sceneLoadingProgress;
        }
        
        private static bool IsPopulated(SimpleEventReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}