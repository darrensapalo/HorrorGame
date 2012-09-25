using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame.Screens
{
    public class GameScreen : Screen
    {
        List<ShadowHandler> currentRoomShadows;

        public override void Initialize(Game1 gameReference, Player playerReference, Sadako sadakoReference, InventoryHandler inventoryHandler, LocationHandler locationHandler, EventHandler eventHandler, RoomDatabase roomDatabase, TileDatabase tileDatabase)
        {
            base.Initialize(gameReference, playerReference, sadakoReference, inventoryHandler, locationHandler, eventHandler, roomDatabase, tileDatabase);
        }




        public override void Update(GameTime gameTime, TouchCollection collection, Vector3 acceleration)
        {

            locationHandler.Update(gameTime);
            inventoryHandler.update(gameTime, collection);
            playerReference.update(gameTime, collection);

            if (Game1.SADAKO_ENABLED)
            {
                sadakoReference.update(gameTime);
                sadakoReference.accelerometerShakeHandler(acceleration);
            }

            currentRoomShadows = locationHandler.getCurrentRoom().getShadows();
            foreach (ShadowHandler shadow in currentRoomShadows)
            {
                shadow.update();
            }

            eventHandler.update(gameTime, collection);
            base.Update(gameTime, collection, acceleration);
        }



        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Gray Background
            spriteBatch.Draw(Game1.simpleTexture, new Vector2(0, 0), null, Color.Gray, 0, Vector2.Zero, new Vector2(800, 480F), SpriteEffects.None, 0);
            locationHandler.Draw(spriteBatch, tileDatabase);

            if (Game1.SADAKO_ENABLED)
            {
                sadakoReference.draw(spriteBatch);
            }

            playerReference.draw(spriteBatch);
            
            if (Game1.SHADOW_FROM_FLASHLIGHT_ENABLED)
            {
                if (playerReference.isFlashLightOn)
                {
                    foreach (ShadowHandler shadow in currentRoomShadows)
                    {
                        shadow.draw(spriteBatch, locationHandler.currentRoom.getShadowLight());
                    }
                }
            }
            if (!playerReference.isFlashLightOn)
            {
                spriteBatch.Draw(Game1.simpleTexture, new Vector2(0, 0), null, Color.Black * 0.2F, 0, Vector2.Zero, new Vector2(800, 480F), SpriteEffects.None, 0);
            }
            //Film Draw
            playerReference.filmDraw(spriteBatch);



            Vector2 loc = locationHandler.getCurrentRoom().getLocation();

            // M! renderer for inventory handler
            inventoryHandler.draw(gameTime, spriteBatch);
            
            eventHandler.draw(spriteBatch);

            playerReference.drawController(spriteBatch);
            sadakoReference.drawCapture(spriteBatch);

            // M! i changed the room info to one string, more efficient yoyoyo! :D

            if (false)
            {
                string gameinfo = "";

                gameinfo += string.Format("Room: {0}: {1}\n", loc.X, loc.Y);
                gameinfo += string.Format("PlayerPos: {0},{1}\n", playerReference.position.X, playerReference.position.Y);
                gameinfo += string.Format("Sadako grabbed: {0}", sadakoReference.grabbed);
                spriteBatch.DrawString(Game1.font, gameinfo, new Vector2(10, 10), Color.White);

                if (playerReference.isDead)
                    spriteBatch.DrawString(Game1.font, "You have died.", new Vector2(385, 240), Color.DarkRed);
            }


            base.Draw(gameTime, spriteBatch);
        }

    }
}
