using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
	[Serializable]
    public class AxisReference : ReferenceBase
    {
	    [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private Axis _variable;
        
        public override ScriptableObject GetVariable()
        {
	        base.GetVariable();
	        return _variable;
        }

        public void SetVariable(Axis value)
        {
	        _variable = value;
        }
        
		public bool IsActive()
		{
			return (GetVariable() as Axis).active;
		}

		public Axis SetStatus(GameObject callingObject, bool targetValue)
		{
			Axis axis = (GetVariable() as Axis);
			axis.StoreCaller(callingObject);
			axis.SetStatus(targetValue);
			return axis;
		}
		
		public Axis SetStatus(GameObject callingObject, BoolVariable targetValue)
		{
			Axis axis = (GetVariable() as Axis);
			axis.StoreCaller(callingObject);
			axis.SetStatus(targetValue.value);
			return axis;
		}
		
		public bool IsInverted()
		{
			return (GetVariable() as Axis).inverted;
		}

		public Axis SetInverted(GameObject callingObject, bool targetValue)
		{
			Axis axis = (GetVariable() as Axis);
			axis.StoreCaller(callingObject);
			axis.SetInverted(targetValue);
			return axis;
		}
		
		public Axis SetInverted(GameObject callingObject, BoolVariable targetValue)
		{
			Axis axis = (GetVariable() as Axis);
			axis.StoreCaller(callingObject);
			axis.SetInverted(targetValue.value);
			return axis;
		}
		
#if UNITY_EDITOR
	    protected override bool ShouldPopulateReference()
	    {
		    if (_variable == null) {
			    return true;
		    }

		    return false;
	    }
        
	    protected override ScriptableObject ReadVariable()
	    {
		    return _variable;
	    }
#endif
    }
}