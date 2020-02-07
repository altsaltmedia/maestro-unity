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
        TMP_Text textRenderer;

        [SerializeField]
        [TextArea]
        string description;

        void OnEnable()
        {
            StoreTextRenderer();
        }

        public void SetText(float targetValue)
        {
            textRenderer.SetText(targetValue.ToString());
        }

        public void SetText(string targetValue)
        {
            textRenderer.SetText(targetValue);
        }

        public void SetText(StringVariable targetValue)
        {
            textRenderer.SetText(targetValue.value);
        }

        public void SetText(IntVariable targetValue)
        {
            textRenderer.SetText(targetValue.value.ToString());
        }

        public void SetText(FloatVariable targetValue)
        {
            textRenderer.SetText(targetValue.value.ToString());
        }
        
        public void SetText(V2Variable targetValue)
        {
            textRenderer.SetText(targetValue.value.ToString());
        }

        void StoreTextRenderer()
        {
            if(textRenderer == null) {
                textRenderer = gameObject.GetComponent<TMP_Text>();
            }
        }
    }
}
