using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt
{
    public class SpriteUtils : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;

        void Start()
        {
            GetSpriteComponent();
        }

        void GetSpriteComponent()
        {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void SetColor(ColorVariable colorVariable)
        {
            GetSpriteComponent();
            spriteRenderer.color = colorVariable.Value;
        }

        public void SetColor(Color color)
        {
            GetSpriteComponent();
            spriteRenderer.color = color;
        }
    }

}