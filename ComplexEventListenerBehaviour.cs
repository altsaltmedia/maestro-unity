/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
	[ExecuteInEditMode]
    public class ComplexEventListenerBehaviour : MonoBehaviour, ISkipRegistration
	{
		[SerializeField]
		private ComplexEventReference _complexEventReference = new ComplexEventReference();
		
		private ComplexEventReference complexEvent => _complexEventReference;

#if UNITY_EDITOR
			[PropertySpace]
            [Multiline]
            public string DeveloperDescription = "";
#endif
		
		[FormerlySerializedAs("_response")]
		[FormerlySerializedAs("Response")]
		[ValidateInput(nameof(IsPopulated))]
		[SerializeField]
		private ComplexPayloadGenericAction _action;

		private ComplexPayloadGenericAction action => _action;

		[SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        [FormerlySerializedAs("doNotRecord")]
        private bool _doNotRecord;

        public bool doNotRecord => _doNotRecord;

        private void OnEnable()
        {
#if UNITY_EDITOR	        
	        _complexEventReference.PopulateVariable(this, nameof(_complexEventReference));
#endif
	        (complexEvent.GetVariable() as ComplexEvent).RegisterListener(this);
		}
        
		private void OnDisable()
		{
			(complexEvent.GetVariable() as ComplexEvent).UnregisterListener(this);
		}

        public void OnEventRaised(ComplexPayload complexPayload)
		{
            action.Invoke(complexPayload);
		}

        private static bool IsPopulated(ComplexPayloadGenericAction attribute)
		{
			return Utils.IsPopulated(attribute);
		}
	}
	   
}