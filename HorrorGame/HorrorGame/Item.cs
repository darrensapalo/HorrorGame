//
#define SHOW_ITEMS_ALWAYS

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public enum ItemType
    {
        HealingSalve = 0,
        Incense = 1,
        Key = 2,
        Amulet = 3,
        MapPiece= 4,
        Lamp = 5,
        Mirror = 6,
    }

    public class Item
    {
        public ItemType type;
        private Player player;
        private Sadako sadako;
        private Rectangle hitBox;
        private static Texture2D sparkTexture;
        private float rotation;
        private float scale;
        private float direction;
        public const int ITEM_SIZE = 50;
        /// <summary>
        /// Duration in seconds.
        /// </summary>
        public const int INCENSE_DURATION = 240;

        public Item(ItemType typeGet, Player playerGet, Sadako sadakoGet, Rectangle hitBoxGet)
        {
            type = typeGet;
            player = playerGet;
            sadako = sadakoGet;
            hitBox = hitBoxGet;
        }

        public static void Initialize(ContentManager Content) { sparkTexture = Content.Load<Texture2D>("spark"); }

        public void useItem()
        {
            switch (type)
            {
                case ItemType.HealingSalve:
                    player.location.transferSadakoRandomly(player.location.getCurrentRoom());

                    break;
                case ItemType.Incense:
                    player.location.getCurrentRoom().roomPresence = -1000 * INCENSE_DURATION;

                    break;
                case ItemType.Amulet:
                    
                    break;
                case ItemType.MapPiece:

                    break;
                case ItemType.Lamp:
                    
                    break;
                case ItemType.Mirror:
                    
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
#if SHOW_ITEMS_ALWAYS
            if ((player.flashLightHitbox.Intersects(hitBox) || player.presenceBox.Intersects(hitBox)) && player.isFlashLightOn)
#endif
            {
                Vector2 position = new Vector2(hitBox.X + ITEM_SIZE / 2, hitBox.Y + ITEM_SIZE / 2);
                rotation += 0.05f;
                scale += direction;
                if (scale > 3) direction = -0.5f;
                if (scale < 2) direction = 0.5f;
                spriteBatch.Draw(sparkTexture, position, null, Color.White, rotation, new Vector2(25f), new Vector2(scale), SpriteEffects.None, 0);
            }
        }

        public Rectangle getHitBox()
        {
            return hitBox;
        }
    }
}
