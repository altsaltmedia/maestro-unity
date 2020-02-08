using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveSprite : ResponsiveLayoutElement
    {
        protected SpriteRenderer spriteRenderer;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreSprite();
        }

        private void StoreSprite()
        {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreSprite();
        }

    }
}