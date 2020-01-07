using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout 
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class ResponsiveSlicedSpriteSize : ResponsiveSprite
    {
        [Range(0, 1)]
        public float margin = 0f;

        [ValueDropdown("dimensionValues")]
        [SerializeField]
        DimensionType orientation;

        private ValueDropdownList<DimensionType> dimensionValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

#if UNITY_EDITOR

        float internalWidthMarginValue = 0f;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreInternalMarginVal();
        }

        protected override void OnRenderObject()
        {
            base.OnRenderObject();
            if (MarginChanged() == true) {
                ExecuteResponsiveAction();
            }
        }

        bool MarginChanged()
        {
            if (Mathf.Approximately(internalWidthMarginValue, margin) == false) {
                StoreInternalMarginVal();
                return true;
            }
            else {
                return false;
            }
        }

        void StoreInternalMarginVal()
        {
            internalWidthMarginValue = margin;
        }

#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            
            double newDimension = Utils.GetResponsiveWidth(sceneHeight.Value, sceneWidth.Value);

            float dimensionModifier = Utils.GetValueFromDesiredPercent((float)newDimension, margin);

            if (orientation == DimensionType.Vertical) {
                spriteRenderer.size = new Vector2((float)newDimension - dimensionModifier, spriteRenderer.size.y);
            } else {
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, (float)newDimension - dimensionModifier);
            }
        }
    }
}