using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HorrorGame
{

    /// <summary>
    /// This class holds a 2D int [,] array which holds different kinds of tileDesigns.
    /// </summary>
    public class RoomDesign
    {

        private int[,] roomDesign = new int[Room.ROOM_WIDTH, Room.ROOM_HEIGHT];
        private int roomRotation;
        private int roomType;

        /// <summary>
        /// This initializes the room design given a 2D int [,] array.
        /// As the object is constructed, it determines the roomRotation and
        /// room type of the room.
        /// </summary>
        /// <param name="design"></param>
        public RoomDesign(int[,] design)
        {
            roomDesign = design;
            Initialize(design);
        }

        /// <summary>
        /// This sets the room's roomRotation and room type.
        /// </summary>
        /// <param name="design">The design of the room</param>
        /// 
        public void Initialize(int[,] design)
        {
            /* 
             * Sets the roomRotation to Room.UP, Room.DOWN, Room.LEFT, Room.RIGHT
             * and roomType to Room.INTERSECTION, Room.TRIAD, or Room.CORRIDOR
             * 
             * So this checks each tile, to find the sides (x == 0, x == Room.WIDTH - 1, y == 0, y == Room.HEIGHT - 1)
             * If the tile found at the side --Vector(x, y)-- is a door design --Room.doors.contains(tileDesign)--
             * 
             * Then there is a door there. Compute for what room type it is
             */

            Boolean up, down, left, right;
            up = down = left = right = false;

            roomRotation = Room.UNDEFINED;
            roomType = Room.UNDEFINED;

            for (int y = 0; y < Room.ROOM_HEIGHT; y++)
            {
                for (int x = 0; x < Room.ROOM_WIDTH; x++)
                {
                    int des = roomDesign[x,y];
                    // If the location is y zero and it has a door
                    if (Room.isDoor(des))
                    {
                        if (Room.typeOfDoor(des) == Room.UP)
                        {
                            up = true;
                            continue;
                        }

                        // If the location is y is max and it has a door
                        if (Room.typeOfDoor(des) == Room.DOWN)
                        {
                            down = true;
                            continue;
                        }

                        // If the location is x zero and it has a door
                        if (Room.typeOfDoor(des) == Room.LEFT)
                        {
                            left = true;
                            continue;
                        }

                        // If the location is x is max and it has a door
                        if (Room.typeOfDoor(des) == Room.RIGHT)
                        {
                            right = true;
                            continue;
                        }
                    }
                }
            }

            // Hard code check all orientations

            if (up && down && left && right)
            {
                roomType = Room.INTERSECTION;
                roomRotation = Room.LEFT;
            }
            else

                // Triads
                if (up && down && left)
                {
                    roomType = Room.TRIAD;
                    roomRotation = Room.LEFT;
                }
                else
                    if (up && down && right)
                    {
                        roomType = Room.TRIAD;
                        roomRotation = Room.RIGHT;
                    }
                    else
                        if (left && right && up)
                        {
                            roomType = Room.TRIAD;
                            roomRotation = Room.UP;
                        }
                        else
                            if (left && right && down)
                            {
                                roomType = Room.TRIAD;
                                roomRotation = Room.DOWN;
                            }
                            else

                                // Corridor
                                if (up && down)
                                {
                                    roomType = Room.CORRIDOR;
                                    roomRotation = Room.UP;
                                }
                                else

                                    if (left && right)
                                    {
                                        roomType = Room.CORRIDOR;
                                        roomRotation = Room.LEFT;
                                    }
                                    else
                                        if (left && up)
                                        {
                                            roomType = Room.CORNER;
                                            roomRotation = Room.LEFT;
                                        }
                                        else
                                            if (up && right)
                                            {
                                                roomType = Room.CORNER;
                                                roomRotation = Room.UP;
                                            }
                                            else
                                                if (right && down)
                                                {
                                                    roomType = Room.CORNER;
                                                    roomRotation = Room.RIGHT;
                                                }
                                                else
                                                    if (down && left)
                                                    {
                                                        roomType = Room.CORNER;
                                                        roomRotation = Room.DOWN;
                                                    }
                                                    else
                                                        if (left)
                                                        {
                                                            roomType = Room.DEAD_END;
                                                            roomRotation = Room.LEFT;
                                                        }
                                                        else
                                                            if (up)
                                                            {
                                                                roomType = Room.DEAD_END;
                                                                roomRotation = Room.UP;
                                                            }
                                                            else
                                                                if (right)
                                                                {
                                                                    roomType = Room.DEAD_END;
                                                                    roomRotation = Room.RIGHT;
                                                                }
                                                                else
                                                                    if (down)
                                                                    {
                                                                        roomType = Room.DEAD_END;
                                                                        roomRotation = Room.DOWN;
                                                                    }


            // Dead end
            // Not implemented to prevent short mazes.
        }

        /// <summary>
        /// Getter for the tile design, this returns the 2D int [,] array which holds the tile designs of the room.
        /// </summary>
        /// <returns></returns>
        public int[,] getTileDesign()
        {
            return roomDesign;
        }

        /// <summary>
        /// Getter for the room roomRotation, this returns the roomRotation of the room.
        /// </summary>
        /// <returns></returns>
        public int getRotation()
        {
            return roomRotation;
        }

        /// <summary>
        /// Getter for the room type, this returns the type of the room.
        /// </summary>
        /// <returns></returns>
        public int getRoomType()
        {
            return roomType;
        }
    }
}
