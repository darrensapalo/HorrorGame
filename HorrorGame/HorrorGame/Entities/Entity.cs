using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace HorrorGame
{
    public class Entity
    {
        protected Rectangle magnetHitBox;

        protected Boolean isActivated = false;
        protected Boolean isCurrentlyUsed = false;
        protected int activatedDrawIndex;

        protected float elapsedTime;

        public const float magnetPulseMagnitude = 1f;

        protected Vector2 position;
        protected static Texture2D texture;
        protected String name;
        protected int currentDrawIndex;
        protected Rectangle hitbox;
        protected int size;
        
        public static Rectangle[] listOfBigSprites;
        public static Rectangle[] listOfSmallSprites;

        public const int GENERIC = 0;
        public const int CABINET = 1;
        public const int CURTAIN = 2;
        public const int BED = 3;

        public const int ITEM_SPAWNER = 4;
        public const int DEBRIS = 5;
        public const int TABLE = 6;
        public const int BIG_HOLE = 7;
        public const int SMALL_HOLE = 8;
        public const int BOTTLES1 = 9;
        public const int BOTTLES2 = 10;
        public const int PAPER = 11;
        public const int HAIR = 12;
        
        // size
        public const int SMALL = 1;
        public const int BIG = 2;

        // draw indices
        public const int DEBRIS_INDEX = 12;
        public const int TABLE_INDEX = 16;
        public const int BIG_HOLE_INDEX = 9;
        public const int SMALL_HOLE_INDEX = 14;
        public const int BOTTLES1_INDEX = 8;
        public const int BOTTLES2_INDEX = 9;
        public const int PAPER_INDEX = 7;
        public const int HAIR_INDEX = 7;



        public static List<int> indexesOfGenericEntities = new List<int>();
        private float elapsedMilliseconds = 0;

        public Entity(int size) {
            setSize(size);
        }

        protected void setSize(int n)
        {
            this.size = n * 32;
        }

        public Entity(String name, Vector2 position, int size)
        {
            setSize(size);
            Initialize(name, position);
        }
        
        public Entity(String name, Vector2 position, int size, int index)
        {
            setDrawIndex(index);
            setSize(size);
            Initialize(name, position);
        }

        public Entity(String name, Vector2 position)
        {
            setSize(BIG);
            Initialize(name, position);
        }

        public void Initialize(String name, Vector2 position)
        {
            this.name = name;
            this.position = position;
            hitbox = new Rectangle((int)position.X, (int)position.Y, size, size);
            if (activatedDrawIndex == 0)
            {
                activatedDrawIndex = currentDrawIndex;
            }
        }

        public static void Initialize(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("entitySheet");
            initializeRectangles();
        }

        private static void initializeRectangles() 
        {
            int x = 0;
            int y = 0;
            List<Rectangle> tileAreasList = new List<Rectangle>();

            int size = 64;
            while (y < 320) 
            {
                while (x < 320) 
                {
                    tileAreasList.Add(new Rectangle(x, y, size, size));
                    x += size;
                }
                x = 0;
                y += size;
            }
            listOfBigSprites = tileAreasList.ToArray();
            tileAreasList.Clear();


            for (y = 0; y < 10; y++) 
            {
                for (x = 0; x < 10; x++)
                {
                    tileAreasList.Add(new Rectangle(x * 32, y * 32, 32, 32));
                }
            }
            listOfSmallSprites = tileAreasList.ToArray();
            tileAreasList.Clear();

        }

        public virtual void Update(GameTime gameTime, Player p)
        {

            /*
            elapsedMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedMilliseconds > 2000)
            {
                currentDrawIndex = (currentDrawIndex == 0) ? 1 : 0;
                elapsedMilliseconds = 0;
            }
             */
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (size / 32 == BIG)
            {
                spriteBatch.Draw(texture, position, listOfBigSprites[currentDrawIndex], Color.White);
            }
            else if (size / 32 == SMALL)
            {
                spriteBatch.Draw(texture, position, listOfSmallSprites[currentDrawIndex], Color.White);
            }
            
        }


        public virtual void Interact(Player p)
        {
            if (magnetHitBox.Intersects(p.hitBox))
            {
                // Current location of player
                Vector2 player = p.position;
                // Center the player in the hiding spot
                Vector2 destination = new Vector2(position.X + 32, position.Y + 48);
                isActivated = true;

                if (elapsedTime >= 1500)
                {
                    p.position = destination;
                    p.isHidden = true;
                    elapsedTime = 0;
                }
                else
                {

                    Vector2 force = Vector2.Zero;
                    if (player.X < destination.X)
                        force.X += magnetPulseMagnitude;
                    else
                        force.X -= magnetPulseMagnitude;

                    if (player.Y < destination.Y)
                        force.Y += magnetPulseMagnitude;
                    else
                        force.Y -= magnetPulseMagnitude;

                    p.position += force;
                }
            }
            else if (isActivated)
            {
                isActivated = false;
                p.isHidden = false;
                elapsedTime = 0;
            }
        }

        public virtual Boolean Touch(Player p)
        {
            if (isActivated) return false;
            else if (p.hitBox.Intersects(hitbox))
            {
                currentDrawIndex = activatedDrawIndex;
                return isActivated = true;
            }
            return false;
            
        }

        public void setActivated(Boolean b)
        {
            isActivated = b;
        }

        public Boolean isActive()
        {
            return isActivated;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public Rectangle getHitBox()
        {
            return hitbox;
        }

        public void setDrawIndex(int n)
        {
            currentDrawIndex = n;
        }
    }
}
