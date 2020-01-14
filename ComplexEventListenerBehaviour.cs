/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;
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

		private ComplexEvent complexEvent => _complexEvent;

#if UNITY_EDITOR
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
        
        private void OnEnable()
		{
			complexEvent.RegisterListener(this);
		}
        
		private void OnDisable()
		{
			complexEvent.UnregisterListener(this);
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