using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public class ItemSpawnEntity : Entity
    {
        private int[] sprite = new int[2];

        public ItemSpawnEntity(String name, Vector2 position) : base(name, position)
        {
            setSize(SMALL);
            Random r = new Random();
            sprite[0] = 17;
            sprite[1] = 27;
            currentDrawIndex = sprite[r.Next(2)];
            base.Initialize(name, position);
        }
        public void Interact(Player p) {

        }
    }
}
