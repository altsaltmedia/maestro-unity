/***********************************************

Copyright © AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / artemio@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public abstract class ActionData : IRegisterActionData
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

        protected ActionData(int priority)
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