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
    public class PlayerController
    {
        public Vector2 initialPosition;
        public Vector2 currentPosition;
        Texture2D texture;
        int _touchid = -1;
        Player player;
        int buttonSize = 1;
        Boolean viewDragged=false;
        Boolean _viewDragged=false;
        int width = 640;
        int height = 330;
        Rectangle controllerarea;
        Boolean isDragging = false;
        int playFieldWidth = 610;
        int playFieldHeight = 480;
        int totalPlayField;
        Rectangle buttonarea;
        Boolean isPressing = false;
        LocationHandler location;
        Vector2 viewDragCurrent;
        Boolean viewDragStart;

        Rectangle flashLightArea;
        Vector2 flashLightPosition;
        Boolean isFlashlightTouched;

        float presenceDegree;

        
        public PlayerController(Texture2D textureGet, Player playerGet,LocationHandler locationGet) 
        {
            texture = textureGet;
            location = locationGet;

            initialPosition = new Vector2(720, 400);
            currentPosition = new Vector2(720, 400);
            player = playerGet;
            totalPlayField = playFieldWidth + 40;
            controllerarea = new Rectangle(800 - 160, 480 - 150, 150, 150);
            buttonarea = new Rectangle(640, 270, 76, 60);

            flashLightArea = new Rectangle(650,200,75,70);
            flashLightPosition = new Vector2(650, 218);

            initializeBeat();
        }

        public void update(GameTime gameTime, Room currentRoom, TouchCollection locations)
        {
            presenceHandler();
            heartBeatHandler(gameTime);
            if (locations.Count == 0)
            {
                viewDragged = false;
                isDragging = false;
                isPressing = false;
                viewDragStart = false;
                player.isMoving = false;
                player.inventory.itemGetCheck = false;
                if (isFlashlightTouched)
                {
                    isFlashlightTouched = false;
                    if (flashLightPosition.X > 685)
                    {
                        flashLightPosition.X = 720;
                        flashLightArea.X = 720;
                        player.isFlashLightOn = false;
                    }
                    else 
                    {
                        flashLightPosition.X = 650;
                        flashLightArea.X = 650;
                        player.isFlashLightOn = true;
                    }
                }
            }

            foreach (TouchLocation location in locations)
            {
                Rectangle touch = new Rectangle((int)location.Position.X,(int)location.Position.Y,buttonSize,buttonSize);


                if (flashLightArea.Intersects(touch))
                {
                    if (!isPressing && !isDragging && !viewDragged || isFlashlightTouched)
                    {
                        isFlashlightTouched = true;
                        if (location.State == TouchLocationState.Pressed || location.State == TouchLocationState.Moved)
                        {
                            flashLightPosition.X = touch.X - 33;
                            flashLightArea.X = touch.X - 33;
                            if (flashLightPosition.X < 650) 
                            {
                                flashLightPosition.X = 650;
                                flashLightArea.X = 650;
                            }
                            if (flashLightPosition.X > 720) 
                            {
                                flashLightPosition.X = 720;
                                flashLightArea.X = 720;
                            }
                        }
                    }
                }

                if (buttonarea.Intersects(touch))
                {
                    if (!isPressing && !isDragging && !viewDragged && !isFlashlightTouched)
                    {
                        isPressing = true;
                        if (player._speed > 0.1F)
                        {
                            player._speed = 0.1F;
                        }
                        else player._speed = 0.18F;
                    }
                }

#if true
                foreach (Entity currentEntities in currentRoom.getEntities())
                {
                    if (!(currentEntities is HidingSpot))
                        continue;
                    Rectangle currentLocation = new Rectangle((int)currentEntities.getPosition().X, (int)currentEntities.getPosition().Y, 64, 64);
                    /*
                        * currentEntity
                        * at the currentLocation
                        * using the currentTouch
                        */
                    if (currentLocation.Intersects(touch))
                    {
                        player.currentHidingSpot = (HidingSpot)currentEntities;
                        if (currentEntities is HidingSpot)
                            ((HidingSpot)currentEntities).Interact(player);
                        break;
                    }
                }

#endif
                
                if (controllerarea.Intersects(touch))
                {
                    isDragging = true;
                }
                else 
                {
                    viewDragged = true;
                }

                if (viewDragged && !viewDragStart && !player.inventory.isOpen && !isPressing && !isFlashlightTouched && !player.inventory.itemGetCheck) 
                {
                    viewDragCurrent = new Vector2(location.Position.X, location.Position.Y);
                    viewDragStart = true;
                }

                if (viewDragStart) 
                {
                    viewDragCurrent = location.Position;
                    getViewDegree();
                }

                if (isDragging)
                {

                    if (!_viewDragged)
                        TryDrag(location);

                    if (_touchid != location.Id || !_viewDragged)
                        continue;

                    TryMove(location, gameTime);
                }
                 
            }

            
            if(initialPosition!=currentPosition)getDegree();
            
          
        }
        public void reset()
        {
            _viewDragged = viewDragged = viewDragStart = false;
        }
        private void TryMove(TouchLocation location,GameTime gameTime)
        {
           
            if (location.State == TouchLocationState.Pressed || location.State == TouchLocationState.Moved)
            {

                float x = location.Position.X;
                float y = location.Position.Y;
                if(x<width) x = width;
                if(y<height) y = height;
                if (x > 800) x = 800;
                if (y > 480) y = 480;
                currentPosition = new Vector2(x,y);

                this.CalculateCurrentDirection();

                playerMove(gameTime);
            }
            else
            {
                _viewDragged = false;
                _touchid = -1;
                currentPosition = initialPosition;
            }
        }

        private void TryDrag(TouchLocation location)
        {
            if (location.State == TouchLocationState.Pressed)
            {
                _viewDragged = true;
                _touchid = location.Id;

            }
        }

        private void presenceHandler() 
        {
            presenceDegree = player.currentSound * 180 / player.maxSound;
        }

        Rectangle hardBeat = new Rectangle(0, 331, 24, 21);
        Rectangle noBeat = new Rectangle(0, 311, 23, 10);
        Rectangle softBeat = new Rectangle(0, 351, 23, 14);

        class beat 
        {
            public beat(Vector2 placingGet, Rectangle rectGet) 
            {
                beatRect = rectGet;
                placing = placingGet;
            }
            public Rectangle beatRect;
            public Vector2 placing;
        }

        //654 start new Vector2(654, 146)

        List<beat> beats = new List<beat>();

        private void initializeBeat()
        {
            beats.Add(new beat(new Vector2(654 + 13 * 0, 146), softBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 1, 146), noBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 2, 146), softBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 3, 146), noBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 4, 146), softBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 5, 146), noBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 6, 146), softBeat));
            beats.Add(new beat(new Vector2(654 + 13 * 7, 146), noBeat));
            
        }

        int beatCounter = 0;

        private void heartBeatHandler(GameTime gameTime) 
        {
            beatCounter +=gameTime.ElapsedGameTime.Milliseconds;
            if (beatCounter > 50) 
            {
                beatCounter = 0;
                foreach (beat b in beats)
                {
                    b.placing.X++;
                    if (b.placing.X > 767) b.placing.X = 654;
                }
            }
        }

        public void draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(texture, new Vector2(640, 0), new Rectangle(80, 0, 160, 480), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            if (player._speed == 0.1F)
                spriteBatch.Draw(texture, new Vector2(640, 259), new Rectangle(0, 129, 80, 60), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
 
            foreach(beat b in beats)
            {
                spriteBatch.Draw(texture,b.placing, b.beatRect, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, new Vector2(639, 130), new Rectangle(80, 531, 160, 70), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, currentPosition, new Rectangle(0, 190, 80, 80), Color.White, 0, new Vector2(40), Vector2.One, SpriteEffects.None, 0);
            
            spriteBatch.Draw(texture, flashLightPosition, new Rectangle(0, 270, 75, 37), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, new Vector2(722, 2), new Rectangle(0, 0, 15, 63), Color.White, -MathHelper.ToRadians(presenceDegree + 130), new Vector2(4, 54), Vector2.One, SpriteEffects.None, 0);   
            
        }


        protected void CalculateCurrentDirection()
        {
            player.direction = CalculateDirection(initialPosition, currentPosition);
        }
      
        protected Vector2 CalculateDirection(Vector2 source, Vector2 destination)
        {
            Vector2 result = destination - source;
            result.Normalize();
            return result;
        }

        private void playerMove(GameTime gameTime)
        {
            player.isMoving = true;
            if(player.position.X < 0)player.position.X = 0;
            if(player.position.Y < 0)player.position.Y = 0;
            if(player.position.X > playFieldWidth)player.position.X = playFieldWidth;
            if (player.position.Y > playFieldHeight) player.position.Y = playFieldHeight;

            float tempX=0, tempY=0;
            Boolean collidedX= false;
            Boolean collidedY = false;
            tempX = player.position.X;
            tempX += player._speed * player.direction.X * gameTime.ElapsedGameTime.Milliseconds;
            Rectangle collisionTest = new Rectangle((int)tempX, (int)player.position.Y, 32, 32);
            foreach (Rectangle rect in player.location.getCurrentRoom().getWalls()) 
            {
                Rectangle check = rect;
                check.X += 16;
                check.Y += 16;
                if (check.Intersects(collisionTest)) { collidedX = true; }
            }
            if (!collidedX && !float.IsNaN(tempX)) player.position.X = tempX;

            tempY = player.position.Y;
            tempY += player._speed * player.direction.Y * gameTime.ElapsedGameTime.Milliseconds;
            Rectangle collisionTest2 = new Rectangle((int)player.position.X, (int)tempY, 32, 32);
            foreach (Rectangle rect in player.location.getCurrentRoom().getWalls())
            {
                Rectangle check = rect;
                check.X += 16;
                check.Y += 16;
                if (check.Intersects(collisionTest2)) { collidedY = true; }
            }
            if (!collidedY && !float.IsNaN(tempY)) player.position.Y = tempY;
            if (float.IsNaN(tempX) || float.IsNaN(tempY))
            {
                Console.Write("XXXX");
                return;
            }
        }


        private void getDegree() 
        {
            player.degree = Math.Atan2(currentPosition.X - initialPosition.X, initialPosition.Y - currentPosition.Y);
        }

        private void getViewDegree()
        {
            player.degree = Math.Atan2(viewDragCurrent.X - player.position.X, player.position.Y - viewDragCurrent.Y);
        }
        

    }
}
