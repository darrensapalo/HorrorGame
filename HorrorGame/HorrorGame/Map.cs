#define KRUSKAL
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HorrorGame
{
    /// <summary>
    /// This is a game component that holds all the rooms together
    /// and facilitates the generation and connections of the rooms.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// A reference to the game should it need some of the game's attributes.
        /// </summary>
        private Game gameReference;
        /// <summary>
        /// A sprite font used for debugging purposes.
        /// </summary>
        private SpriteFont spriteFont;
        /// <summary>
        /// A texture used for displaying pixelized representation of the map for debugging.
        /// </summary>
        private List<Texture2D> debugTextures;

        /// <summary>
        /// A Random object used by the Map to generate random results.
        /// </summary>
        private Random random = new Random();
        /// <summary>
        /// The map definition, which holds the starting X and Y location and the
        /// LENGTH and WIDTH of the map.
        /// </summary>
        private Rectangle mapDefinition;
        /// <summary>
        /// The end location that the player must reach.
        /// </summary>
        private Vector2 endLocation;

        private RoomDatabase roomDatabaseReference;

        /// <summary>
        /// This List holds the complete list of the rooms available in a Map.
        /// <
        /// </summary>
        public List<Room> listOfRooms = new List<Room>();
        private List<Room> exploredRooms = new List<Room>();
        private List<Room> visitedRooms = new List<Room>();

        private List<Room> kruskalRooms = new List<Room>();
        private List<List<Room>> sets = new List<List<Room>>();

        public static int NO_DOOR_REQUIRED = -1;

        private Texture2D shadowTexture;
        private Player playerReference;

        /// <summary>
        /// Used for displaying a pixelized representation of the map.
        /// </summary>
        public const Boolean DEBUG = true;


        public Map()
        {
        }

        public void setDebugTexture(List<Texture2D> t)
        {
            debugTextures = t;
        }

        /// <summary>
        /// This initializes a new map given a rectangle which has
        /// the starting X and Y locations of the player and the 
        /// width and size of the map (not the room).
        /// </summary>
        /// <param name="init"></param>
        public void Initialize(Game game, Player playerReference, ContentManager Content, Rectangle init, Texture2D tileTexture, List<Texture2D> mapDebuggingTextures, RoomDatabase roomDatabaseReference, TileDatabase tileDatabaseReference)
        {
            gameReference = game;
            this.shadowTexture = Content.Load<Texture2D>("shadow");
            this.playerReference = playerReference;
            mapDefinition = init;
            debugTextures = mapDebuggingTextures;
            this.roomDatabaseReference = roomDatabaseReference;
            // Generate rooms
            int width = init.Width;
            int height = init.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Room r = new Room(gameReference, x, y, shadowTexture, tileDatabaseReference, playerReference, tileTexture);
                    listOfRooms.Add(r);

#if !KRUSKAL
                    r.Initialize();
#else
                    kruskalRooms.Add(r);
                    
                    // In Kruskal's, each room has a set of their own.
                    List<Room> list = new List<Room>();
                    list.Add(r);
                    sets.Add(list);
#endif

                }
            }

            // Generate maze
            int destination_x = random.Next(width);
            int destination_y = random.Next(height);
            endLocation.X = destination_x;
            endLocation.Y = destination_y;
#if KRUSKAL
            generateMaze();
#else
            generateMaze(destination_x, destination_y, null);
#endif

        }

        public Room getRoom(int x, int y)
        {
            foreach (Room r in listOfRooms)
            {
                if (r.getLocation().X == x && r.getLocation().Y == y)
                    return r;
            }
            return null;
        }

        public Room getRoom(Vector2 v)
        {
            return getRoom((int)v.X, (int)v.Y);

        }

        private class Edge
        {
            private Room left, right;

            public Edge(Room left, Room right)
            {
                this.left = left;
                this.right = right;
            }

            public Room getLeft() { return left; }
            public Room getRight() { return right; }


            public bool EqualEdges(Edge e)
            {
                return ((e.getLeft() == getLeft() && e.getRight() == getRight()) || (e.getLeft() == getRight() && e.getRight() == getLeft()));
            }
        }

        private class EdgeQueue
        {
            List<Edge> edges;
            public EdgeQueue()
            {
                edges = new List<Edge>();
            }

            public void Add(Edge e)
            {
                edges.Add(e);
            }

            public int Count
            {
                get
                {
                    return edges.Count;
                }
            }

            public Boolean Contains(Edge e)
            {
                foreach (Edge current in edges)
                {
                    if (current.EqualEdges(e))
                        return true;
                }
                return false;
            }

            public Edge Remove(int n)
            {
                Edge e = edges.ElementAt(n);
                edges.Remove(e);
                return e;
            }
        }
#if KRUSKAL
	    public void generateMaze() {
		    EdgeQueue allEdges = new EdgeQueue();
            Random rand = new Random();
		
		    /** This code is supposed to get all the edges from the grid as long as no unordered pair repeats. @Darren **/ 
		    for(int x = 0; x < Room.ROOM_WIDTH; x++){
			    for(int y = 0; y < Room.ROOM_HEIGHT; y++) {
                    for (int dir = Room.LEFT; dir <= Room.DOWN; dir++)
                    {
                        Room source, destination;
                        source = getRoom(x, y);
                        destination = getRoomAt(source, dir);
                        if (source != null && destination != null)
                        {
                            Edge temporary = new Edge(destination, source);
                            if (!allEdges.Contains(temporary))
                            {
                                allEdges.Add(temporary);
                            }
                        }
                    }
			    }
		    }
		
		
		    while(allEdges.Count > 0){
			    /** Current edge being tested @Darren */
			    Edge temp;
			
			    /** Indices of the stack lists, initialized @Darren */
			    int a = -1, b = -1;
			
			    /** take random edge, and remove it from the list @Darren */
			    temp = allEdges.Remove(rand.Next(allEdges.Count));
			
                // Get index of both a and i
			    for(int i = 0; i < sets.Count; i++) {
				    if( sets.ElementAt(i).Contains(temp.getLeft())) // search for the cell in the sets
					    a = i;
				    if( sets.ElementAt(i).Contains(temp.getRight())) // search for the cell in the sets
					    b = i;
			    }
			
			    /* If A and B are in the same set, no point in connecting them because it will create a cycle. 
			     * However, if they are of different sets, intersect the sets and leave one as null, and tear down
			     * the wall. 
			     */
			    if(a != b) 	{
				
				    // sample function only; connect the two cells
				    /** Not sample code anymore. @Darren */
                    setRoomRelation(temp.getRight(), isToTheDirectionOf(temp.getLeft(), temp.getRight()), temp.getLeft()); 
				
				    // Join the sets of the formerly divided cells
                    List<Room> LeftSet = getListThatContains(sets, temp.getLeft());
                    List<Room> RightSet = getListThatContains(sets, temp.getRight());
				    mergeRooms(LeftSet, RightSet);
                    sets.Remove(RightSet);
				    // Null the second set, it is useless now
				    // done in mergeRooms();
				
				    // Add cell connection
				    //edgesListOfMaze.add(temp);
                    // not used in this program
			    }
		    }

            foreach (Room r in listOfRooms)
            {
                r.Initialize(roomDatabaseReference);
            }
	    }


        /// <summary>
        /// Read as, connecting room 'destination' is at the 'dir' of 'source'
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="dir"></param>
        /// <param name="source"></param>
        private void setRoomRelation(Room destination, int dir, Room source)
        {
            switch (dir)
            {
                case Room.UP:
                    source.up = true;
                    destination.down = true;
                    break;
                case Room.DOWN:
                    source.down = true;
                    destination.up = true;
                    break;
                case Room.LEFT:
                    source.left = true;
                    destination.right = true;
                    break;
                case Room.RIGHT:
                    source.right = true;
                    destination.left = true;
                    break;
            }
        }
        

        private List<Room> getListThatContains(List<List<Room>> bigList, Room r)
        {
            foreach (List<Room> currentList in bigList)
            {
                if (currentList.Contains(r))
                    return currentList;
            }
            return null;
        }

        /// <summary>
        /// Adds all the elements of source into destination and clears
        /// the elements in source.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        private void mergeRooms(List<Room> destination, List<Room> source)
        {
            destination.AddRange(source);
            source.Clear();
        }

#else
        // DFS Maze creation, fixed.
        // TODONT: Add corners for map generation.
        // Deprecated
        /**
         * 22, 22, 22, 22, 12% (deadend) chance.
         */
        private void generateMaze(int x, int y, Room prev)
        {
            int roomType, roomRotation;
            Room current = getRoom(x, y);

            /**
             * BASE CASE END OF RECURSIVE ALGORITHM
             * 
             * This checks if the room has already been
             * explored or if the room is out of bounds
             * from the map.
             */
            if (exploredRooms.Contains(current))
                return;

            if (current == null)
                return;

            /**
             * ROOM TYPE, ROTATION, DESIGN
             * 
             * This handles the room generation with
             * regards to room type, rotation, and design
             */

            do
            {
                if (prev == null)
                    roomRotation = random.Next(4);
                else
                    roomRotation = (prev.getRotation() + 2) % 4;
                current.setRotation(roomRotation);
                roomType = random.Next(4) + 2;
                current.setRoomType(roomType);
            } while (!areRoomsConnected(isToTheDirectionOf(current, prev), prev, current) || getAccessibleRooms(current).Count <= 0);

            int[,] roomDesign = roomDatabaseReference.getRoomDesign(roomType, roomRotation);
            if (roomDesign != null)
            {
                current.setRoomDesign(roomDesign);
                current.Initialize();
            }
            else
            {
                // TODONT: Must use room designer java application to create corners.
                // Deprecated. Corner rooms are done already.
                throw new Exception("Undefined Room Design Exception!");
            }


            /**
             * MAZE CREATION
             * 
             * Start the DFS creation of the maze
             */

            // Add the current room to list of explored rooms
            exploredRooms.Add(current);

            List<Room> roomsAvailable = getAccessibleRooms(current);

            do
            {
                // Get a random room from the list and remove it from the list
                int index = random.Next(roomsAvailable.Count);
                Room roomToVenture = roomsAvailable.ElementAt(index);
                roomsAvailable.RemoveAt(index);

                // The room is already explored, don't go set it anymore
                if (exploredRooms.Contains(roomToVenture))
                    continue;

                // There is no room to venture to, don't go there
                if (roomToVenture == null)
                    continue;

                // Determine the direction to take and the coordinate of the target room
                int directionToTake = isToTheDirectionOf(current, roomToVenture);
                Vector2 destinationCoordinate = getPointAt(current, directionToTake);

                // Recurse -- DEPRECATED
                generateMaze((int)destinationCoordinate.X, (int)destinationCoordinate.Y, current);

            } while (roomsAvailable.Count > 0);
        }
        //*/
#endif
        /// <summary>
        /// This method is read as "To the DIR of SOURCE is DESTINATION".
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private Boolean areRoomsConnected(int dir, Room source, Room destination)
        {
            // First Room is always conected
            if (source == null)
                return true;
            return isDoorEnabled(source, dir) && isDoorEnabled(destination, (dir + 2) % 4);
        }

        /// <summary>
        /// Given the current room, this function checks if current room has the door 
        /// opened at the direction chosen.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private Boolean isDoorEnabled(Room current, int dir)
        {
            // No door required, no problem
            if (dir == NO_DOOR_REQUIRED)
                return true;

            // Intersections has all rooms open, no problem

            if (current.getRoomType() == Room.INTERSECTION)
                return true;

            // Only the previous rotation was open.
            if (current.getRoomType() == Room.DEAD_END)
            {
                return dir == current.getRotation();
            }

            // Given the rotation of the room, only the opposite is not(!) allowed.
            if (current.getRoomType() == Room.TRIAD)
            {
                int rotation = current.getRotation() + 2; // get the opposite direction
                return !(rotation == (dir + 2) % 4);
            }

            if (current.getRoomType() == Room.CORRIDOR)
            {
                int rotation = current.getRotation();
                return rotation == dir || rotation == ((dir + 2) % 4); // can open the direction of the room, or the opposite.
            }

            if (current.getRoomType() == Room.CORNER)
            {
                int rotation = current.getRotation();
                return rotation == dir || rotation == ((dir + 1) % 4); // can open the direction of the room, or the next direction.
            }
            return false;
        }

        /// <summary>
        /// This method returns the direction that you need to open
        /// for the new room depending on the previous room.
        /// </summary>
        /// <param name="now">The current room</param>
        /// <param name="prev">The previous room</param>
        /// <returns>the direction of the door required to be open from the new room</returns>
        private int isToTheDirectionOf(Room now, Room prev)
        {
            if (prev == null)
                return NO_DOOR_REQUIRED;
            if (now.location.X > prev.location.X)
                return Room.LEFT;
            else if (now.location.X < prev.location.X)
                return Room.RIGHT;
            if (now.location.Y > prev.location.Y)
                return Room.UP;
            else if (now.location.Y < prev.location.Y)
                return Room.DOWN;
            return NO_DOOR_REQUIRED;
        }
        
        public List<Room> getAccessibleRooms(Room current)
        {
            List<Room> listOfAccessibleRooms = new List<Room>();
            Vector2 location = current.location;
            int rotation = current.getRotation();

            // Intersection
            if (current.getRoomType() == Room.INTERSECTION)
            {
                for (int dir = Room.LEFT; dir <= Room.DOWN; dir++)
                {
                    Room r = getRoomAt(current, dir);
                    if (r != null)
                        listOfAccessibleRooms.Add(r);
                }
            }

            // Triad
            else if (current.getRoomType() == Room.TRIAD)
            {
                for (int offset = -1; offset <= 1; offset++)
                {
                    int dirToBeChecked = (rotation + offset) % 4;
                    Room r = getRoomAt(current, dirToBeChecked);
                    if (r != null)
                        listOfAccessibleRooms.Add(r);
                }
            }

            // Corridor
            else if (current.getRoomType() == Room.CORRIDOR)
            {
                for (int offset = 0; offset <= 2; offset += 2)
                {
                    int dirTobeChecked = (rotation + offset) % 4;
                    Room r = getRoomAt(current, dirTobeChecked);
                    if (r != null)
                        listOfAccessibleRooms.Add(r);

                }
            }

            // Deadend
            else if (current.getRoomType() == Room.DEAD_END)
            {
                listOfAccessibleRooms.Add(getRoomAt(current, rotation));
            }

            return listOfAccessibleRooms;
        }

        public Room getRoomAt(Room r, int dir)
        {
            if (r == null)
                return null;
            Vector2 v = getPointAt(r, dir);
            return getRoom((int)v.X, (int)v.Y);
        }

        public Vector2 getPointAt(Room r, int dir)
        {
            Vector2 v = r.location;
            switch (dir)
            {
                case Room.LEFT: return new Vector2(v.X - 1, v.Y);
                case Room.UP: return new Vector2(v.X, v.Y - 1);
                case Room.RIGHT: return new Vector2(v.X + 1, v.Y);
                case Room.DOWN: return new Vector2(v.X, v.Y + 1);
            }
            return v;
        }

        /**
         * Debug purposes
         */
        public void Draw(SpriteBatch spriteBatch, Room currentRoom)
        {
            if (DEBUG)
            {
                int offset = 150;
                int cellDistance = 32;
                for (int y = 0; y < mapDefinition.Height; y++)
                {
                    for (int x = 0; x < mapDefinition.Width; x++)
                    {
                        Rectangle destination = new Rectangle(offset + x * cellDistance, offset + y * cellDistance, 32, 32);
                        Room current = getRoom(x, y);
                        if (current.getRoomType() == 0)
                            continue;
                        //Vector2 position = new Vector2(offset + x * cellDistance, offset + y * cellDistance);
                        Texture2D selectedTexture = getDebugTexture(current, debugTextures);
                        float rotation = getDebugRotation(current);
                        Color color = getCurrentRoomColor(x, y, currentRoom);
                        spriteBatch.Draw(selectedTexture, destination, null, color, rotation, new Vector2(1.5f, 1.5f), SpriteEffects.None, 0);
                        /*
                        spriteBatch.DrawString(spriteFont, getRoom(x,y).getRotation().ToString(), position, Color.Black);
                        spriteBatch.DrawString(spriteFont, "Start- " + endLocation.X + ":" + endLocation.Y, new Vector2(10, 10), Color.Black);
                         */
                    }
                }
            }
        }

        private Color getCurrentRoomColor(int x, int y, Room currentRoom)
        {
            if (currentRoom.getLocation().X == x && currentRoom.getLocation().Y == y)
                return Color.Yellow;
            return Color.Brown;
        }

        private float getDebugRotation(Room r)
        {
            if (r.getRotation() == 1)
                return MathHelper.ToRadians(90);
            if (r.getRotation() == 2)
                return MathHelper.ToRadians(180);
            if (r.getRotation() == 3)
                return MathHelper.ToRadians(270);
            return MathHelper.ToRadians(0);

        }

        private Texture2D getDebugTexture(Room r, List<Texture2D> textures)
        {
            if (r.getRoomType() == Room.DEAD_END)
                return textures.ElementAt(0);
            if (r.getRoomType() == Room.CORRIDOR)
                return textures.ElementAt(1);
            if (r.getRoomType() == Room.TRIAD)
                return textures.ElementAt(2);
            if (r.getRoomType() == Room.INTERSECTION)
                return textures.ElementAt(3);
            if (r.getRoomType() == Room.CORNER)
                return textures.ElementAt(4);
            return null;
        }

        public void setFont(SpriteFont sf)
        {
            spriteFont = sf;
        }

        public Color getColor(Room r)
        {
            switch (r.getRoomType())
            {
                case Room.CORRIDOR: return Color.Yellow;
                case Room.DEAD_END: return Color.Red;
                case Room.INTERSECTION: return Color.Blue;
                case Room.TRIAD: return Color.Green;
            }
            return Color.White;
        }

    }
}
