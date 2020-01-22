using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Layout")]
    public class LayoutConfig : RegisterableScriptableObject, IModifyConfig
    {
        
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Header(nameof(LayoutConfig))]
        private string _description;
#endif
        [SerializeField]
        private List<LayoutConfigReference> _layoutsToDisable;

        public List<LayoutConfigReference> layoutsToDisable => _layoutsToDisable;

        [SerializeField]
        private bool _hasTextFamilyDependencies;

        public bool hasTextFamilyDependencies
        {
            get => _hasTextFamilyDependencies;
            set => _hasTextFamilyDependencies = value;
        }

        [FormerlySerializedAs("supportedTextFamilies"),SerializeField]
        [ShowIf(nameof(hasTextFamilyDependencies))]
        private List<TextFamilyReference> _textFamilyDependencies = new List<TextFamilyReference>();

        public List<TextFamilyReference> textFamilyDependencies => _textFamilyDependencies;

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

        public static LayoutConfig GetActiveLayout(List<LayoutConfig> layoutConfigs)
        {
            var convertedConfigs = layoutConfigs.ConvertAll(x => (IModifyConfig) x);
            if (Utils.ContainsActiveModifyConfig(convertedConfigs, out IModifyConfig modifyConfig)) {
                return modifyConfig as LayoutConfig;;
            }

            return null;
        }
    }   
}