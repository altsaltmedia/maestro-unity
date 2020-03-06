using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Content Extension/Text Family")]
    public class TextFamily : RegisterableScriptableObject, IContentExtensionConfig
    {
        
    #if UNITY_EDITOR
            [SerializeField]
            [Multiline]
            [Header(nameof(TextFamily))]
            private string _description;
    #endif
        
        [SerializeField]
        public SystemLanguage _languageType = SystemLanguage.English;

        public SystemLanguage languageType
        {
            get => _languageType;
            set => _languageType = value;
        }
        
        [SerializeField]
        private List<TextFamilyReference> _textFamiliesToDisable = new List<TextFamilyReference>();

        public List<TextFamilyReference> textFamiliesToDisable => _textFamiliesToDisable;

        [SerializeField]
        private bool _hasLayoutDependencies;

        public bool hasLayoutDependencies
        {
            get => _hasLayoutDependencies;
            set => _hasLayoutDependencies = value;
        }

        [FormerlySerializedAs("supportedLayouts"),SerializeField]
        [ShowIf(nameof(hasLayoutDependencies))]
        private List<LayoutConfigReference> _layoutDependencies = new List<LayoutConfigReference>();

        public List<LayoutConfigReference> layoutDependencies => _layoutDependencies;

        [SerializeField]
        private bool _active;

        public bool active
        {
            get => _active;
            set => _active = value;
        }

        [SerializeField]
        private int _priority;

        public int priority
        {
            get => _priority;
            set => _priority = value;
        }
        
        public IContentExtensionConfig SetActive(bool targetStatus)
        {
            active = targetStatus;
            return this;
        }

        public static TextFamily GetActiveTextFamily(List<TextFamily> layoutConfigs)
        {
            var convertedConfigs = layoutConfigs.ConvertAll(x => (IContentExtensionConfig) x);
            if (Utils.ContainsActiveContentExtensionConfig(convertedConfigs, out IContentExtensionConfig modifyConfig)) {
                return modifyConfig as TextFamily;
            }
            
            return null;
        }
    }   
}