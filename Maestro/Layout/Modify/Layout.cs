using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Layout")]
    public class Layout : RegisterableScriptableObject
    {
        #if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Header("Layout")]
        string description;
        #endif

        [SerializeField]
        public List<TextFamily> supportedTextFamilies = new List<TextFamily>();
    }   
}