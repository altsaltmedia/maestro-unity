using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Modify Settings")]
    public class ModifySettings : ScriptableObject
    {
        [Required]
        [SerializeField]
        private List<LayoutConfig> _activeLayoutConfigs;

        public List<LayoutConfig> activeLayoutConfigs
        {
            get => _activeLayoutConfigs;
            set => _activeLayoutConfigs = value;
        }

        [Required]
        [SerializeField]
        private List<TextFamily> _activeTextFamilies;

        public List<TextFamily> activeTextFamilies
        {
            get => _activeTextFamilies;
            set => _activeTextFamilies = value;
        }

        [Required]
        [SerializeField]
        [TitleGroup("Defaults")]
        private List<LayoutConfig> _defaultLayoutConfigs;

        public List<LayoutConfig> defaultLayoutConfigs
        {
            get => _defaultLayoutConfigs;
            set => _defaultLayoutConfigs = value;
        }
        
        [Required]
        [SerializeField]
        [TitleGroup("Defaults")]
        private List<TextFamily> _defaultTextFamilies;

        public List<TextFamily> defaultTextFamilies
        {
            get => _defaultTextFamilies;
            set => _defaultTextFamilies = value;
        }
    }
}