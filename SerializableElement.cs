using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class SerializableElement : MonoBehaviour
    {
#if UNITY_EDITOR
        protected string activeLayoutName;
#endif
        [ShowInInspector]
        [ReadOnly]
        [InfoBox("This ID will be appended to the filename where the object's data will be stored.")]
        [SerializeField]
        protected int id;

        [ReadOnly]
        [SerializeField]
        protected string sceneName = "";

        [ReadOnly]
        [SerializeField]
        bool initialized;
        
#if UNITY_EDITOR
        [Title("$"+nameof(activeLayoutName))]
#endif
        [ShowInInspector]
        [InfoBox("Active layout should be set by a ModifyHandler at runtime, or via the LayoutTools or TextTools." +
                 " Do not touch this value unless you know what you're doing.", InfoMessageType.Warning)]
        private bool _allowLayoutDebugging;
        
        [ShowInInspector]
        [EnableIf(nameof(_allowLayoutDebugging))]
        private LayoutConfig _activeLayout;

        protected LayoutConfig activeLayout
        {
            get => _activeLayout;
            set => _activeLayout = value;
        }

        [FormerlySerializedAs("_layouts")]
        [SerializeField]
        [OnValueChanged(nameof(RefreshActiveLayout))]
        private List<LayoutConfig> _layoutsLegacy = new List<LayoutConfig>();

        [SerializeField]
        private List<LayoutConfigReference> _layoutReferences = new List<LayoutConfigReference>();
        
        private List<LayoutConfigReference> layouts => _layoutReferences;

        [SerializeField]
        [ReadOnly]
        protected List<string> nonserializedProperties = new List<string>();

        public int GetID()
        {
            return id;
        }

        [SerializeField]
        private bool _migrated;

        private bool migrated
        {
            get => _migrated;
            set => _migrated = value;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            for (int i = 0; i < _layoutReferences.Count; i++) {
                _layoutReferences[i].PopulateVariable(this,
                    new[] {nameof(_layoutReferences), i.ToString()});
            }
            
            Initialize();
            if (migrated == false) {
                migrated = true;
                for (int i = 0; i < _layoutsLegacy.Count; i++) {
                    layouts.Add(new LayoutConfigReference(_layoutsLegacy[i]));
                }
            }
#endif
            RefreshActiveLayout();
        }

        protected void RefreshActiveLayout()
        {
            if (layouts.Count > 0) {
                activeLayout = LayoutConfig.GetActiveLayout(layouts.Select(x => x.GetVariable() as LayoutConfig).ToList());
            }
        }

#if UNITY_EDITOR
        protected void Initialize()
        {
            if(initialized == false) {
                SaveSceneName();
                GenerateID();
                initialized = true;
            }
        }

        void SaveSceneName()
        {
            if(sceneName.Length < 1) {
                sceneName = this.gameObject.scene.name;
            }
        }

        void SaveSceneName(bool force)
        {
            if (force == true) {
                sceneName = this.gameObject.scene.name;
            }
        }

        void GenerateID()
        {
            if (id == 0 || initialized == false) {
                id = (int)DateTime.Now.Ticks;
            }
        }

        void GenerateID(bool force)
        {
            if (force == true) {
                id = (int)DateTime.Now.Ticks;
            }
            initialized = true;
        }

        protected virtual void PopulateNonSerializedProperties()
        {
            nonserializedProperties.Clear();
            nonserializedProperties.Add(nameof(id));
            nonserializedProperties.Add(nameof(sceneName));
            nonserializedProperties.Add(nameof(initialized));
            nonserializedProperties.Add(nameof(_layoutsLegacy));
            nonserializedProperties.Add(nameof(nonserializedProperties));
        }

        protected virtual void OnGUI()
        {
            UpdateActiveLayoutName();
        }

        void UpdateActiveLayoutName()
        {
            if (activeLayout != null) {
                activeLayoutName = "Current active layout: " + activeLayout.name;
            }
            else {
                activeLayoutName = "No active layout.";
            }
        }

        [InfoBox("Force reset of the element's scene name and serializable ID.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(10)]
        public void Reinitialize()
        {
            SaveSceneName(true);
            GenerateID(true);
        }
#endif
    }
    
}