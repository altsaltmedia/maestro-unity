using System;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    
    public class AppUtils : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }
        
        [SerializeField]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        private InputGroupKey inputGroupKey => _inputGroupKey.GetVariable() as InputGroupKey;

        private bool appUtilsRequested
        {
            set => appSettings.SetAppUtilsRequested(this.gameObject, inputGroupKey, value);
        }

        [SerializeField]
        private RectTransform _header;

        private RectTransform header => _header;

        [SerializeField]
        private RectTransform _footer;

        private RectTransform footer => _footer;

        [SerializeField]
        private Vector2 _headerHidePosition = new Vector2(0, 130);

        private Vector2 headerHidePosition => _headerHidePosition;

        private Vector2 _headerShowPosition;

        private Vector2 headerShowPosition
        {
            get => _headerShowPosition;
            set => _headerShowPosition = value;
        }
        
        [SerializeField]
        private Vector2 _footerHidePosition = new Vector2(0, -130);

        private Vector2 footerHidePosition => _footerHidePosition;

        
        private Vector2 _footerShowPosition;

        private Vector2 footerShowPosition
        {
            get => _footerShowPosition;
            set => _footerShowPosition = value;
        }

        [SerializeField]
        private float _animationDuration = .2f;

        private float animationDuration => _animationDuration;

        [SerializeField]
        private ComplexEventManualTrigger _triggerUnloadScene;

        private ComplexEventManualTrigger triggerUnloadScene => _triggerUnloadScene;

        [SerializeField]
        private StringReference _appUtilsSceneName;

        private string appUtilsSceneName => _appUtilsSceneName.GetValue();

#if UNITY_EDITOR
        private void OnEnable()
        {
            _inputGroupKey.PopulateVariable(this, nameof(_inputGroupKey));
            _triggerUnloadScene.PopulateVariable(this, nameof(_triggerUnloadScene));
        }
#endif

        private void OnDisable()
        {
            appUtilsRequested = false;
        }

        private void Start()
        {
            headerShowPosition = header.anchoredPosition;
            footerShowPosition = footer.anchoredPosition;

            header.anchoredPosition = headerHidePosition;
            footer.anchoredPosition = footerHidePosition;
            
            ShowHeaderAndFooter();
        }

        private void ShowHeaderAndFooter()
        {
            header.DOAnchorPos(headerShowPosition,
                animationDuration);
            footer.DOAnchorPos(footerShowPosition,
                animationDuration);
        }

        public void HideHeaderAndFooter()
        {
            header.DOAnchorPos(headerHidePosition,
                animationDuration);
            footer.DOAnchorPos(footerHidePosition,
                animationDuration).OnComplete(() =>
            {
                triggerUnloadScene.RaiseEvent(this.gameObject, appUtilsSceneName);
            });
        }

    }
}