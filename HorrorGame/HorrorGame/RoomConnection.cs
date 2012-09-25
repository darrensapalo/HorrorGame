using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HorrorGame
{
    public class RoomConnection
    {
        private Room a, b;

        public RoomConnection(Room a, Room b)
        {
            this.a = a;
            this.b = b;
        }

        public Room getLeft()
        {
            return a;
        }

        public Room getRight()
        {
            return b;
        }

        public Boolean Equals(RoomConnection r)
        {
            return (r.getLeft() == a && r.getRight() == b) || (r.getLeft() == b && r.getRight() == a);
        }
    }
}
