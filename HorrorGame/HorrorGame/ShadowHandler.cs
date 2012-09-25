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
    public class ShadowHandler
    {
        Texture2D texture;
        Texture2D squareTexture;
        Player player;
        Vector2 position;
        Rectangle rectangle;
        float degree;

        public ShadowHandler(Texture2D textureGet, Player playerGet, Vector2 positionGet, Texture2D squareTextureGet) 
        {
            texture = textureGet;
            player = playerGet;
            position = positionGet;
            squareTexture = squareTextureGet;
            rectangle = new Rectangle((int)position.X,(int)position.Y,32,32);
        }

        public void update() 
        {
            if (isHit())
            {
                degree = (float)Math.Atan2(player.position.X - (position.X + 16F), (position.Y + 16F) - player.position.Y);
            }
        }

        public Boolean isHit() 
        {
            return player.shadowHitBox.Intersects(rectangle);
        }

        public float getDistance() 
        {
        return Vector2.Distance(player.position,this.position + new Vector2(16F,16F));
        }

        Vector2 offset = new Vector2(16, 16);
        Rectangle rect = new Rectangle(0, 0, 400, 400);
        Vector2 origin = new Vector2(200, 16);
        Vector2 size = new Vector2(1.2F, 1F);

        public void draw(SpriteBatch spriteBatch, float intensity)
        {
            if (isHit()) spriteBatch.Draw(texture, position + offset, rect, Color.White * intensity, (float)degree, origin, size, SpriteEffects.None, 0);
        }
    }
}
