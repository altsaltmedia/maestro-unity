using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Text Family")]
    public class TextFamily : RegisterableScriptableObject
    {
        [SerializeField]
        [Header("Text Family")]
        public SystemLanguage languageType = SystemLanguage.English;

        [SerializeField]
        [InfoBox("When setting default text family, if there is a conflict, the family with highest priority takes precedence.")]
        public int priority;

        [SerializeField]
        public List<Layout> supportedLayouts = new List<Layout>();
    }   
}