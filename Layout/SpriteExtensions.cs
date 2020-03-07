using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt.Maestro.Layout
{
    public class SpriteExtensions : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private SpriteRenderer spriteRenderer
        {
            get
            {
                if (_spriteRenderer == null) {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }

                return _spriteRenderer;
            }
            set => _spriteRenderer = value;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        [Button(ButtonSizes.Large)]
        public void SetColor(ColorVariable colorVariable)
        {
            spriteRenderer.color = colorVariable.value;
        }

        [Button(ButtonSizes.Large)]
        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
    }

}