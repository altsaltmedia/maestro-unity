using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class RelativePage : RelativeElement
    {
        [ValueDropdown("orientationValues")]
        [SerializeField]
        DimensionType orientation;

        private ValueDropdownList<DimensionType> orientationValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

        public override void ExecuteRelativeAction()
        {
            // Set this object's position relative to the reference object
            if(referenceObject != null) {
                float modifier;
                if(orientation == DimensionType.Vertical) {
                    modifier = isNegative ? rectTransform.rect.width * -1f : rectTransform.rect.width;
                    rectTransform.anchoredPosition3D = new Vector3(referenceObject.anchoredPosition3D.x + modifier, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
                } else {
                    modifier = isNegative ? rectTransform.rect.height * -1f : rectTransform.rect.height;
                    rectTransform.anchoredPosition3D = new Vector3(rectTransform.anchoredPosition3D.x, referenceObject.anchoredPosition3D.y + modifier, rectTransform.anchoredPosition3D.z);
                }
            } else {
                Debug.LogException(new Exception("Reference object not set on " + this.name));
            }
        }

    }

}