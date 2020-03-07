using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    public class TextExtensions : MonoBehaviour
    {
        private TMP_Text _textRenderer;

        private TMP_Text textRenderer
        {
            get => _textRenderer;
            set => _textRenderer = value;
        }

#if UNITY_EDITOR        
        [SerializeField]
        [TextArea]
        string description;
#endif
        private void Awake()
        {
            StoreTextRenderer();
        }

        private void StoreTextRenderer()
        {
            if(textRenderer == null) {
                textRenderer = gameObject.GetComponent<TMP_Text>();
            }
        }

        [Button(ButtonSizes.Large)]
        public void SetText(float targetValue)
        {
            textRenderer.SetText(targetValue.ToString());
        }

        [Button(ButtonSizes.Large)]
        public void SetText(string targetValue)
        {
            textRenderer.SetText(targetValue);
        }

        [Button(ButtonSizes.Large)]
        public void SetText(StringVariable targetValue)
        {
            textRenderer.SetText(targetValue.value);
        }

        [Button(ButtonSizes.Large)]
        public void SetText(IntVariable targetValue)
        {
            textRenderer.SetText(targetValue.value.ToString());
        }

        [Button(ButtonSizes.Large)]
        public void SetText(FloatVariable targetValue)
        {
            textRenderer.SetText(targetValue.value.ToString());
        }
        
        [Button(ButtonSizes.Large)]
        public void SetText(V2Variable targetValue)
        {
            textRenderer.SetText(targetValue.value.ToString());
        }

    }
}
