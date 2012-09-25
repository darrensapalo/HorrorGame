using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public abstract class HidingSpot : Entity
    {
        
        protected int sprite;


        public HidingSpot(int size) : base(size) {
            setSize(size);
        }

        public HidingSpot(String name, Vector2 position) : base (name, position)
        {
            base.Initialize(name, position);
        }

        public new void Initialize(String name, Vector2 position)
        {
            magnetHitBox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            base.Initialize(name, position);
        }

        public override void Update(GameTime gameTime, Player p)
        {
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (p.hitBox.Intersects(hitbox))
                currentDrawIndex = getIndex();
            else
                currentDrawIndex = sprite;
        }

        public virtual int getIndex() { return 0; }

    }
}
