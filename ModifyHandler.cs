using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class ModifyHandler : MonoBehaviour
    {
        [Required]
        public ComplexEventManualTrigger textUpdateTrigger;

        [Required]
        public SimpleEventTrigger layoutUpdateTrigger;
        
        public void TriggerModifyText(ComplexPayload complexPayload)
        {
            TextFamily textFamily = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextFamily;
            bool targetStatus = complexPayload.GetBoolValue(DataType.boolType);
            TextCollectionBank textCollectionBank = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextCollectionBank;

            bool triggerLayoutChange;
            
            if (targetStatus == true) {
                ActivateOriginTextFamily(textFamily, this.gameObject, out triggerLayoutChange);
            }
            else {
                DeactivateOriginTextFamily(textFamily, this.gameObject, out triggerLayoutChange);
            }
            
            TriggerTextUpdate(textCollectionBank);
            
            if(triggerLayoutChange == true) {
                TriggerLayoutUpdate();
            }
        }
        public void TriggerModifyLayout(ComplexPayload complexPayload)
        {
            LayoutConfig layoutConfig = complexPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as LayoutConfig;
            bool targetStatus = complexPayload.GetBoolValue(DataType.boolType);

            bool triggerTextChange;
            
            if (targetStatus == true) {
                ActivateOriginLayout(layoutConfig, this.gameObject, out triggerTextChange);
            }
            else {
                DeactivateOriginLayout(layoutConfig, this.gameObject, out triggerTextChange);
            }
            
            TriggerLayoutUpdate();

            if (triggerTextChange == true) {
                TriggerTextUpdate();
            }
        }

        public static TextFamily ActivateOriginTextFamily(TextFamily targetTextFamily, Object callingObject,
            out bool triggerLayoutChange)
        {
            targetTextFamily.SetActive(true);

            for (int i = 0; i < targetTextFamily.textFamiliesToDisable.Count; i++) {
                DeactivateLinkedTextFamily(targetTextFamily.textFamiliesToDisable[i].GetVariable(callingObject), targetTextFamily, callingObject);
            }
                    
            triggerLayoutChange = false;

            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                triggerLayoutChange = true;
                ActivateOriginTextFamilyDependencies(targetTextFamily, callingObject);
            }

            return targetTextFamily;
        }

        private static TextFamily ActivateOriginTextFamilyDependencies(TextFamily sourceTextFamily, Object callingObject)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                LayoutConfig layoutConfig = sourceTextFamily.layoutDependencies[i].GetVariable(callingObject);
                ActivateLinkedLayout(layoutConfig, sourceTextFamily, callingObject);
            }

            return sourceTextFamily;
        }

        private static LayoutConfig ActivateLinkedLayout(LayoutConfig targetLayout, IModifyConfig callingConfig,
            Object callingObject)
        {
            targetLayout.SetActive(true);
            
            for (int i = 0; i < targetLayout.layoutsToDisable.Count; i++) {
                DeactivateLinkedLayout(targetLayout.layoutsToDisable[i].GetVariable(callingObject), callingConfig, callingObject);
            }

            if (targetLayout.hasTextFamilyDependencies == true) {
                ActivateLinkedLayoutDependencies(targetLayout, callingConfig, callingObject);
            }

            return targetLayout;
        }

        private static LayoutConfig ActivateLinkedLayoutDependencies(LayoutConfig sourceLayout,
            IModifyConfig callingConfig, Object callingObject)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i].GetVariable(callingObject);
                if (textFamilyDependency == callingConfig || textFamilyDependency.hasLayoutDependencies == false) continue;
                ActivateLinkedTextFamily(textFamilyDependency, sourceLayout, callingObject);
            }

            return sourceLayout;
        }

        private static TextFamily ActivateLinkedTextFamily(TextFamily targetTextFamily, IModifyConfig callingConfig,
            Object callingObject)
        {
            targetTextFamily.SetActive(true);

            for (int i = 0; i < targetTextFamily.textFamiliesToDisable.Count; i++) {
                DeactivateLinkedTextFamily(targetTextFamily.textFamiliesToDisable[i].GetVariable(callingObject), callingConfig, callingObject);
            }
            
            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                ActivateLinkedTextFamilyDependencies(targetTextFamily, callingConfig, callingObject);
            }

            return targetTextFamily;
        }

        private static TextFamily ActivateLinkedTextFamilyDependencies(TextFamily sourceTextFamily,
            IModifyConfig callingConfig, Object callingObject)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                LayoutConfig layoutDependency = sourceTextFamily.layoutDependencies[i].GetVariable(callingObject);
                if (layoutDependency == callingConfig || layoutDependency.hasTextFamilyDependencies == false) continue;
                ActivateLinkedLayout(layoutDependency, sourceTextFamily, callingObject);
            }

            return sourceTextFamily;
        }

        public static LayoutConfig ActivateOriginLayout(LayoutConfig targetLayout, Object callingObject,
            out bool triggerTextChange)
        {
            targetLayout.SetActive(true);
                    
            for (int i = 0; i < targetLayout.layoutsToDisable.Count; i++) {
                DeactivateLinkedLayout(targetLayout.layoutsToDisable[i].GetVariable(callingObject), targetLayout, callingObject);
            }
                    
            triggerTextChange = false;
                    
            if (targetLayout.hasTextFamilyDependencies == true) {
                triggerTextChange = true;
                ActivateOriginLayoutDependencies(targetLayout, callingObject);
            }

            return targetLayout;
        }
        
        private static LayoutConfig ActivateOriginLayoutDependencies(LayoutConfig sourceLayout,
            Object callingObject)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                TextFamily textFamily = sourceLayout.textFamilyDependencies[i].GetVariable(callingObject);
                if (textFamily.hasLayoutDependencies == false) continue;
                ActivateLinkedTextFamily(textFamily, sourceLayout, callingObject);
            }

            return sourceLayout;
        }
        
        public static TextFamily DeactivateOriginTextFamily(TextFamily targetTextFamily, Object callingObject,
            out bool triggerLayoutChange)
        {
            targetTextFamily.SetActive(false);
            
            triggerLayoutChange = false;

            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                triggerLayoutChange = true;
                DeactivateOriginTextFamilyDependencies(targetTextFamily, callingObject);
            }

            return targetTextFamily;
        }

        private static TextFamily DeactivateOriginTextFamilyDependencies(TextFamily sourceTextFamily,
            Object callingObject)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                
                LayoutConfig layoutConfig = sourceTextFamily.layoutDependencies[i].GetVariable(callingObject);
                if (layoutConfig.hasTextFamilyDependencies == false) continue;

                bool textFamilyDependencyActive = false;
                for (int j = 0; j < layoutConfig.textFamilyDependencies.Count; j++) {
                    if (layoutConfig.textFamilyDependencies[i].GetVariable(callingObject).active == true) {
                        textFamilyDependencyActive = true;
                        break;
                    }
                }
                    
                if (textFamilyDependencyActive == false) {
                    DeactivateLinkedLayout(layoutConfig, sourceTextFamily, callingObject);
                }
            }

            return sourceTextFamily;
        }

        private static LayoutConfig DeactivateLinkedLayout(LayoutConfig targetLayout, IModifyConfig callingConfig,
            Object callingObject)
        {
            targetLayout.SetActive(false);

            if (targetLayout.hasTextFamilyDependencies == true) {
                DeactivateLinkedLayoutDependencies(targetLayout, callingConfig, callingObject);
            }
            
            return targetLayout;
        }
        
        private static LayoutConfig DeactivateLinkedLayoutDependencies(LayoutConfig sourceLayout,
            IModifyConfig callingConfig, Object callingObject)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i].GetVariable(callingObject);
                if (textFamilyDependency == callingConfig || textFamilyDependency.hasLayoutDependencies == false) continue;
                
                bool subdependencyActive = false;
                
                for (int j = 0; j < textFamilyDependency.layoutDependencies.Count; j++) {
                    if (textFamilyDependency.layoutDependencies[i].GetVariable(callingObject).active == true) {
                        subdependencyActive = true;
                        break;
                    }
                }
                    
                if (subdependencyActive == false) {
                    DeactivateLinkedTextFamily(textFamilyDependency, sourceLayout, callingObject);
                }
            }

            return sourceLayout;
        }

        private static TextFamily DeactivateLinkedTextFamily(TextFamily targetTextFamily, IModifyConfig callingConfig,
            Object callingObject)
        {
            targetTextFamily.SetActive(false);

            if(targetTextFamily.hasLayoutDependencies == true) {
                DeactivateLinkedTextFamilyDependencies(targetTextFamily, callingConfig, callingObject);
            }

            return targetTextFamily;
        }

        private static TextFamily DeactivateLinkedTextFamilyDependencies(TextFamily sourceTextFamily,
            IModifyConfig callingConfig, Object callingObject)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                
                LayoutConfig layoutDependency = sourceTextFamily.layoutDependencies[i].GetVariable(callingObject);
                if (layoutDependency == callingConfig || layoutDependency.hasTextFamilyDependencies == false) continue;

                bool subdependencyActive = false;
                for (int j = 0; j < layoutDependency.textFamilyDependencies.Count; j++) {
                    if (layoutDependency.textFamilyDependencies[i].GetVariable(callingObject).active == true) {
                        subdependencyActive = true;
                        break;
                    }
                }
                    
                if (subdependencyActive == false) {
                    DeactivateLinkedLayout(layoutDependency, sourceTextFamily, callingObject);
                }
            }

            return sourceTextFamily;
        }

        public static LayoutConfig DeactivateOriginLayout(LayoutConfig targetLayout, Object callingObject,
            out bool triggerTextChange)
        {
            targetLayout.SetActive(false);
            
            triggerTextChange = false;
            
            if (targetLayout.hasTextFamilyDependencies == true && targetLayout.textFamilyDependencies.Count > 0) {
                triggerTextChange = true;
                DeactivateOriginLayoutDependencies(targetLayout, callingObject);
            }
            return targetLayout;
        }

        private static LayoutConfig DeactivateOriginLayoutDependencies(LayoutConfig sourceLayout,
            Object callingObject)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i].GetVariable(callingObject);
                if (textFamilyDependency.hasLayoutDependencies == false) continue;
                bool subdependencyActive = false;
                
                for (int j = 0; j < textFamilyDependency.layoutDependencies.Count; j++) {
                    if (textFamilyDependency.layoutDependencies[i].GetVariable(callingObject).active == true) {
                        subdependencyActive = true;
                        break;
                    }
                }
                    
                if (subdependencyActive == false) {
                    DeactivateLinkedTextFamily(textFamilyDependency, sourceLayout, callingObject);
                }
            }

            return sourceLayout;
        }

        private void TriggerTextUpdate(TextCollectionBank targetBank = null)
        {
            if(targetBank != null) {
                textUpdateTrigger.RaiseEvent(this.gameObject, targetBank);
            } else {
                textUpdateTrigger.RaiseEvent(this.gameObject);
            }
        }

        private void TriggerLayoutUpdate()
        {
            layoutUpdateTrigger.RaiseEvent(this.gameObject);
        }

        private static bool IsPopulated(List<TextCollectionBank> attribute) {
            return Utils.IsPopulated(attribute);
        }

    }
}