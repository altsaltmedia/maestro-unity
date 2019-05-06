using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Language")]
    public class Language : ScriptableObject
    {
        [SerializeField]
        [Header("Language")]
        public SystemLanguage languageType = SystemLanguage.English;

        [SerializeField]
        [InfoBox("When setting default language, if there is a conflict, the language with highest priority takes precedence.")]
        public int priority;

        [SerializeField]
        public List<Layout> layouts = new List<Layout>();
    }   
}