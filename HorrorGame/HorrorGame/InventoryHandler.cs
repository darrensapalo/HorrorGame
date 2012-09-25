using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

namespace HorrorGame
{
    public class InventoryHandler
    {
        private int? touchID    = null;
        public bool isOpen = false;
        public bool itemGetCheck = false;
        public Texture2D backgroundTexture;
        private Vector2 vector = new Vector2(0, 0);
        private Rectangle touchRect = new Rectangle(0, 0, 150,240);
        private Player player;
        private Sadako sadako;
        private Texture2D itemTexture;
        private Texture2D itemGetTexture;

        public Item itemGet;
        private Rectangle rectGet;

        private Item slot1;
        private Item slot2;
        private Item slot3;
        private Item slot4;
        private Item slot5;
        private Item slot6;

        private Vector2 vSlot1;
        private Vector2 vSlot2;
        private Vector2 vSlot3;
        private Vector2 vSlot4;
        private Vector2 vSlot5;
        private Vector2 vSlot6;

        private Rectangle rSlot1;
        private Rectangle rSlot2;
        private Rectangle rSlot3;
        private Rectangle rSlot4;
        private Rectangle rSlot5;
        private Rectangle rSlot6;

        public InventoryHandler()
        {

        }

        public void Initialize(Player player,Sadako sadako, ContentManager Content)
        {
            backgroundTexture = Content.Load<Texture2D>("horror_inventoryScreen");
            itemTexture = Content.Load<Texture2D>("itemlist");
            itemGetTexture = Content.Load<Texture2D>("itemget");
            this.player = player;
            this.sadako = sadako;

            rectGet = new Rectangle(0, 330, 150, 150);
            // debug disabled D!
            //itemGet = new Item(ItemType.HealingSalve, player, sadako);  
            vSlot1 = new Vector2(-250, 75);
            vSlot2 = new Vector2(-250, 75);
            vSlot3 = new Vector2(-250, 75);
            vSlot4 = new Vector2(-250, 220);
            vSlot5 = new Vector2(-250, 220);
            vSlot6 = new Vector2(-250, 220);

            rSlot1 = new Rectangle(-250, 75, 150, 150);
            rSlot2 = new Rectangle(-250, 75, 150, 150);
            rSlot3 = new Rectangle(-250, 75, 150, 150);
            rSlot4 = new Rectangle(-250, 220, 150, 150);
            rSlot5 = new Rectangle(-250, 220, 150, 150);
            rSlot6 = new Rectangle(-250, 220, 150, 150);
        }

        public Boolean addItem(Item itemGet) 
        {
            if(slot1==null)
            {
                slot1 = itemGet;
                return true;
            }
            else if (slot2==null) 
            {
                slot2 = itemGet;
                return true;
            }
            else if (slot3 == null)
            {
                slot3 = itemGet;
                return true;
            }
            else if (slot4 == null)
            {
                slot4 = itemGet;
                return true;
            }
            else if (slot5 == null)
            {
                slot5 = itemGet;
                return true;
            }
            else if (slot6 == null)
            {
                slot6 = itemGet;
                return true;
            }
            return false;
        }

        public void removeItem()
        {
            if (slot1 != null)
            {
                slot1 = null;
            }
            else if (slot2 != null)
            {
                slot2 = null;
            }
            else if (slot3 != null)
            {
                slot3 = null;
            }
            else if (slot4 != null)
            {
                slot4 = null;
            }
            else if (slot5 != null)
            {
                slot5 = null;
            }
            else if (slot6 != null)
            {
                slot6 = null;
            }
        }

        public void update(GameTime gameTime, TouchCollection touchie)
        {
            if (sadako.grabbed) return;

            Point touchPoint = Point.Zero;
            foreach (TouchLocation location in touchie)
            {
                touchPoint.X = (int)location.Position.X;
                touchPoint.Y = (int)location.Position.Y;

                Boolean foundItem = false;
                foreach (Item item in player.location.getCurrentRoom().getItems())
                {

                    if (item.getHitBox().Intersects(player.hitBox) && player.isFlashLightOn)
                    {
                        foundItem = true;
                        player.inventory.itemGet = item;
                        itemGetCheck = true;
                    }
                }
                if (!foundItem)
                    player.inventory.itemGet = null;

                if (touchRect.Contains(touchPoint))
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:

                            touchRect.Width = 250;
                            touchID         = location.Id;

                            break;
                        case TouchLocationState.Released:

                            if (isOpen)
                            {
                                if (vector.X > 400)
                                {
                                    isOpen  = false;
                                }
                            }

                            isOpen          = !isOpen;
                            touchID         = null;
                            break;
                    }

                }
 
                if (location.State == TouchLocationState.Moved && touchID != null && touchID == location.Id)
                {
                    vector.X = touchRect.X = (int)MathHelper.Clamp(touchPoint.X, 0, 570);
                }

                if (isOpen)
                {
                    if (rSlot1.Contains(touchPoint)) { if (slot1 != null)slot1.useItem(); slot1 = null;}
                    if (rSlot2.Contains(touchPoint)) { if (slot2 != null)slot2.useItem(); slot2 = null;}
                    if (rSlot3.Contains(touchPoint)) { if (slot3 != null)slot3.useItem(); slot3 = null;}
                    if (rSlot4.Contains(touchPoint)) { if (slot4 != null)slot4.useItem(); slot4 = null;}
                    if (rSlot5.Contains(touchPoint)) { if (slot5 != null)slot5.useItem(); slot5 = null;}
                    if (rSlot6.Contains(touchPoint)) { if (slot6 != null)slot6.useItem(); slot6 = null;}
                }
                else if (itemGet != null)
                {
                    if (rectGet.Contains(touchPoint))
                    {
                        if (addItem(itemGet))
                        {
                            player.location.getCurrentRoom().removeItem(itemGet);
                            itemGet = null;
                        }
                        
                    }
                }
            }
            
            if (vector.X < 400)
            {
                isOpen = false;
            }

            if (isOpen)
            {
                if (vector.X < 570)
                {
                    touchRect.X+=20;
                    vector.X+=20;
                }
            }
            else
            {
                if (vector.X > 0)
                {
                    touchRect.Width = 150;
                    touchRect.X-=20;
                    vector.X-=20;
                }
            }
            float size = 570;
            vSlot1.X = vector.X + 55F - size;
            vSlot2.X = vector.X + 205F - size;
            vSlot3.X = vector.X + 350F - size;
            vSlot4.X = vector.X + 55F - size;
            vSlot5.X = vector.X + 205F - size;
            vSlot6.X = vector.X + 350F - size;
            rSlot1.X = (int)(vector.X + 55F - size);
            rSlot2.X = (int)(vector.X + 205F - size);
            rSlot3.X = (int)(vector.X + 350F - size);
            rSlot4.X = (int)(vector.X + 55F - size);
            rSlot5.X = (int)(vector.X + 205F - size);
            rSlot6.X = (int)(vector.X + 350F - size);
        }

        public void draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (sadako.grabbed) return;
            if (itemGet != null)
            {
                spriteBatch.Draw(this.itemGetTexture, new Vector2(0, 280), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(itemTexture, new Vector2(0, 330), new Rectangle((int)itemGet.type * 150, 0, 150, 150), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(this.backgroundTexture, vector, null, Color.White, 0, new Vector2(570, 0), 1, SpriteEffects.None, 0);
            if (vSlot1.X >= -150 && slot1 != null)spriteBatch.Draw(itemTexture, vSlot1, new Rectangle((int)slot1.type * 150,0,150,150),Color.White,0, Vector2.Zero, Vector2.One, SpriteEffects.None,0);
            if (vSlot2.X >= -150 && slot2 != null) spriteBatch.Draw(itemTexture, vSlot2, new Rectangle((int)slot2.type * 150, 0, 150, 150), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            if (vSlot3.X >= -150 && slot3 != null) spriteBatch.Draw(itemTexture, vSlot3, new Rectangle((int)slot3.type * 150, 0, 150, 150), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            if (vSlot4.X >= -150 && slot4 != null) spriteBatch.Draw(itemTexture, vSlot4, new Rectangle((int)slot4.type * 150, 0, 150, 150), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            if (vSlot5.X >= -150 && slot5 != null) spriteBatch.Draw(itemTexture, vSlot5, new Rectangle((int)slot5.type * 150, 0, 150, 150), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            if (vSlot6.X >= -150 && slot6 != null) spriteBatch.Draw(itemTexture, vSlot6, new Rectangle((int)slot6.type * 150, 0, 150, 150), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

    }
}
