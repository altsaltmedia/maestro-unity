﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public class LayoutConditionResponse : ConditionResponseBase
    {
        [SerializeField]
        [Title("$conditionEventTitle")]
        [Title("Layout Reference")]
        private LayoutConfig _layoutReference;

        private LayoutConfig layoutReference
        {
            get => _layoutReference;
            set => _layoutReference = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _activeLayoutCondition;

        [Title("Layout Status Condition")]
        private BoolReference activeLayoutCondition => _activeLayoutCondition;

        public override void SyncValues()
        {
            if (layoutReference == null) {
                conditionEventTitle = "Please populate a layout as your condition.";
                return;
            }

            conditionEventTitle = "Trigger Condition : Layout " + layoutReference.name + " active is " + activeLayoutCondition.Value;
        }

        public override bool CheckCondition()
        {
            if (layoutReference.active == activeLayoutCondition.Value) {
                return true;
            }

            return false;
        }
        
        public LayoutConfig GetReference()
        {
            return layoutReference;
        }

        public BoolReference GetCondition()
        {
            return activeLayoutCondition;
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}