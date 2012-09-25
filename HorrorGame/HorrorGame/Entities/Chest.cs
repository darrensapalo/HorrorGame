using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public class Chest : Entity
    {
        public Chest(String name, Vector2 position):base(name, position)
        {
            base.Initialize(name, position);
        }

        public void Interact() { }
    }
}
