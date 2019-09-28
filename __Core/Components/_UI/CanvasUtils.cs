using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AltSalt
{
    public class CanvasUtils : MonoBehaviour
    {
        public void UpdateCanvases()
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}
