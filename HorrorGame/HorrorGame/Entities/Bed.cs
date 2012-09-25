using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public class Bed : HidingSpot
    {
        public const int notActivatedIndex = 21;
        public const int activatedIndex = 20;

        public Bed(String name, Vector2 position)
            : base(name, position)
        {
            currentDrawIndex = getIndex();
            sprite = notActivatedIndex;
            Initialize(name, position);
        }
                
        public override void Interact(Player p)
        {
            // Beds cannot be used.
            // base.Interact(p);
        }

        public override int getIndex()
        {
            return (isActivated) ? activatedIndex : notActivatedIndex;
        }
    }
}
