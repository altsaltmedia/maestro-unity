using UnityEngine;

namespace AltSalt.Maestro
{
    public class ResponsiveSprite : ResponsiveElement
    {
        protected SpriteRenderer spriteRenderer;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreSprite();
        }

        void StoreSprite()
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