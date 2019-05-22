using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEditor;

namespace AltSalt
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

        bool idGenerated = false;

        [Required]
        [Title("$activeLayoutName")]
        [SerializeField]
        protected ModifySettings modifySettings;

        [SerializeField]
        [ReadOnly]
        protected List<string> nonserializedProperties = new List<string>();

        protected void GenerateID()
        {
            if (id == 0 || idGenerated == false) {
                id = (int)DateTime.Now.Ticks;
            }
            idGenerated = true;
        }

        protected void GenerateID(bool force)
        {
            if (force == true) {
                id = (int)DateTime.Now.Ticks;
            }
            idGenerated = true;
        }

        public int GetID()
        {
            return id;
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            GetActiveLayoutName();
        }

        void GetActiveLayoutName()
        {
            activeLayoutName = "Current active layout: " + modifySettings.activeLayout.name;
        }

        protected virtual void OnRenderObject()
        {
            GenerateID();
        }

        public virtual void Reset()
        {
            if (modifySettings == null) {
                modifySettings = Utils.GetModifySettings();
            }
        }

        [InfoBox("Force reset of the element's serializable ID.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(10)]
        public void ResetID()
        {
            GenerateID(true);
        }
#endif
    }
    
}