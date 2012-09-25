using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame.Screens
{
    public abstract class Screen
    {
        // References to the game, player, and sadako respectively.
        protected Game1 gameReference;
        protected Player playerReference;
        protected Sadako sadakoReference;

        // References to the different handlers
        protected InventoryHandler inventoryHandler;
        protected LocationHandler locationHandler;
        protected EventHandler eventHandler;

        // References to databases
        protected TileDatabase tileDatabase;
        protected RoomDatabase roomDatabase;

        public virtual void Initialize(Game1 gameReference, Player playerReference, Sadako sadakoReference, InventoryHandler inventoryHandler, LocationHandler locationHandler, EventHandler eventHandler, RoomDatabase roomDatabase, TileDatabase tileDatabase)
        {
            this.gameReference = gameReference;
            this.playerReference = playerReference;
            this.sadakoReference = sadakoReference;
            this.inventoryHandler = inventoryHandler;
            this.locationHandler = locationHandler;
            this.eventHandler = eventHandler;
            this.tileDatabase = tileDatabase;
            this.roomDatabase = roomDatabase;
        }

        public virtual void Update(GameTime gameTime, TouchCollection collection, Vector3 acceleration)
        {
            
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
