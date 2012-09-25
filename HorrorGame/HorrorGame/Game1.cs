

// Determines whether SADAKO is enabled or not.
//#define SADAKO

// Determines whether shadows will be formed using the flash light.
//#define SHADOWS



/* TODO: 
 *  1) Sometimes, when going through doors, it is locked. Floodfill and spawn a key in one of the dead ends.
 *  2) When Flashlight is off, then shadow handlers should not be created.
 *  3) Fix the starting position of the players.
 *  4) What is 'mapTextures' for?
 *  
 * 
 * Technical:
 *  1) Port to accelerometer. Fix the gyro + acce next time.
 * 
 */
using System;
using System.Collections.Generic;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using HorrorGame.Screens;

namespace HorrorGame
{
    public enum ScreenType
    {
        MainMenuScreen = 0,
        GameScreen,
        PauseScreen, 
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont font;
        
        // Jon Hans! :) :)
        // Databases.
        TileDatabase tileDatabase;
        RoomDatabase roomDatabase;

        // Handlers.
        LocationHandler locationHandler;
        EventHandler eventHandler;
        SongHandler songHandler;
        InventoryHandler inventoryHandler;

        // Screens
        Screen currentScreen;
        GameScreen gameScreen;
        MainMenuScreen mainMenuScreen;
        PauseScreen pauseScreen;

        /// <summary>
        /// Debugging texture. 1x1 pixel.
        /// </summary>
        public static Texture2D simpleTexture;

        // Greg variables! :) :)
        // Main objects.
        Player playerReference;
        Sadako sadakoReference;
        
        // Temporary Shadow for Testing;
        //List<ShadowHandler> currentRoomShadows;

        // Darren variables
        private Map mapDefinition;
        
        // Accelerometer input.
        private Vector3 acceleration = new Vector3();
        private Accelerometer accelSensor;

        // Debug variables
        public const Boolean SADAKO_ENABLED = true;
        public const Boolean SHADOW_FROM_FLASHLIGHT_ENABLED = true;
        public const Boolean LIGHTS_ARE_ON = false;
        public const Boolean DRAW_HITBOX_OF_PLAYER = false;

        public const int mapStartX = 0;
        public const int mapStartY = 0;
        public const int mapSize = 10;

        float loadScreenCounter = 0;
            

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        protected override void Initialize()
        {
            
            // Sample texture, 1x1 pixel for testing.
            simpleTexture = new Texture2D(GraphicsDevice, 1, 1);
            simpleTexture.SetData(new[] { Color.White });

            // Initializes the accelerometer.
            initializeAccelerometer();

            // Databases
            tileDatabase = new TileDatabase();
            roomDatabase = new RoomDatabase();

            // Handlers
            songHandler = new SongHandler();
            inventoryHandler = new InventoryHandler();
            eventHandler = new EventHandler();
            locationHandler = new LocationHandler();

            // Objects
            mapDefinition = new Map();
            sadakoReference = new Sadako();
            playerReference = new Player();

            // M! game profiler for FPS and other related stuff
#if DEBUG
            Components.Add(new GameProfiler(this, Content));
#endif
            
            base.Initialize();
        }

        private void DatabaseInitialization()
        {
            tileDatabase.Initialize(Content);
            roomDatabase.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Debug font
            font = Content.Load<SpriteFont>("arial");

            // GREG INITIALIZATION DO NOT TOUCH
            // Darren: Sorry, I need to 'touch' these for the cleaning up of screens :( :(
            // September 21, 2012 touched.

            // TODO: What is this mapTextures for?
            List<Texture2D> mapTextures = null;
            Rectangle mapSettings = new Rectangle(mapStartX, mapStartY, mapSize, mapSize);

            
            DatabaseInitialization();
            MapInitialization(mapSettings, mapTextures);
            HandlerInitializations();
            ObjectInitializations();
            InitalizeScreens();
        }

        private void HandlerInitializations()
        {
            inventoryHandler.Initialize(playerReference, sadakoReference, Content);
            eventHandler.Initialize(Content, simpleTexture, sadakoReference, playerReference);
            locationHandler.Initialize(playerReference, sadakoReference, mapDefinition, songHandler, eventHandler);
            songHandler.Initialize(Content);
        }

        private void MapInitialization(Rectangle mapSettings, List<Texture2D> mapTextures)
        {
            mapDefinition.Initialize(this, playerReference, Content, mapSettings, simpleTexture, mapTextures, roomDatabase, tileDatabase);
        }

        private void ObjectInitializations()
        {
            
            // Sadako starting position
            Vector2 sadakoStartingPosition = new Vector2(240, -300);
            // Player starting position
            Vector2 startingPosition = new Vector2(400, 180);
            
            
            playerReference.Initialize(Content, startingPosition, simpleTexture, songHandler, locationHandler, inventoryHandler);
            sadakoReference.Initialize(Content, sadakoStartingPosition, songHandler, playerReference, locationHandler);

            Entity.Initialize(Content);
            Item.Initialize(Content);


            // HARD CODE: Sadako hard code to teleport to current room
            sadakoReference.teleport(locationHandler.getCurrentRoom());
        }

        private void InitalizeScreens()
        {
            gameScreen = new GameScreen();
            gameScreen.Initialize(this, playerReference, sadakoReference, inventoryHandler, locationHandler, eventHandler, roomDatabase, tileDatabase);

            mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.Initialize(this, playerReference, sadakoReference, inventoryHandler, locationHandler, eventHandler, roomDatabase, tileDatabase);

            pauseScreen = new PauseScreen();
            pauseScreen.Initialize(this, playerReference, sadakoReference, inventoryHandler, locationHandler, eventHandler, roomDatabase, tileDatabase);
            
            currentScreen = mainMenuScreen;
        }

        public void changeScreen(ScreenType screen)
        {
            switch (screen)
            {
                case ScreenType.GameScreen: currentScreen = gameScreen; break;
                case ScreenType.MainMenuScreen: currentScreen = mainMenuScreen; break;
                case ScreenType.PauseScreen: currentScreen = pauseScreen; break;
            }
        }

        protected override void UnloadContent()
        {

        }

        private void initializeAccelerometer()
        {
            accelSensor = new Accelerometer();
            accelSensor.Start();
        }

        protected void CheckExit()
        {
            changeScreen(ScreenType.PauseScreen);
            // TODO: Ask the user whether he really wishes to quit.
            //this.Exit();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.CheckExit();

            TouchCollection collection = TouchPanel.GetState();

            // M! Just place the accelerometer update here since the game runs at 1 / 60 hertz
            Vector3 v = this.accelSensor.CurrentValue.Acceleration;

            // D! Since landscape left is the orientation, interchange the Y axis and the X axis.
            acceleration.X = -v.Y;
            acceleration.Y = -v.X;
            acceleration.Z = v.Z;

            if (currentScreen == mainMenuScreen)
            {
                loadScreenCounter += gameTime.ElapsedGameTime.Milliseconds;
                if (loadScreenCounter >= 1500)
                {
                    currentScreen = gameScreen;
                }
            }

            currentScreen.Update(gameTime, collection, acceleration);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                currentScreen.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        
    }
}
