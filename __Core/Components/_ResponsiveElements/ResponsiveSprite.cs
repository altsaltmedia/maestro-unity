using UnityEngine;

namespace AltSalt
{
    public class ResponsiveSprite : ResponsiveElement
    {

        bool spriteStored = false;
        protected SpriteRenderer spriteRenderer;

        void StoreSprite()
        {
            if (spriteStored == false) {
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteStored = true;
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreSprite();
        }

    }
}