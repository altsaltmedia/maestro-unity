﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class ModifyHandler : MonoBehaviour
    {
        [Required]
        public ComplexEventTrigger textUpdateTrigger;

        [Required]
        public SimpleEventTrigger layoutUpdateTrigger;
        
        public void TriggerModifyText(EventPayload eventPayload)
        {
            TextFamily textFamily = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextFamily;
            bool targetStatus = eventPayload.GetBoolValue(DataType.boolType);
            TextCollectionBank textCollectionBank = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as TextCollectionBank;

            bool triggerLayoutChange;
            
            if (targetStatus == true) {
                ActivateOriginTextFamily(textFamily, out triggerLayoutChange);
            }
            else {
                DeactivateOriginTextFamily(textFamily, out triggerLayoutChange);
            }
            
            TriggerTextUpdate(textCollectionBank);
            
            if(triggerLayoutChange == true) {
                TriggerLayoutUpdate();
            }
        }
        public void TriggerModifyLayout(EventPayload eventPayload)
        {
            LayoutConfig layoutConfig = eventPayload.GetScriptableObjectValue(DataType.scriptableObjectType) as LayoutConfig;
            bool targetStatus = eventPayload.GetBoolValue(DataType.boolType);

            bool triggerTextChange;
            
            if (targetStatus == true) {
                ActivateOriginLayout(layoutConfig, out triggerTextChange);
            }
            else {
                DeactivateOriginLayout(layoutConfig, out triggerTextChange);
            }
            
            TriggerLayoutUpdate();

            if (triggerTextChange == true) {
                TriggerTextUpdate();
            }
        }

        public static TextFamily ActivateOriginTextFamily(TextFamily targetTextFamily, out bool triggerLayoutChange)
        {
            targetTextFamily.SetActive(true);

            for (int i = 0; i < targetTextFamily.textFamiliesToDisable.Count; i++) {
                DeactivateLinkedTextFamily(targetTextFamily.textFamiliesToDisable[i], targetTextFamily);
            }
                    
            triggerLayoutChange = false;

            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                triggerLayoutChange = true;
                ActivateOriginTextFamilyDependencies(targetTextFamily);
            }

            return targetTextFamily;
        }

        private static TextFamily ActivateOriginTextFamilyDependencies(TextFamily sourceTextFamily)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                LayoutConfig layoutConfig = sourceTextFamily.layoutDependencies[i];
                ActivateLinkedLayout(layoutConfig, sourceTextFamily);
            }

            return sourceTextFamily;
        }

        private static LayoutConfig ActivateLinkedLayout(LayoutConfig targetLayout, IModifyConfig callingConfig)
        {
            targetLayout.SetActive(true);
            
            for (int i = 0; i < targetLayout.layoutsToDisable.Count; i++) {
                DeactivateLinkedLayout(targetLayout.layoutsToDisable[i], callingConfig);
            }

            if (targetLayout.hasTextFamilyDependencies == true) {
                ActivateLinkedLayoutDependencies(targetLayout, callingConfig);
            }

            return targetLayout;
        }

        private static LayoutConfig ActivateLinkedLayoutDependencies(LayoutConfig sourceLayout, IModifyConfig callingConfig)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i];
                if (textFamilyDependency == callingConfig || textFamilyDependency.hasLayoutDependencies == false) continue;
                ActivateLinkedTextFamily(textFamilyDependency, sourceLayout);
            }

            return sourceLayout;
        }

        private static TextFamily ActivateLinkedTextFamily(TextFamily targetTextFamily, IModifyConfig callingConfig)
        {
            targetTextFamily.SetActive(true);

            for (int i = 0; i < targetTextFamily.textFamiliesToDisable.Count; i++) {
                DeactivateLinkedTextFamily(targetTextFamily.textFamiliesToDisable[i], callingConfig);
            }
            
            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                ActivateLinkedTextFamilyDependencies(targetTextFamily, callingConfig);
            }

            return targetTextFamily;
        }

        private static TextFamily ActivateLinkedTextFamilyDependencies(TextFamily sourceTextFamily, IModifyConfig callingConfig)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                LayoutConfig layoutDependency = sourceTextFamily.layoutDependencies[i];
                if (layoutDependency == callingConfig || layoutDependency.hasTextFamilyDependencies == false) continue;
                ActivateLinkedLayout(layoutDependency, sourceTextFamily);
            }

            return sourceTextFamily;
        }

        public static LayoutConfig ActivateOriginLayout(LayoutConfig targetLayout, out bool triggerTextChange)
        {
            targetLayout.SetActive(true);
                    
            for (int i = 0; i < targetLayout.layoutsToDisable.Count; i++) {
                DeactivateLinkedLayout(targetLayout.layoutsToDisable[i], targetLayout);
            }
                    
            triggerTextChange = false;
                    
            if (targetLayout.hasTextFamilyDependencies == true) {
                triggerTextChange = true;
                ActivateOriginLayoutDependencies(targetLayout);
            }

            return targetLayout;
        }
        
        private static LayoutConfig ActivateOriginLayoutDependencies(LayoutConfig sourceLayout)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                TextFamily textFamily = sourceLayout.textFamilyDependencies[i];
                if (textFamily.hasLayoutDependencies == false) continue;
                ActivateLinkedTextFamily(textFamily, sourceLayout);
            }

            return sourceLayout;
        }
        
        public static TextFamily DeactivateOriginTextFamily(TextFamily targetTextFamily, out bool triggerLayoutChange)
        {
            targetTextFamily.SetActive(false);
            
            triggerLayoutChange = false;

            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                triggerLayoutChange = true;
                DeactivateOriginTextFamilyDependencies(targetTextFamily);
            }

            return targetTextFamily;
        }

        private static TextFamily DeactivateOriginTextFamilyDependencies(TextFamily sourceTextFamily)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                
                LayoutConfig layoutConfig = sourceTextFamily.layoutDependencies[i];
                if (layoutConfig.hasTextFamilyDependencies == false) continue;

                bool textFamilyDependencyActive = false;
                for (int j = 0; j < layoutConfig.textFamilyDependencies.Count; j++) {
                    if (layoutConfig.textFamilyDependencies[i].active == true) {
                        textFamilyDependencyActive = true;
                        break;
                    }
                }
                    
                if (textFamilyDependencyActive == false) {
                    DeactivateLinkedLayout(layoutConfig, sourceTextFamily);
                }
            }

            return sourceTextFamily;
        }

        private static LayoutConfig DeactivateLinkedLayout(LayoutConfig targetLayout, IModifyConfig callingConfig)
        {
            targetLayout.SetActive(false);

            if (targetLayout.hasTextFamilyDependencies == true) {
                DeactivateLinkedLayoutDependencies(targetLayout, callingConfig);
            }
            
            return targetLayout;
        }
        
        private static LayoutConfig DeactivateLinkedLayoutDependencies(LayoutConfig sourceLayout, IModifyConfig callingConfig)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i];
                if (textFamilyDependency == callingConfig || textFamilyDependency.hasLayoutDependencies == false) continue;
                
                bool subdependencyActive = false;
                
                for (int j = 0; j < textFamilyDependency.layoutDependencies.Count; j++) {
                    if (textFamilyDependency.layoutDependencies[i].active == true) {
                        subdependencyActive = true;
                        break;
                    }
                }
                    
                if (subdependencyActive == false) {
                    DeactivateLinkedTextFamily(textFamilyDependency, sourceLayout);
                }
            }

            return sourceLayout;
        }

        private static TextFamily DeactivateLinkedTextFamily(TextFamily targetTextFamily, IModifyConfig callingConfig)
        {
            targetTextFamily.SetActive(false);

            if(targetTextFamily.hasLayoutDependencies == true) {
                DeactivateLinkedTextFamilyDependencies(targetTextFamily, callingConfig);
            }

            return targetTextFamily;
        }

        private static TextFamily DeactivateLinkedTextFamilyDependencies(TextFamily sourceTextFamily, IModifyConfig callingConfig)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                
                LayoutConfig layoutDependency = sourceTextFamily.layoutDependencies[i];
                if (layoutDependency == callingConfig || layoutDependency.hasTextFamilyDependencies == false) continue;

                bool subdependencyActive = false;
                for (int j = 0; j < layoutDependency.textFamilyDependencies.Count; j++) {
                    if (layoutDependency.textFamilyDependencies[i].active == true) {
                        subdependencyActive = true;
                        break;
                    }
                }
                    
                if (subdependencyActive == false) {
                    DeactivateLinkedLayout(layoutDependency, sourceTextFamily);
                }
            }

            return sourceTextFamily;
        }

        public static LayoutConfig DeactivateOriginLayout(LayoutConfig targetLayout, out bool triggerTextChange)
        {
            targetLayout.SetActive(false);
            
            triggerTextChange = false;
            
            if (targetLayout.hasTextFamilyDependencies == true && targetLayout.textFamilyDependencies.Count > 0) {
                triggerTextChange = true;
                DeactivateOriginLayoutDependencies(targetLayout);
            }
            return targetLayout;
        }

        private static LayoutConfig DeactivateOriginLayoutDependencies(LayoutConfig sourceLayout)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i];
                if (textFamilyDependency.hasLayoutDependencies == false) continue;
                bool subdependencyActive = false;
                
                for (int j = 0; j < textFamilyDependency.layoutDependencies.Count; j++) {
                    if (textFamilyDependency.layoutDependencies[i].active == true) {
                        subdependencyActive = true;
                        break;
                    }
                }
                    
                if (subdependencyActive == false) {
                    DeactivateLinkedTextFamily(textFamilyDependency, sourceLayout);
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