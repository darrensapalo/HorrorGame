
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame.Screens
{
    public class MainMenuScreen : Screen
    {
        public override void Initialize(Game1 gameReference, Player playerReference, Sadako sadakoReference, InventoryHandler inventoryHandler, LocationHandler locationHandler, EventHandler eventHandler, RoomDatabase roomDatabase, TileDatabase tileDatabase)
        {
            base.Initialize(gameReference, playerReference, sadakoReference, inventoryHandler, locationHandler, eventHandler, roomDatabase, tileDatabase);
        }

        public override void Update(GameTime gameTime, TouchCollection collection, Vector3 acceleration)
        {
            base.Update(gameTime, collection, acceleration);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
}
