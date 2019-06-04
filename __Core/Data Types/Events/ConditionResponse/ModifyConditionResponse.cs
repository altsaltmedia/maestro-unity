using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ModifyConditionResponse : ConditionResponse
    {
        [Required]
        [ReadOnly]
        [SerializeField]
        protected ModifySettings modifySettings;

#if UNITY_EDITOR
        protected string activeLayoutName;
        protected string activeTextFamilyName;

        public override void SyncValues()
        {
            GetModifySettings();

            activeLayoutName = "Active layout: " + modifySettings.activeLayout.name;
            activeTextFamilyName = "Active text family: " + modifySettings.activeTextFamily.name;
        }

        void GetModifySettings()
        {
            if (modifySettings == null) {
                modifySettings = Utils.GetModifySettings();
            }
        }
#endif

        // This is placeholder function and is overriden in children - it is not used. //
        public override bool CheckCondition()
        {
            return true;
        }
    }
}