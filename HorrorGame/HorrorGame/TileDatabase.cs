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
    public class TileDatabase
    {
        Texture2D tileSet;
        Rectangle[] tileAreas;

        public TileDatabase()
        {
        }

        public void Initialize(ContentManager Content)
        {
            tileSet = Content.Load<Texture2D>("tileset_v1");
            initializeRectangles();
        }

        private void initializeRectangles() 
        {
            int x = 0;
            int y = 0;
            List<Rectangle> tileAreasList = new List<Rectangle>();

            while (y < 640) 
            {
                while (x < 640) 
                {
                    tileAreasList.Add(new Rectangle(x, y, 32, 32));
                    x += 32;
                }
                x = 0;
                y += 32;
            }
            tileAreas = tileAreasList.ToArray();
        }

        public void drawTile(SpriteBatch spriteBatch, Vector2 position, int number) 
        {
            if (number > 400)
            {
                number = 0;
            }
            spriteBatch.Draw(tileSet, position,tileAreas[number],Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
    
    }
}
