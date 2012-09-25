using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HorrorGame
{
    public class Door
    {
        private Rectangle location;
        private int dir;

        public Door(Rectangle r, int dir)
        {
            location = r;
            this.dir = dir;
        }

        public Rectangle getLocation()
        {
            return location;
        }

        public int getDirection()
        {
            return dir;
        }

        public Vector2 getDestination()
        {
            float offset = 32;
            // The 16 represents the middle of the tile
            Vector2 v = new Vector2(location.X + 16, location.Y + 16);
            switch (dir)
            {
                case Room.LEFT: v.X += offset; break;
                case Room.RIGHT: v.X -= offset; break;
                case Room.UP: v.Y += offset; break;
                case Room.DOWN: v.Y -= offset; break;
            }
            return v;
        }
    }
}
