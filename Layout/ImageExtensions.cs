using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt.Maestro.Layout
{
    public class ImageExtensions : MonoBehaviour
    {
        private Image _image;

        private Image image
        {
            get => _image;
            set => _image = value;
        }

        private void Awake()
        {
            GetImageComponent();
        }

        private void GetImageComponent()
        {
            if(image == null) {
                image = GetComponent<Image>();
            }
        }

        [Button(ButtonSizes.Large)]
        public void SetColor(ColorVariable colorVariable)
        {
            GetImageComponent();
            image.color = colorVariable.value;
        }

        [Button(ButtonSizes.Large)]
        public void SetColor(Color color)
        {
            GetImageComponent();
            image.color = color;
        }
    }
    
}