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
		[FormerlySerializedAs("Event")]
		[Required]
		[SerializeField]
		private ComplexEvent _complexEvent;

		[SerializeField]
		private ComplexEventReference _complexEventReference = new ComplexEventReference();
		
		private ComplexEventReference complexEvent => _complexEventReference;

#if UNITY_EDITOR
			[PropertySpace]
            [Multiline]
            public string DeveloperDescription = "";
#endif
		
		[FormerlySerializedAs("Response")]
		[ValidateInput(nameof(IsPopulated))]
		[SerializeField]
		private ComplexPayloadGenericAction _response;

		private ComplexPayloadGenericAction response => _response;

		[SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        [FormerlySerializedAs("doNotRecord")]
        private bool _doNotRecord;

        public bool doNotRecord => _doNotRecord;

        [SerializeField]
        private bool _migrated;

        private bool migrated
        {
	        get => _migrated;
	        set => _migrated = value;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR	        
	        _complexEventReference.PopulateVariable(this, nameof(_complexEventReference));
	        
	        if (migrated == false) {
		        _complexEventReference.SetVariable(_complexEvent);
		        var serializedObject = new SerializedObject(this);
		        var migratedProperty = serializedObject.FindProperty(nameof(_migrated));
		        migratedProperty.boolValue = true;
		        serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
	        }
#endif	        
			
			(complexEvent.GetVariable() as ComplexEvent).RegisterListener(this);
		}
        
		private void OnDisable()
		{
			(complexEvent.GetVariable() as ComplexEvent).UnregisterListener(this);
		}

        public void OnEventRaised(ComplexPayload complexPayload)
		{
            response.Invoke(complexPayload);
		}

        private static bool IsPopulated(ComplexPayloadGenericAction attribute)
		{
			return Utils.IsPopulated(attribute);
		}
	}
	   
}