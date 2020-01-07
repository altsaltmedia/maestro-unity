using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Text Family")]
    public class TextFamily : RegisterableScriptableObject, IModifyConfig
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
        private List<TextFamily> _textFamiliesToDisable;

        public List<TextFamily> textFamiliesToDisable
        {
            get => _textFamiliesToDisable;
            set => _textFamiliesToDisable = value;
        }
        
        [SerializeField]
        private bool _hasLayoutDependencies;

        public bool hasLayoutDependencies
        {
            get => _hasLayoutDependencies;
            set => _hasLayoutDependencies = value;
        }

        [FormerlySerializedAs("supportedLayouts"),SerializeField]
        [ShowIf(nameof(hasLayoutDependencies))]
        private List<LayoutConfig> _layoutDependencies = new List<LayoutConfig>();

        public List<LayoutConfig> layoutDependencies
        {
            get => _layoutDependencies;
            set => _layoutDependencies = value;
        }

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
        
        public IModifyConfig SetActive(bool targetStatus)
        {
            active = targetStatus;
            return this;
        }

        public static TextFamily GetActiveTextFamily(List<TextFamily> layoutConfigs)
        {
            var convertedConfigs = layoutConfigs.ConvertAll(x => (IModifyConfig) x);
            if (Utils.ContainsActiveModifyConfig(convertedConfigs, out IModifyConfig modifyConfig)) {
                return modifyConfig as TextFamily;
            }
            
            return null;
        }
    }   
}