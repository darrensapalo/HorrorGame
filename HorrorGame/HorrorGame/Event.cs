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
    public class EventHandler
    {
        Texture2D bg;
        Random rand = new Random();
        int counter=0;

        Texture2D headTurn;
        Boolean isHeadTurning = false;
        int headTurnFrame = 0;
        
        Texture2D falling;
        Texture2D falldown;
        Vector2 fallPos;
        Vector2 liePos;
        Boolean isFalling=false;

        Texture2D backgroundTexture;
        Texture2D sadakoFar;
        Texture2D sadakoFarNone;
        Vector2 vector = new Vector2(0, 0);
        Rectangle touchRect = new Rectangle(0, 0, 800, 480);
        public bool isOpen = false;
        private int? touchID = null;

        Texture2D headLeft;
        Texture2D bodyRight;
        Texture2D headRight;
        Texture2D bodyLeft;

        Texture2D cabinetPeek;
        public Sadako sadako;
        public Player player;

        public EventHandler() 
        {

        }

        public void Initialize(ContentManager Content, Texture2D bgGet, Sadako sadakoGet, Player playerGet)
        {
            headTurn = Content.Load<Texture2D>("facebacksheet");
            falling = Content.Load<Texture2D>("falling");
            falldown = Content.Load<Texture2D>("falldown");
            backgroundTexture = Content.Load<Texture2D>("peek");
            sadakoFar = Content.Load<Texture2D>("sadakoFar");
            sadakoFarNone = Content.Load<Texture2D>("sadakoFarNone");
            headLeft = Content.Load<Texture2D>("headLeft");
            bodyRight = Content.Load<Texture2D>("bodyRight");
            headRight = Content.Load<Texture2D>("headRight");
            bodyLeft = Content.Load<Texture2D>("bodyLeft");
            cabinetPeek = Content.Load<Texture2D>("cabinetPeek");
            bg = bgGet;
            sadako = sadakoGet;
            player = playerGet;
            sadako.setEvent(this);
        }

        public void startHeadTurn() 
        {
            isHeadTurning = true;
            headTurnFrame = 0;
            counter = 0;
            sadako.disable = true;
        }

        public void startFalling() 
        {
            isFalling = true;
            counter = 0;
            sadako.disable = true;
            fallPos = new Vector2(50,-700);
            liePos = new Vector2(50,500);
        }

        private void headTurnHandler(GameTime gameTime) 
        {
            int frameDuration = 150;
            counter += gameTime.ElapsedGameTime.Milliseconds;

            if (counter > frameDuration && headTurnFrame < 3) 
            {
                counter = 0;
                headTurnFrame++;
            }
            if (counter > frameDuration * 3) 
            {
                isHeadTurning = false;
                sadako.disable = false;
            }
        }

        private void fallingHandler(GameTime gameTime) 
        {
            counter += gameTime.ElapsedGameTime.Milliseconds;
            if (fallPos.Y <= 4200) fallPos.Y += gameTime.ElapsedGameTime.Milliseconds*4;
            else 
            {
                if(liePos.Y>-150)liePos.Y -= gameTime.ElapsedGameTime.Milliseconds*3;
            }
            if (counter > 1500) 
            {
                isFalling = false;
                sadako.disable = false;
            }
        }

        private void drawHeadTurn(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(bg, Vector2.Zero, null, Color.Black, 0, Vector2.Zero, new Vector2(800,480), SpriteEffects.None, 0);
            spriteBatch.Draw(headTurn, new Vector2(250,0) + new Vector2(rand.Next(-10,10), rand.Next(-10,10)), new Rectangle(0, headTurnFrame*500, 500, 500), Color.White , 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        private void drawFalling(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(bg, Vector2.Zero, null, Color.Black, 0, Vector2.Zero, new Vector2(800, 480), SpriteEffects.None, 0);

            if (fallPos.Y <= 800) spriteBatch.Draw(falling, fallPos, new Rectangle(0, 0, 800, 800), Color.White, 0, Vector2.Zero, new Vector2(0.8F, 0.8F), SpriteEffects.None, 0);
            else
            {
                 spriteBatch.Draw(falldown, liePos, new Rectangle(0, 0, 800, 800), Color.White, 0, Vector2.Zero, new Vector2(0.8F, 0.8F), SpriteEffects.None, 0);
            }
        }

        private void drawDoorEvent(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(bg, Vector2.Zero, null, Color.Black, 0, Vector2.Zero, new Vector2(800, 480), SpriteEffects.None, 0);
            spriteBatch.Draw(sadakoFarNone, new Vector2(rand.Next(-2, 2), rand.Next(-2, 2)), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            drawShakeHeadLeft(spriteBatch);
            spriteBatch.Draw(this.backgroundTexture, vector+new Vector2(rand.Next(-1, 1), rand.Next(-1, 1)), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        private void drawCabinetPeek(SpriteBatch spriteBatch) 
        {
            drawShakeHeadRight(spriteBatch,player.position);
            spriteBatch.Draw(cabinetPeek, new Vector2(rand.Next(-1, 1)+player.position.X-800, 0), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        private void drawShakeHeadLeft(SpriteBatch spriteBatch) 
        {
            Vector2 pos = new Vector2(310,320);
            spriteBatch.Draw(bodyRight,new Vector2(rand.Next(-2, 2), rand.Next(-2, 2)), null, Color.White, 0, Vector2.Zero, 0.8F, SpriteEffects.None, 0);
            spriteBatch.Draw(headLeft, pos, null, Color.White, MathHelper.ToRadians(rand.Next(-2, 2)), new Vector2(400,400), 0.8F, SpriteEffects.None, 0);
        }

        private void drawShakeHeadRight(SpriteBatch spriteBatch, Vector2 offset)
        {
            Vector2 pos = new Vector2(350, 320);
            spriteBatch.Draw(bodyLeft, new Vector2(350+rand.Next(-2, 2)+offset.X-400, rand.Next(-2, 2)), null, Color.White, 0, Vector2.Zero, 0.8F, SpriteEffects.None, 0);
            spriteBatch.Draw(headRight, pos+new Vector2(offset.X-400,0), null, Color.White, MathHelper.ToRadians(rand.Next(-2, 2)), new Vector2(400, 400), 0.8F, SpriteEffects.None, 0);
        }

        public void update(GameTime gameTime, TouchCollection touchie) 
        {
            if (isHeadTurning) headTurnHandler(gameTime);
            if (isFalling) fallingHandler(gameTime);
            controller(touchie);
        }

        int initialPoint = 0;
        int initialPos = 0;

        private void controller(TouchCollection touchie) 
        {
            Point touchPoint = Point.Zero;
            foreach (TouchLocation location in touchie)
            {
                touchPoint.X = (int)location.Position.X;
                touchPoint.Y = (int)location.Position.Y;

                if (touchRect.Contains(touchPoint))
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            touchID = location.Id;
                            initialPoint = touchPoint.X;
                            initialPos = touchRect.X;

                            break;
                        case TouchLocationState.Released:

                            if (isOpen)
                            {
                                if (vector.X > 400)
                                {
                                    isOpen = false;
                                }
                            }

                            isOpen = !isOpen;
                            touchID = null;
                            initialPos = 0;
                            break;
                    }

                }

                if (location.State == TouchLocationState.Moved && touchID != null && touchID == location.Id)
                {
                    int position = initialPos + touchPoint.X - initialPoint;
                    vector.X = touchRect.X = position = (int)MathHelper.Clamp(position,-740,0);
                }

                if (vector.X < 400)
                {
                    isOpen = false;
                }

                if (isOpen)
                {
                    if (vector.X < 570)
                    {
                        touchRect.X += 20;
                        vector.X += 20;
                    }
                }
                else
                {
                    if (vector.X > 0)
                    {
                        touchRect.X -= 20;
                        vector.X -= 20;
                    }
                }
            }
        }

        public void draw(SpriteBatch spriteBatch) 
        {
            if (isHeadTurning) drawHeadTurn(spriteBatch);
            if (isFalling) drawFalling(spriteBatch);
            //drawDoorEvent(spriteBatch);
            //if(player.isHidden)drawCabinetPeek(spriteBatch);
        }
        
    }
}
