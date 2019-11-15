using UnityEngine;
using AltSalt;

namespace AltSalt.Maestro.Layout
{
	[ExecuteInEditMode]
	public class LeftPanelPosition : ResponsiveRectTransform {

	    public float xModifier;

		public override void ExecuteResponsiveAction ()
	    {
	        base.ExecuteResponsiveAction();
	        rectTransform.localPosition = new Vector3(rectTransform.localScale.x * xModifier, rectTransform.localPosition.y, rectTransform.localPosition.z);	
		}
	}
}
