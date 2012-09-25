//#define LIGHTS
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace HorrorGame
{
    public class Player
    {
        
        public PlayerController playerController;
        SongHandler songs;
        Texture2D playerTexture;
        Texture2D lightTexture;
        Texture2D noLightTexture;
        Texture2D filmTexture;

        //Location Handler
        public LocationHandler location;

        //Player Variables
        public float _speed = 0.1F;
        public double degree=0;
        public Vector2 position;
        public Vector2 direction;
        public Rectangle hitBox;
        public Rectangle presenceBox;
        public float life;
        public float maxLife = 10000;
        public Boolean isDead = false;

        //Flashlight Variables
        public Rectangle flashLightHitbox;
        Vector2 flashLightLocation;
        //.982 sweet spot =))
#if LIGHTS
        public float flashLightIntensity = 0.982f; //.982F;
#else
        public float flashLightIntensity = 0.94f; //.94F;
#endif
        int flashlightRange = 150;
        int shadowRange = 120;
        Vector2 shadowLocation;
        public Boolean isFlashLightOn;
        public Rectangle shadowHitBox;


        //filmgrain variables
        Random rand = new Random();
        public float filmIntensity = 1F;
        Vector2 filmLocation;
        float filmDegree;

        //Sadako
        public Sadako sadako;
        public Vector2 currentPosition;

        // Darren variable added 7/11/2012 1:30PM
        public Boolean isHidden;
        public HidingSpot currentHidingSpot;
        public InventoryHandler inventory;

        //Presence Variables
        public float currentPresence = 0;
        public Boolean isMoving = false;
        public float maxPresence = 25000000;

        //Sound Variables
        public float currentSound = 0;
        public float maxSound = 100;
        public float targetSound = 0;


        public Player()
        {
        }

        public void Initialize(ContentManager Content, Vector2 startingPosition, Texture2D simpleTexture, SongHandler songHandler, LocationHandler locationHandler, InventoryHandler inventoryHandler)
        {

            // Load the content
            Texture2D lightTexture = Content.Load<Texture2D>("light");
            Texture2D noLightTexture = Content.Load<Texture2D>("nolight");
            Texture2D filmTexture = Content.Load<Texture2D>("film");
            Texture2D playerTexture = Content.Load<Texture2D>("player");
            Texture2D buttonTexture = Content.Load<Texture2D>("joystickhead");
            
            // TODO: Load the player's textures
            PreparePlayer(simpleTexture, buttonTexture, startingPosition, lightTexture, noLightTexture, filmTexture, songHandler, locationHandler, inventoryHandler);
        }

        public void PreparePlayer(Texture2D playerTextureGet, Texture2D buttonTexture,Vector2 positionGet,Texture2D lightTextureGet,Texture2D noLightTextureGet, Texture2D filmTextureGet, SongHandler songGet, LocationHandler locationHandler, InventoryHandler inventoryHandler) 
        {

            inventory = inventoryHandler;
            location = locationHandler;
            songs = songGet;
            playerTexture = playerTextureGet;
            lightTexture = lightTextureGet;
            noLightTexture = noLightTextureGet;
            filmTexture = filmTextureGet;
            position = positionGet;
            life = maxLife;
            hitBox = new Rectangle(0,0,30,30);
            flashLightHitbox = new Rectangle(0, 0, 150, 150);
            shadowHitBox = new Rectangle(0, 0,150, 150);
            presenceBox = new Rectangle(0, 0, 90, 90);
            Vector2 buttonPosition = new Vector2(480, 160);
            playerController = new PlayerController(buttonTexture,this,location);
            direction = new Vector2(0, 0);
            isFlashLightOn = true;
            songs.playAmbient();
        }

        public void update(GameTime gameTime, TouchCollection locations)
        {
            if (!sadako.grabbed)
                playerController.update(gameTime, location.getCurrentRoom(), locations);

            flashLightHandler();
            shadowHandler();
            filmHandler();

            // DARREN! Put an offset of -15 for the orientation
            hitBox.X = (int)position.X - 15;
            hitBox.Y = (int)position.Y - 15;

            flashLightHitbox.X = (int)flashLightLocation.X;
            flashLightHitbox.Y = (int)flashLightLocation.Y;
            shadowHitBox.X = (int)shadowLocation.X;
            shadowHitBox.Y = (int)shadowLocation.Y;
            presenceBox.X = (int)position.X - 40;
            presenceBox.Y = (int)position.Y - 40;
            filmLocation = new Vector2(400 - rand.Next(-50, 50), 240 - rand.Next(-50, 50));
            filmDegree = MathHelper.ToRadians(rand.Next(-180, 180));
            flashLightIntensity = location.currentRoom.getFlashlight();
            // darren: check if player has left hiding spot
            if (currentHidingSpot != null && !currentHidingSpot.getHitBox().Intersects(hitBox))
            {
                isHidden = false;
                currentHidingSpot.setActivated(false);
            }
            wayPointHandler(gameTime);
        }

        private void wayPointHandler(GameTime gameTime) 
        {
                if (Vector2.Distance(currentPosition, position) > 32)
                {
                    sadako.wayPoints.Add(position);
                    currentPosition = position;
                }
                if (sadako.wayPoints.Count > 10) 
                {
                    sadako.wayPoints.Remove(sadako.wayPoints.First());
                }
        }

        public void draw(SpriteBatch spriteBatch) 
        {
            flashLight(spriteBatch);
            // spriteBatch.Draw(playerTexture, position, new Rectangle(0,0,32,64), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            if (!isHidden)
                spriteBatch.Draw(playerTexture, position, null, Color.White, (float)degree, new Vector2(0.5F,0.5F), new Vector2(30,30), SpriteEffects.None, 0);
            
            if (Game1.DRAW_HITBOX_OF_PLAYER)
            spriteBatch.Draw(playerTexture, hitBox, Color.Red);
        }

        public void drawController(SpriteBatch spriteBatch) 
        {
            playerController.draw(spriteBatch);
        }

        private void filmHandler() 
        {
            filmIntensity = MathHelper.Clamp((1F-(Vector2.Distance(sadako.position, position) / 1000 / 0.5F))*(0.1F/0.9F), 0, 0.1F)+0.06F;
            if (sadako.disable) filmIntensity = 0.06F;
        }

        public void flashLight(SpriteBatch spriteBatch) 
        {
            if (Game1.LIGHTS_ARE_ON)
                flashLightIntensity = 0F;
            if (isFlashLightOn && !isHidden) spriteBatch.Draw(lightTexture, position, new Rectangle(0, 0, 1200, 1200), Color.White * flashLightIntensity, (float)degree, new Vector2(600, 600), new Vector2(2F, 2F), SpriteEffects.None, 0);
            else spriteBatch.Draw(noLightTexture, position, new Rectangle(0, 0, 1200, 1200), Color.White * flashLightIntensity, (float)degree, new Vector2(600, 600), new Vector2(1.5F, 1.5F), SpriteEffects.None, 0);
            
            //Hitbox Test Representation:
            //if(isFlashLightOn) spriteBatch.Draw(playerTexture, shadowLocation, null, Color.Yellow * 0.25F, 0, Vector2.Zero, new Vector2(150, 150), SpriteEffects.None, 0);
            //spriteBatch.Draw(playerTexture, position - new Vector2(45,45), null, Color.Yellow * 0.25F, 0, Vector2.Zero, new Vector2(90, 90), SpriteEffects.None, 0); 
        }

        private void flashLightHandler() 
        {
            flashLightLocation.Y = (float)(flashlightRange * Math.Sin((float)degree - MathHelper.ToRadians(90F)) + position.Y);
            flashLightLocation.X = (float)(flashlightRange * Math.Cos((float)degree - MathHelper.ToRadians(90F)) + position.X);
            flashLightLocation.X -= 75F;
            flashLightLocation.Y -= 75F;
        }

        private void shadowHandler() 
        {
            shadowLocation.Y = (float)(shadowRange * Math.Sin((float)degree - MathHelper.ToRadians(90F)) + position.Y);
            shadowLocation.X = (float)(shadowRange * Math.Cos((float)degree - MathHelper.ToRadians(90F)) + position.X);
            shadowLocation.X -= 75F;
            shadowLocation.Y -= 75F;
        }

        public void filmDraw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(filmTexture, filmLocation, new Rectangle(0, 0, 900, 900), Color.White * filmIntensity, filmDegree, new Vector2(450,450), new Vector2(1.2F,1.2F), SpriteEffects.None, 0);
        }

        public void checkForActivation()
        {

        }
    }
}
