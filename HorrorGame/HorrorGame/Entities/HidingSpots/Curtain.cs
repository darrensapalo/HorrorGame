using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public class Curtain : HidingSpot
    {
        private int activatedIndex = 17;

        public Curtain(String name, Vector2 position): base(name, position)
        {
            sprite = currentDrawIndex = activatedIndex;
            base.Initialize(name, position);
        }
        
        public override void Interact(Player p)
        {
            base.Interact(p);
        }

        public override int getIndex()
        {
            return sprite;
        }

    }
}
