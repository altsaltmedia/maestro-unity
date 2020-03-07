using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("orientation")]
        private DimensionType _orientation;

        private DimensionType orientation => _orientation;

#if UNITY_EDITOR

        float internalWidthMarginValue = 0f;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreInternalMarginVal();
        }

        protected void OnRenderObject()
        {
            if (MarginChanged() == true) {
                ExecuteResponsiveAction();
            }
        }

        private bool MarginChanged()
        {
            if (Mathf.Approximately(internalWidthMarginValue, margin) == false) {
                StoreInternalMarginVal();
                return true;
            }

            return false;
        }

        private void StoreInternalMarginVal()
        {
            internalWidthMarginValue = margin;
        }

#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            
            double newDimension = Utils.GetResponsiveWidth(sceneHeight, sceneWidth);

            float dimensionModifier = Utils.GetValueFromDesiredPercent((float)newDimension, margin);

            if (orientation == DimensionType.Vertical) {
                spriteRenderer.size = new Vector2((float)newDimension - dimensionModifier, spriteRenderer.size.y);
            } else {
                spriteRenderer.size = new Vector2(spriteRenderer.size.x, (float)newDimension - dimensionModifier);
            }
        }
    }
}