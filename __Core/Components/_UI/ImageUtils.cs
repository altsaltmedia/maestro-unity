using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt
{
    public class ImageUtils : MonoBehaviour
    {
        Image image;

        void Start()
        {
            GetImageComponent();
        }

        void GetImageComponent()
        {
            if(image == null) {
                image = GetComponent<Image>();
            }
        }

        public void SetColor(ColorVariable colorVariable)
        {
            GetImageComponent();
            image.color = colorVariable.Value;
        }

        public void SetColor(Color color)
        {
            GetImageComponent();
            image.color = color;
        }
    }
    
}