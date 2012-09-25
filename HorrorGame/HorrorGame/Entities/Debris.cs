using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public class Debris : Entity
    {
        public const int textureIndex = 12;
        public const float magnetPulseMagnitude = 1f;

        public Debris(int size) : base(size) {
            setSize(size);
        }

        public Debris(String name, Vector2 position) : base (name, position)
        {
            Initialize(name, position);
        }

        public new void Initialize(String name, Vector2 position)
        {
            currentDrawIndex = textureIndex;
            base.Initialize(name, position);
        }

        public override Boolean Touch(Player p)
        {
            if (base.Touch(p))
            {
                // TODO: Add noise to player
            }
            return false;
        }

        public void Update()
        {
            currentDrawIndex = textureIndex;
        }

        public void setActivated(Boolean b)
        {
            isActivated = b;
        }

        public Boolean isActive()
        {
            return isActivated;
        }
    }
}
