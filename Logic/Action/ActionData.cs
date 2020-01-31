using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public abstract class ActionData
    {
        [SerializeField]
        [HideInInspector]
        private int _priority;

        public int priority
        {
            get => _priority;
            set => _priority = value;
        }
        
        protected abstract string title { get; } 

        [TitleGroup("$"+nameof(title))]
        [ShowInInspector]
        [HideLabel]
        [DisplayAsString(false)]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)]
        private string _actionDescription = "";

        public string actionDescription
        {
            get => _actionDescription;
            set => _actionDescription = value;
        }

        public ActionData(int priority)
        {
            this.priority = priority;
        }

#if UNITY_EDITOR
        public virtual void SyncEditorActionHeadings() { }

        public virtual ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            return this;
        }
#endif

        public abstract void PerformAction(GameObject callingObject);
    }
}