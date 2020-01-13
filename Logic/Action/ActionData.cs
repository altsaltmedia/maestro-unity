using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public abstract class ActionData
    {
        [HideInInspector]
        [SerializeField]
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
        private string _actionDescription = "";

        protected string actionDescription
        {
            get => _actionDescription;
            set => _actionDescription = value;
        }

        public ActionData(int priority)
        {
            this.priority = priority;
        }
        
        public abstract void SyncEditorActionHeadings();

        public abstract void PerformAction(GameObject callingObject);
    }
}