using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class ContentExtensionController : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private ComplexEventManualTrigger _textChanged = new ComplexEventManualTrigger();

        private ComplexEventManualTrigger textChanged => _textChanged;

        [Required]
        [SerializeField]
        private SimpleEventTrigger _layoutChanged = new SimpleEventTrigger();

        private SimpleEventTrigger layoutChanged => _layoutChanged;

        public void TriggerTextChange(ComplexPayload complexPayload)
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
            
            RaiseTextChangedEvent(textCollectionBank);
            
            if(triggerLayoutChange == true) {
                RaiseLayoutChangedEvent();
            }
        }
        public void TriggerLayoutChange(ComplexPayload complexPayload)
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
            
            RaiseLayoutChangedEvent();

            if (triggerTextChange == true) {
                RaiseTextChangedEvent();
            }
        }

        public static TextFamily ActivateOriginTextFamily(TextFamily targetTextFamily, Object callingObject,
            out bool triggerLayoutChange)
        {
            targetTextFamily.SetActive(true);

            for (int i = 0; i < targetTextFamily.textFamiliesToDisable.Count; i++) {
                DeactivateLinkedTextFamily(targetTextFamily.textFamiliesToDisable[i].GetVariable() as TextFamily, targetTextFamily, callingObject);
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
                LayoutConfig layoutConfig = sourceTextFamily.layoutDependencies[i].GetVariable() as LayoutConfig;
                ActivateLinkedLayout(layoutConfig, sourceTextFamily, callingObject);
            }

            return sourceTextFamily;
        }

        private static LayoutConfig ActivateLinkedLayout(LayoutConfig targetLayout, IContentExtensionConfig callingConfig,
            Object callingObject)
        {
            targetLayout.SetActive(true);
            
            for (int i = 0; i < targetLayout.layoutsToDisable.Count; i++) {
                DeactivateLinkedLayout(targetLayout.layoutsToDisable[i].GetVariable() as LayoutConfig, callingConfig, callingObject);
            }

            if (targetLayout.hasTextFamilyDependencies == true) {
                ActivateLinkedLayoutDependencies(targetLayout, callingConfig, callingObject);
            }

            return targetLayout;
        }

        private static LayoutConfig ActivateLinkedLayoutDependencies(LayoutConfig sourceLayout,
            IContentExtensionConfig callingConfig, Object callingObject)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i].GetVariable() as TextFamily;
                if (textFamilyDependency == callingConfig || textFamilyDependency.hasLayoutDependencies == false) continue;
                ActivateLinkedTextFamily(textFamilyDependency, sourceLayout, callingObject);
            }

            return sourceLayout;
        }

        private static TextFamily ActivateLinkedTextFamily(TextFamily targetTextFamily, IContentExtensionConfig callingConfig,
            Object callingObject)
        {
            targetTextFamily.SetActive(true);

            for (int i = 0; i < targetTextFamily.textFamiliesToDisable.Count; i++) {
                DeactivateLinkedTextFamily(targetTextFamily.textFamiliesToDisable[i].GetVariable() as TextFamily, callingConfig, callingObject);
            }
            
            if(targetTextFamily.hasLayoutDependencies == true && targetTextFamily.layoutDependencies.Count > 0) {
                ActivateLinkedTextFamilyDependencies(targetTextFamily, callingConfig, callingObject);
            }

            return targetTextFamily;
        }

        private static TextFamily ActivateLinkedTextFamilyDependencies(TextFamily sourceTextFamily,
            IContentExtensionConfig callingConfig, Object callingObject)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                LayoutConfig layoutDependency = sourceTextFamily.layoutDependencies[i].GetVariable() as LayoutConfig;
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
                DeactivateLinkedLayout(targetLayout.layoutsToDisable[i].GetVariable() as LayoutConfig, targetLayout, callingObject);
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
                TextFamily textFamily = sourceLayout.textFamilyDependencies[i].GetVariable() as TextFamily;
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
                
                LayoutConfig layoutConfig = sourceTextFamily.layoutDependencies[i].GetVariable() as LayoutConfig;
                if (layoutConfig.hasTextFamilyDependencies == false) continue;

                bool textFamilyDependencyActive = false;
                for (int j = 0; j < layoutConfig.textFamilyDependencies.Count; j++) {
                    if ((layoutConfig.textFamilyDependencies[i].GetVariable() as TextFamily).active == true) {
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

        private static LayoutConfig DeactivateLinkedLayout(LayoutConfig targetLayout, IContentExtensionConfig callingConfig,
            Object callingObject)
        {
            targetLayout.SetActive(false);

            if (targetLayout.hasTextFamilyDependencies == true) {
                DeactivateLinkedLayoutDependencies(targetLayout, callingConfig, callingObject);
            }
            
            return targetLayout;
        }
        
        private static LayoutConfig DeactivateLinkedLayoutDependencies(LayoutConfig sourceLayout,
            IContentExtensionConfig callingConfig, Object callingObject)
        {
            for (int i = 0; i < sourceLayout.textFamilyDependencies.Count; i++) {
                
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i].GetVariable() as TextFamily;
                if (textFamilyDependency == callingConfig || textFamilyDependency.hasLayoutDependencies == false) continue;
                
                bool subdependencyActive = false;
                
                for (int j = 0; j < textFamilyDependency.layoutDependencies.Count; j++) {
                    if ((textFamilyDependency.layoutDependencies[i].GetVariable() as LayoutConfig).active == true) {
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

        private static TextFamily DeactivateLinkedTextFamily(TextFamily targetTextFamily, IContentExtensionConfig callingConfig,
            Object callingObject)
        {
            targetTextFamily.SetActive(false);

            if(targetTextFamily.hasLayoutDependencies == true) {
                DeactivateLinkedTextFamilyDependencies(targetTextFamily, callingConfig, callingObject);
            }

            return targetTextFamily;
        }

        private static TextFamily DeactivateLinkedTextFamilyDependencies(TextFamily sourceTextFamily,
            IContentExtensionConfig callingConfig, Object callingObject)
        {
            for(int i=0; i<sourceTextFamily.layoutDependencies.Count; i++) {
                
                LayoutConfig layoutDependency = sourceTextFamily.layoutDependencies[i].GetVariable() as LayoutConfig;
                if (layoutDependency == callingConfig || layoutDependency.hasTextFamilyDependencies == false) continue;

                bool subdependencyActive = false;
                for (int j = 0; j < layoutDependency.textFamilyDependencies.Count; j++) {
                    if ((layoutDependency.textFamilyDependencies[i].GetVariable() as TextFamily).active == true) {
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
                
                TextFamily textFamilyDependency = sourceLayout.textFamilyDependencies[i].GetVariable() as TextFamily;
                if (textFamilyDependency.hasLayoutDependencies == false) continue;
                bool subdependencyActive = false;
                
                for (int j = 0; j < textFamilyDependency.layoutDependencies.Count; j++) {
                    if ((textFamilyDependency.layoutDependencies[i].GetVariable() as LayoutConfig).active == true) {
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

        private void RaiseTextChangedEvent(TextCollectionBank targetBank = null)
        {
            if(targetBank != null) {
                textChanged.RaiseEvent(this.gameObject, targetBank);
            } else {
                textChanged.RaiseEvent(this.gameObject);
            }
        }

        private void RaiseLayoutChangedEvent()
        {
            layoutChanged.RaiseEvent(this.gameObject);
        }

        private static bool IsPopulated(List<TextCollectionBank> attribute) {
            return Utils.IsPopulated(attribute);
        }

    }
}