using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public class Cabinet : HidingSpot
    {
        public const int notActivatedIndex = 0;
        public const int activatedIndex = 1;

        public Cabinet(String name, Vector2 position) : base(name, position)
        {
            Random r = new Random();
            sprite = ((r.Next(3) + (int)position.X) % 3) * 5;
            Initialize(name, position);
        }
        public override void Interact(Player p) {

            base.Interact(p);
        }

        public override int getIndex()
        {
            return (isActivated) ? sprite : sprite + 1;
        }
    }
}
