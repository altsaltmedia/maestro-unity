using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEditor;

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

        [Required]
        [Title("$activeLayoutName")]
        [SerializeField]
        protected ModifySettings modifySettings;

        [SerializeField]
        [ReadOnly]
        protected List<string> nonserializedProperties = new List<string>();

        public int GetID()
        {
            return id;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (modifySettings == null) {
                modifySettings = Utils.GetModifySettings();
            }
            Initialize();
#endif
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

        void OnGUI()
        {
            GetActiveLayoutName();
        }

        void GetActiveLayoutName()
        {
            activeLayoutName = "Current active layout: " + modifySettings._activeLayoutConfig.name;
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