using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    public class LinkHandler : MonoBehaviour
    {
        public void OpenURL(string targetURL)
        {
            Application.OpenURL(targetURL);
        }
    }
}