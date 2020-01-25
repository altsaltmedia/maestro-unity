using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt.Maestro.Logic
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ProgressBar : MonoBehaviour
    {
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

        private void OnEnable()
        {
            
            canvasGroup = GetComponent<CanvasGroup>();
            slider = GetComponentInChildren<Slider>();
        }

        public void ShowProgressBar()
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, fadeTime);
        }
        
        public void HideProgressBar()
        {
            DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, fadeTime).OnComplete(() =>
            {
                slider.value = 0;
            });
        }

        public void UpdateProgress(ComplexPayload complexPayload)
        {
            float progress = complexPayload.GetFloatValue();
            if (float.IsNaN(progress) == false) {
                slider.value = progress;
            }
        }
    }
}