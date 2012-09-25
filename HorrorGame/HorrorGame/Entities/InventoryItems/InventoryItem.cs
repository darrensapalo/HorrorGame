using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame
{
    public enum ItemStatus
    {
        OnFloor = 0,
        OnHand,
        InInventory,
    }

    public abstract class InventoryItem : Entity
    {
        public const float magnetPulseMagnitude = 1f;
        public ItemStatus itemStatus;
        

        public InventoryItem(int size) : base(size) {
            setSize(size);
        }

        public InventoryItem(String name, Vector2 position)
            : base(name, position)
        {
            Initialize(name, position);
        }

        public new void Initialize(String name, Vector2 position, int textureIndex)
        {
            currentDrawIndex = textureIndex;
            itemStatus = ItemStatus.OnFloor;
            base.Initialize(name, position);
        }

        public new void Interact(Player p)
        {
            if (itemStatus == ItemStatus.OnFloor && hitbox.Intersects(p.hitBox))
            {
                // Show image of item first
            }
            else if (itemStatus == ItemStatus.OnHand)
            {
                // Interacting again should close
            }
            else if (itemStatus == ItemStatus.InInventory)
            {

            }
            
        }

        public new void Update()
        {

        }

    }
}
