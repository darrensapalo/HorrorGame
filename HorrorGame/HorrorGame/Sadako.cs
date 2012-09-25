#define GRAB
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
using Microsoft.Devices.Sensors;

namespace HorrorGame
{
    public class Sadako
    {
        public Vector2 position;
        public Vector2 direction;
        public Rectangle hitBox;
        public LocationHandler location;
        SongHandler songs;
        Texture2D texture;
        Player player;
        Boolean chasePlayer = true;
        float runSpeed = 0.10F;
        float lurkSpeed = 0.05F;
        float speed;
        Random rand = new Random();
        public Boolean disturbed = false;
        public Boolean lurking = false;
        public Vector2 lurkHere;
        public int lurkCounter = 8000;
        int chaseCounter = 0;
        Boolean chaseEnded = false;
        public List<Vector2> wayPoints;
        Boolean wallCollide;
        Texture2D grabTexture;
        Vector2 grabOffset;
        Texture2D bloodTexture;
        int grabDuration;
        int frameLocation;
        float bloodIntensity = 0F;
        public Boolean grabbed = false;
        public Boolean isStunned = false;
        public Boolean disable = false;
        int stunCounter = 0;
        int stunDuration = 4000;
        public Boolean disableTurnTransition = false;
        bool moving = false;
        int disturbCounter = 0;
        bool wallCheck = false;
        bool hitTestsThePlayer = false;
        //Controller Variables:
        float z = 0;
        float threshold = 1.2F;
        Boolean isShaked;
        public int shakeCounter = 0;

        //18 is the Sweet Spot
        int releaseThreshold = 18;

        EventHandler events;

        public Sadako() 
        {
            
        }

        public void Initialize(ContentManager Content, Vector2 sadakoStartingPosition, SongHandler songHandler, Player playerReference, LocationHandler locationHandler)
        {
            Texture2D sadakoTexture = Content.Load<Texture2D>("image");
            Texture2D grabTexture = Content.Load<Texture2D>("waifu");
            Texture2D bloodTexture = Content.Load<Texture2D>("blood");
            PrepareSadako(new Vector2(240, -300), sadakoTexture, playerReference, songHandler, grabTexture, bloodTexture, locationHandler);
        }

        public void PrepareSadako(Vector2 positionGet, Texture2D sadakoTexture, Player playerGet, SongHandler songGet, Texture2D captureTexture, Texture2D bloodTextureGet, LocationHandler locationHandler) 
        {
            songs = songGet;
            position = positionGet;
            texture = sadakoTexture;
            player = playerGet;
            speed = lurkSpeed;
            hitBox = new Rectangle(0, 0, 30, 30);
            lurkHere = positionGet;
            direction = new Vector2(0, 0);
            player.sadako = this;
            wayPoints = new List<Vector2>();
            lurking = true;
            grabTexture = captureTexture;
            bloodTexture = bloodTextureGet;
            location = locationHandler;
        }

        public void setEvent(EventHandler eventGet) 
        {
            events = eventGet;
            events.sadako = this;
        }

        public void update(GameTime gameTime) 
        {
            if (!disable)
            {
                if (!isStunned)
                {
                    if (isDisturbed(gameTime) || disturbed) shakeSadako();
                    if (disturbed) chaseHandler(gameTime);
                    if (lurking) lurkHandler(gameTime);
                    hitBox.X = (int)position.X;
                    hitBox.Y = (int)position.Y;

                    wallCheckHandler();
                    stateHandler(gameTime);
                    grabHandler(gameTime);
                    #if GRAB 
                    captureSadako(); 
                    #endif
                }
                else stunHandler(gameTime);
                frameHandler(gameTime);
            }
        }

        private void stunHandler(GameTime gameTime)
        {
            stunCounter += gameTime.ElapsedGameTime.Milliseconds;
            if(stunCounter> stunDuration)
            {
                isStunned = false;
                stunCounter = 0;
            }
        }

        private void grabHandler(GameTime gameTime) 
        {
            bloodIntensity = 1F - player.life / player.maxLife;
            if (grabbed)
            {
                grabDuration += gameTime.ElapsedGameTime.Milliseconds;
                if (isShaked) { shakeCounter++; }
                if (shakeCounter > releaseThreshold)
                {
                    grabbed = false;
                    isStunned = true;
                    player.playerController.reset();
                    return;
                }

                if (grabDuration > player.life/2F)
                {
                    grabDuration = 0;
                    frameLocation += 480;

                    if (frameLocation >= 1440)
                    {
                        frameLocation = 960;
                        player.isDead = true;
                        shakeCounter = 0;
                    }
                }
                grabOffset = new Vector2(rand.Next(-5, 5), rand.Next(-5, 5));
                player.life -= gameTime.ElapsedGameTime.Milliseconds/3F;
            }
        }

        private void startGrab() 
        {
            grabbed = true;
            frameLocation = 0;
            shakeCounter = 0;
            songs.presence.Play();      
        }

        public void accelerometerShakeHandler(Vector3 detect) 
        {
            if (Math.Abs(detect.Z - z) > threshold) { isShaked = true; }
            else isShaked = false;
            z = detect.Z;
        }

        private void captureSadako()
        {
            if (player.hitBox.Intersects(hitBox))
            {
                hitTestsThePlayer = true;
                if (!grabbed && !isStunned)
                {
                    startGrab();
                    player._speed = 0.2F;
                }
            }
            else
            {
                hitTestsThePlayer = false;
            }
        }

        public void drawCapture(SpriteBatch spriteBatch) 
        {
            if(grabbed)spriteBatch.Draw(grabTexture, new Vector2(-25,-25) + grabOffset, new Rectangle(0, frameLocation, 800, 480), Color.White, 0, Vector2.Zero, new Vector2(1.2F,1.2F), SpriteEffects.None, 0);
            spriteBatch.Draw(bloodTexture, Vector2.Zero, new Rectangle(0, 0, 800, 480), Color.White * bloodIntensity, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        private void wallCheckHandler() 
        {
            wallCollide = false;
            foreach (Rectangle rect in location.getCurrentRoom().getWalls()) 
            {
                if (rect.Intersects(hitBox)) wallCollide = true;
            }
        }

        private void stateHandler(GameTime gameTime) 
        {

            if (isDisturbed(gameTime) && !disturbed && !player.isHidden)
            {
                disturbed = true;
                lurking = false;
                speed = runSpeed;
                chaseEnded = false;
                songs.playChase();
                //songs.psycho.Play();
                events.startHeadTurn();  
            }
            else
            {
                if (disturbed)
                {
                    chaseCounter += gameTime.ElapsedGameTime.Milliseconds;
                    if (rand.Next(0, 6000000) < chaseCounter && !isDisturbed(gameTime))
                    {
                        chaseCounter = 0;
                        lurking = true;
                        disturbed = false;
                        speed = lurkSpeed;
                        if (!chaseEnded)
                        {
                            songs.chaseEnd.Play();
                            songs.switchSongToAmbient();
                            chaseEnded = true;
                            player.location.currentRoom.roomPresence = 0;
                            disable = true;
                        }
                    }
                    else if (player.isHidden && player.currentPresence < 20) 
                    {
                            chaseCounter = 0;
                            lurking = true;
                            disturbed = false;
                            speed = lurkSpeed;
                            if (!chaseEnded)
                            {
                                songs.chaseEnd.Play();
                                songs.switchSongToAmbient();
                                chaseEnded = true;
                                player.location.currentRoom.roomPresence = 0;
                                disable = true;
                            }
                   }
                }
            }
        }

        private void chaseHandler(GameTime gameTime) 
        {
            if (Vector2.Distance(position, player.position) > 0)
            {
                chasePlayer = true;
            }
            else
            {
                chasePlayer = false;
            }

            if (chasePlayer)
            {
                if (Vector2.Distance(player.position, position) > 128)
                {
                    while (Vector2.Distance(position, player.position) < Vector2.Distance(player.position, wayPoints.First()) && wayPoints.Count > 1)
                    wayPoints.Remove(wayPoints.First());
                    goTo(gameTime, wayPoints.First() + new Vector2(-25,-25));
                }
                else 
                {
                    wayPoints = new List<Vector2>();
                    wayPoints.Add(player.position);
                    goTo(gameTime, wayPoints.First() + new Vector2(-25, -25));
                }
            }
        }

        public float degree = 0;
        public int frameX = 0;
        public int frameY = 0;

        int frameCounter = 0;

        private void frameHandler(GameTime gameTime) 
        {
            if(lurking)degree = (float)Math.Atan2(lurkHere.X - position.X, position.Y - lurkHere.Y);
            else degree = (float)Math.Atan2(wayPoints.First().X - position.X, position.Y - wayPoints.First().Y);
            degree = MathHelper.ToDegrees(degree);

            if (degree >= 157 && degree <= 180 || degree >= -180 && degree <= -157) { frameY = 0; }
            else if (degree >= 113 && degree <= 157) { frameY = 7; }
            else if (degree >= 68 && degree <= 113) { frameY = 6; }
            else if (degree >= 23 && degree <= 68) { frameY = 5; }
            else if (degree <= 23 && degree >= -23) { frameY = 4; }
            else if (degree <= -23 && degree >= -68) { frameY = 3; }
            else if (degree <= -68 && degree >= -113) { frameY = 2; }
            else if (degree <= -113 && degree >= -157) { frameY = 1; }

            frameCounter +=gameTime.ElapsedGameTime.Milliseconds;
            int speed = 70;
            if (isDisturbed(gameTime)) speed = 50;

            if (frameCounter > speed)
            {
                frameCounter = 0;
                if (moving)
                {
                    if (frameX < 8) frameX++;
                    else frameX = 1;
                }
                else frameX = 0;
            }
            
            if (Vector2.Distance(player.position + new Vector2(-25, -25), position) < 20) 
            {
                frameX = 9;
            }

        }

        public void draw(SpriteBatch spriteBatch) 
        {
            if (!disable)
            {
                if (!isStunned)
                {
                    if (!wallCollide && player.isFlashLightOn) spriteBatch.Draw(texture, position - new Vector2(15, 30), new Rectangle(48 * frameX, 75 * frameY, 48, 75), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                    else spriteBatch.Draw(texture, position - new Vector2(15, 30), new Rectangle(48 * frameX, 75 * frameY, 48, 75), Color.White * 0.4F, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, position - new Vector2(15, 30), new Rectangle(0, 0, 48, 75), Color.White, MathHelper.ToRadians(80F), Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                }
            }
            spriteBatch.Draw(texture, hitBox, Color.Green);
        }

        private void lurkHandler(GameTime gameTime)
        {
            lurkCounter +=gameTime.ElapsedGameTime.Milliseconds;
            if(lurkCounter > 5000)
            {
                lurkCounter = 0;
                    List<Door> listOfDoors = location.sadakoRoom.getDoorList();

                    if (rand.Next(0, 5) == 1)
                    {
                        lurkHere = player.position;
                        disableTurnTransition = false;
                    }
                    else if (rand.Next(0, 4) == 1 && !disableTurnTransition)
                    {
                        foreach (Door currentDoor in listOfDoors)
                        {
                            if (rand.Next(0, 3) == 2)
                            {
                                Rectangle doorLocation = currentDoor.getLocation();
                                lurkHere = new Vector2(doorLocation.X, doorLocation.Y);
                            }
                        }
                    }
                    else 
                    {
                        foreach (Vector2 l in wayPoints) 
                        {
                            if (rand.Next(0, 6) == 3) 
                            {
                                disableTurnTransition = false;
                                lurkHere = l;
                                break;
                            }
                        }
                    }
            }
            if (lurkHere.X < 0 || lurkHere.X > 800 || lurkHere.Y > 480 || lurkHere.Y < 0) lurkHere = player.position;
            goTo(gameTime, lurkHere);
        }

        private void goTo(GameTime gameTime, Vector2 destination) 
        {
            CalculateCurrentDirection(destination);
            if (Vector2.Distance(position, destination) > 5)
            {
                moving = true;
                position += speed * direction * gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                moving = false;
                if (disturbed && wayPoints.Count > 1)
                {
                    wayPoints.Remove(wayPoints.First());
                }
            }
        }


        protected void CalculateCurrentDirection(Vector2 destination)
        {
            direction= CalculateDirection(position, destination);
        }
      
        protected Vector2 CalculateDirection(Vector2 source, Vector2 destination)
        {
            Vector2 result = destination - source;
            result.Normalize();
            return result;
        }

        public Boolean isDisturbed(GameTime gameTime)
        {
            if (player.presenceBox.Intersects(hitBox) || player.flashLightHitbox.Intersects(hitBox) && player.isFlashLightOn)
            {
                disturbCounter += gameTime.ElapsedGameTime.Milliseconds;
            }
            else disturbCounter=0;

            if (disturbCounter > 1000) return true;
            else return false;
        }

        private void shakeSadako() 
        {
            if (Vector2.Distance(player.position, position) < 50)
            {
               position += new Vector2(rand.Next(-5,5),rand.Next(-5,5));
            }
            else position += new Vector2(rand.Next(-2,2),rand.Next(-2,2));
        }

        public void teleport(Room roomGet) 
        {
            Boolean loopCut = true;
            Vector2 positionGet = position;
            while (loopCut)
            {
                loopCut = false;
                Vector2 random = new Vector2(rand.Next(0, 600), rand.Next(0, 480));
                Rectangle checkWall = new Rectangle((int)random.X, (int)random.Y, 2, 2);
                foreach (Rectangle wall in roomGet.getWalls())
                {
                    if (wall.Intersects(checkWall))
                    {
                        loopCut = true;
                    }
                }
                positionGet = random;
            }
            position = positionGet;
        }
    }
}
