using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public abstract class Table : Entity
    {
        protected Rectangle magnetHitBox;
        protected Boolean isActivated = false;
        protected Boolean isCurrentlyUsed = false;
        protected float elapsedTime;
        private Entity[,] minorEntities;

        public const float magnetPulseMagnitude = 1f;

        public Table(int size)
            : base(size)
        {
            setSize(size);
        }

        public Table(String name, Vector2 position) : base(name, position)
        {
            Initialize(name, position);
        }

        public new void Initialize(String name, Vector2 position)
        {
            magnetHitBox = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            minorEntities = new Entity[2,2];

            Random rand = new Random();

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    // randomly generate entities
                    if (rand.Next(2) == 0)
                    {
                        Vector2 entityPosition = new Vector2(position.X + x * 32, position.Y + y * 32);
                        int type = rand.Next(6);
                        minorEntities[x, y] = spawnOnTables(entityPosition, type);
                    }
                }
            }

            base.Initialize(name, position);
        }

        private Entity spawnOnTables(Vector2 position, int type)
        {
            Entity e = new Entity(Entity.SMALL);
            String name = "";
            switch (type)
            {
                case 0:
                    e.setDrawIndex(6);
                    name = "SinglePurseAndTissue";
                    break;
                case 1:
                    e.setDrawIndex(7);
                    name = "PlentyBottles";
                    break;
                case 2:
                    e.setDrawIndex(16);
                    name = "JarsAndBottles";
                    break;
                case 3:
                    e.setDrawIndex(17);
                    name = "PurseAndDolls";
                    break;
                case 4:
                    e.setDrawIndex(36);
                    name = "Dolls";
                    break;
                case 5:
                    e.setDrawIndex(37);
                    name = "Purse";
                    break;
            }
            e.Initialize(name, position);
            return e;
        }

        public virtual void Interact(Player p)
        {

        }
    }
}
