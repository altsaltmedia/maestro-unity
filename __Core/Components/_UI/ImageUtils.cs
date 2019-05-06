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
            image = GetComponent<Image>();
        }

        public void SetColor(ColorVariable colorVariable)
        {
            image.color = colorVariable.Value;
        }
    }
    
}