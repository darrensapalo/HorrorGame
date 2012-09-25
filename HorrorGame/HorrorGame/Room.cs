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
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace HorrorGame
{
    /// <summary>
    /// This is the representation of a room in the maze.
    /// </summary>
    public class Room : Microsoft.Xna.Framework.GameComponent
    {

        /********************************
        * Attributes and constants
        ********************************/

        /// <summary>
        /// This is the room type which ranges from 1 to 4, representing
        /// the number of doors available.
        /// 1 : Room.DEAD_END
        /// 2 : Room.CORRIDOR
        /// 3 : Room.TRIAD
        /// 4 : Room.INTERSECTION
        /// </summary>
        private int roomType;

        /// <summary>
        /// This is the design of the room which holds the 2D int [,] array tile design
        /// </summary>
        private int[,] roomDesign = new int[20, 15];

        /// <summary>
        /// This is the coordinates of the room according to the map.
        /// </summary>
        public Vector2 location;

        /// <summary>
        /// This is the rotation of the room, which affects the orientation
        /// of the room's doors. It ranges from the static variables LEFT, RIGHT, UP, DOWN.
        /// The direction specified determines one sure instance of a door.
        /// 
        /// e.g. if the rotation is UP then it is sure that there is a door
        /// available upwards.
        /// </summary>
        private int rotation;

        public float lightIntensity = 0;

        /// <summary>
        /// This represents the room is facing left, with a door available in that direction.
        /// </summary>
        public const int LEFT = 0;
        /// <summary>
        /// This represents the room is facing up, with a door available in that direction.
        /// </summary>
        public const int UP = 1;
        /// <summary>
        /// This represents the room is facing right, with a door available in that direction.
        /// </summary>
        public const int RIGHT = 2;
        /// <summary>
        /// This represents the room is facing down, with a door available in that direction.
        /// </summary>
        public const int DOWN = 3;

        /// <summary>
        /// This represents a room type of a dead end, which has only one (1) door available.
        /// </summary>
        public const int DEAD_END = 1;

        /// <summary>
        /// This represents a room type of a corridor, which has only two (2) door available.
        /// </summary>
        public const int CORRIDOR = 2;

        /// <summary>
        /// This represents a room type of a triad, which has only three (3) door available.
        /// </summary>
        public const int TRIAD = 3;

        /// <summary>
        /// This represents a room type of a intersection, which has only four (4) door available.
        /// </summary>
        public const int INTERSECTION = 4;

        /// <summary>
        /// This represents a room type of a corner, which has only two (2) door available.
        /// </summary>
        public const int CORNER = 5;


        /// <summary>
        /// This value holds the width of the room. 
        /// Currently it is set to 20 for hard-code testing purposes.
        /// Author: Jon
        /// </summary>
        public const int ROOM_WIDTH = 20; // Jon used 20 for some reason.

        /// <summary>
        /// This value holds the height of the room. 
        /// Currently it is set to 15 for hard-code testing purposes.
        /// Author: Jon
        /// </summary>
        public const int ROOM_HEIGHT = 15; // Jon used 15 for some reason.

        /// <summary>
        /// This constant is used for undefined tilesDesigns, roomTypes, and rotations.
        /// </summary>
        public const int UNDEFINED = -1;

        /// <summary>
        /// This constant signifies that the tile design at a certain coordinate in a Room is a path. 
        /// Author: Jon
        /// </summary>
        public const int PATH = 0; // Jon used 0 to designate walkable paths.

        /// <summary>
        /// This holds the tileDesigns that are regarded as doors.
        /// </summary>
        public static int[] southDoorTiles = { 380, 384, 388, 392, 393 };
        public static int[] eastDoorTiles =  { 381, 385, 389, 394, 395 };
        public static int[] westDoorTiles =  { 382, 386, 390, 397, 396 };
        public static int[] northDoorTiles = { 383, 387, 391, 398, 399 };
        
        /// <summary>
        /// This holds the tiles for the walls.
        /// </summary>
        public static int[] wallTiles = { 3 };

        /// <summary>
        /// This holds the tiles for the walls.
        /// </summary>
        public static int[] shadowHandlerTiles = {0, 20 };

        private List<Entity> listOfEntities = new List<Entity>();

        private const int NUMBER_OF_ITEM_SPAWNS = 1;
        private const int NUMBER_OF_ENTITY_SPAWNS = 4;

        private const int DOOR_INDEX_START = 380;
        private const int DOOR_INDEX_END = 400;

        private List<Rectangle> wallHitTests = new List<Rectangle>();
        private List<ShadowHandler> shadowHandlers = new List<ShadowHandler>();

        private List<Rectangle> hidingSpots = new List<Rectangle>();
        private List<Door> doorList = new List<Door>();

        public Boolean up, down, left, right;

        

        // Sadako Variables
        public Vector2 sadakoPositionInRoom;
        public float roomPresence;

        /// <summary>
        /// This holds the size of the tile.
        /// </summary>
        public const int TILE_SIZE = 32;

        public const float ROOM_ITEM_SPAWN_CHANCE = 0.30F;
        
        private Game gameReference;
        private TileDatabase tileDatabaseReference;
        private Player playerReference;

        private Texture2D shadowTexture;
        private Texture2D tileTexture;

        private List<Item> itemsSpawned = new List<Item>();
        private List<Vector2> roomSpawns = new List<Vector2>();

        private float flashlightFamiliarity = 1F;
        private int durationInTheRoom = 0;

        private int roomBrightnessType;
        public const int BRIGHT = 0;
        public const int DARKER = 1;
        public const int REGULAR = 2;


        /****************
         * Methods
         ****************/

        public Room(Game game, int X, int Y, Texture2D shadowTexture, TileDatabase tileDatabaseReference, Player playerReference, Texture2D tileTexture)
            : base(game)
        {
            gameReference = game;
            location.X = X;
            location.Y = Y;
            this.shadowTexture = shadowTexture;
            this.playerReference = playerReference;
            this.tileDatabaseReference = tileDatabaseReference;
            this.tileTexture = tileTexture;
            Random r = new Random();
            roomBrightnessType = r.Next(10);
            if (roomBrightnessType >= 4)
                roomBrightnessType = REGULAR;
            else if (roomBrightnessType >= 2)
                roomBrightnessType = DARKER;
            else
                roomBrightnessType = BRIGHT;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void Initialize(RoomDatabase rdb)
        {
            initTypeAndRotation();
            initRoomDesign(rdb);

            initShadowHandlers();
            initWallHitTests();
            initDoors();
            initEntities();
            initItems();
            base.Initialize();
        }

        private void initItems()
        {
            Random rand = new Random();
            // TODO: Increase spawn rate of items when dead end type
            if (rand.NextDouble() < ROOM_ITEM_SPAWN_CHANCE || roomType == DEAD_END)
            {
                for (int i = 0; i < NUMBER_OF_ITEM_SPAWNS + ( ( roomType == DEAD_END ) ? 1 : 0 ) ; i++)
                {
                    Random random = new Random();
                    int type = 1; //random.Next(7);
                    ItemType p = 0;
                    switch(type){
                        case 0: p = ItemType.Amulet; break;
                        case 1: p = ItemType.HealingSalve; break;
                        case 2: p = ItemType.Incense; break;
                        case 3: p = ItemType.Key; break;
                        case 4: p = ItemType.Lamp; break;
                        case 5: p = ItemType.MapPiece; break;
                        case 6: p = ItemType.Mirror; break;
                    }
                    int x, y;
                    x = y = -1;
                
                    // Keep selecting x,y location until it is not yet used.
                    do
                    {
                        x = random.Next(20);
                        y = random.Next(15);

                    } while (isDoor(roomDesign[x, y]) || isWall(roomDesign[x, y]) || roomEntitiesLocationIsUsed((x +1) * 32, y * 32, roomSpawns, 2));
                    
                    x *= 32; y *= 32;
                    roomSpawns.Add(new Vector2(x, y));
                    Rectangle sourceRectangle = new Rectangle(x, y, TILE_SIZE, TILE_SIZE);
                    Item newItem = new Item(p, playerReference, playerReference.sadako, sourceRectangle);
                    itemsSpawned.Add(newItem);
                }
            }
        }

        /// <summary>
        /// This method initializes the room's design by randomly selecting
        /// from a list of valid room designs (e.g. a list of all TRIADs facing UP)
        /// </summary>
        /// <param name="rdb">The database that contains all room designs</param>
        private void initRoomDesign(RoomDatabase rdb)
        {
            int[,] roomDesign = rdb.getRoomDesign(roomType, rotation);
            if (roomDesign != null)
            {
                setRoomDesign(roomDesign);
            }
            else
            {
                throw new Exception("Null Room Design Exception! No room design for type: " + Room.getRoomTypeString(roomType) + " and rotation: " + Room.getRoomRotationString(rotation));
            }
        }

        public static String getRoomTypeString(int type)
        {
            switch (type)
            {
                case Room.CORNER: return "corner"; 
                case Room.CORRIDOR: return "corridor"; 
                case Room.DEAD_END: return "dead end"; 
                case Room.TRIAD: return "triad"; 
                case Room.INTERSECTION: return "intersection"; 
            }
            return "Unknown room type";
        }

        public static String getRoomRotationString(int rot)
        {
            switch (rot)
            {
                case Room.LEFT: return "left"; 
                case Room.UP: return "up"; 
                case Room.RIGHT: return "right"; 
                case Room.DOWN: return "down"; 
            }
            return "Unknown rotation";
        }

        private Boolean isEntityAllowed(int entityType, int x, int y)
        {
            switch(entityType)
            {
                case Entity.CABINET:
                    return (x >= 1 && x < 19 && y >= 1 && y < 14 &&
                        isWallForEntity(roomDesign[x, y]) &&
                        isWallForEntity(roomDesign[x + 1, y]) &&
                        isFloor(roomDesign[x, y + 1]) &&
                        isFloor(roomDesign[x + 1, y + 1]));
                case Entity.CURTAIN:
                    return (x >= 0 && x < 19 && y >= 0 && y < 14 &&
                        isWallForEntity(roomDesign[x, y]) &&
                        isWallForEntity(roomDesign[x + 1, y]) &&
                        isWallForEntity(roomDesign[x, y + 1]) &&
                        isWallForEntity(roomDesign[x + 1, y + 1]));
                case Entity.BED:
                    return (x >= 0 && x < 19 && y >= 0 && y < 14 &&
                        isFloor(roomDesign[x, y]) &&
                        isFloor(roomDesign[x + 1, y]) &&
                        isFloor(roomDesign[x, y + 1]) &&
                        isFloor(roomDesign[x + 1, y + 1]));
                case Entity.ITEM_SPAWNER:
                    return (x >= 0 && x < 19 && y >= 0 && y < 14 &&
                        isFloor(roomDesign[x, y]) &&
                        isFloor(roomDesign[x + 1, y]) &&
                        isFloor(roomDesign[x, y + 1]) &&
                        isFloor(roomDesign[x + 1, y + 1]));
            }
            return false;
        }
        
        private void initEntities()
        {
            List<Entity> entities = new List<Entity>();
            Random r = new Random();
            Entity current = null;
            Boolean allowed = false;
            int entityType;
            int x, y;

            for (int i = 0; i < NUMBER_OF_ENTITY_SPAWNS; i++)
            {
                // random entity type
                //entityType = r.Next(6) + 1;
                

                // random entity location
                Vector2 location = new Vector2();
                
                do
                {
                    entityType = r.Next(5);
                    x = r.Next(20);
                    y = r.Next(15);

                    location.X = x * 32;
                    location.Y = y * 32;
                    
                    // Hard code for cabinets
                    if (isEntityAllowed(entityType, x, y))
                    {
                        allowed = true;
                    }
                    else
                    {
                        allowed = false;
                    }
                } while (!allowed || roomEntitiesLocationIsUsed(x, y, roomSpawns, 2));

                /* This location now has a special random object in there. Do not spawn
                 * other objects such as other hiding spots or items.
                 */

                roomSpawns.Add(new Vector2(x, y));
                if (entityType != Entity.ITEM_SPAWNER && entityType != Entity.GENERIC)
                {
                    roomSpawns.Add(new Vector2(x + 1, y));
                    roomSpawns.Add(new Vector2(x, y + 1));
                    roomSpawns.Add(new Vector2(x + 1, y + 1));
                }
                
                // instantiate
                switch (entityType)
                {
                    case Entity.CABINET: current = new Cabinet("Cabinet", location); break;
                    case Entity.BED: current = new Bed("Bed", location); break;
                    case Entity.ITEM_SPAWNER: current = new ItemSpawnEntity("Chest", location); break;
                    case Entity.CURTAIN: current = new Curtain("Curtain", location); break;
                    case Entity.GENERIC: current = new Entity("Generic", location, Entity.SMALL); break;
                }

                // add to list
                listOfEntities.Add(current);

                // reset
                allowed = false;
                x = y = 0;
            }
        }

        private Boolean roomEntitiesLocationIsUsed(int x, int y, List<Vector2> list, int size)
        {
            if (size == 2)
                return roomEntitiesLocationIsUsed(x, y, list, 0) || roomEntitiesLocationIsUsed(x + 1, y, list, 0) ||
                    roomEntitiesLocationIsUsed(x, y + 1, list, 0) || roomEntitiesLocationIsUsed(x + 1, y + 1, list, 0);

            foreach (Vector2 b in list)
                if (b.X == x && b.Y == y)
                    return true;

            return false;
        }

        /// <summary>
        /// This method prepares the room type and rotation depending on the tiles
        /// that contains doors specified in the room design.
        /// </summary>
        private void initTypeAndRotation()
        {
            if (up && down && left && right)
            {
                roomType = Room.INTERSECTION;
                rotation = Room.LEFT;
            }
            else

                // Triads
                if (up && down && left)
                {
                    roomType = Room.TRIAD;
                    rotation = Room.LEFT;
                }
                else
                    if (up && down && right)
                    {
                        roomType = Room.TRIAD;
                        rotation = Room.RIGHT;
                    }
                    else
                        if (left && right && up)
                        {
                            roomType = Room.TRIAD;
                            rotation = Room.UP;
                        }
                        else
                            if (left && right && down)
                            {
                                roomType = Room.TRIAD;
                                rotation = Room.DOWN;
                            }
                            else

                                // Corridor
                                if (up && down)
                                {
                                    roomType = Room.CORRIDOR;
                                    rotation = Room.UP;
                                }
                                else

                                    if (left && right)
                                    {
                                        roomType = Room.CORRIDOR;
                                        rotation = Room.LEFT;
                                    }
                                    else
                                        // corners
                                        if (left && up)
                                        {
                                            roomType = Room.CORNER;
                                            rotation = Room.LEFT;
                                        }
                                        else
                                            if (up && right)
                                            {
                                                roomType = Room.CORNER;
                                                rotation = Room.UP;
                                            }
                                            else
                                                if (right && down)
                                                {
                                                    roomType = Room.CORNER;
                                                    rotation = Room.RIGHT;
                                                }
                                                else
                                                    if (down && left)
                                                    {
                                                        roomType = Room.CORNER;
                                                        rotation = Room.DOWN;
                                                    }
                                                        // dead ends
                                                    else
                                                        if (left)
                                                        {
                                                            roomType = Room.DEAD_END;
                                                            rotation = Room.LEFT;
                                                        }
                                                        else
                                                            if (up)
                                                            {
                                                                roomType = Room.DEAD_END;
                                                                rotation = Room.UP;
                                                            }
                                                            else
                                                                if (right)
                                                                {
                                                                    roomType = Room.DEAD_END;
                                                                    rotation = Room.RIGHT;
                                                                }
                                                                else
                                                                    if (down)
                                                                    {
                                                                        roomType = Room.DEAD_END;
                                                                        rotation = Room.DOWN;
                                                                    }
            if (roomType == 0){
                throw new Exception("Invalid Room Type Exception!");
            }
        }

        private Boolean isFloor(int design)
        {
            return design >= 0 && design < 200 && design != 20 && design != 0;
        }

        /// <summary>
        /// This method creates a List of Vector2 that represents all the
        /// doors in the room.
        /// </summary>
        private void initDoors()
        {
            for (int y = 0; y < ROOM_HEIGHT; y++)
            {
                for (int x = 0; x < ROOM_WIDTH; x++)
                {
                    /*
                     * Old Code
                    if (doorTiles.Contains(roomDesign[x, y]))
                        doorList.Add(new Vector2(x, y));
                     */
                    int roomDesign = this.roomDesign[x, y];
                    if (roomDesign >= DOOR_INDEX_START && roomDesign <= DOOR_INDEX_END)
                    {
                        Rectangle locationOfDoor = new Rectangle(x * 32, y * 32, 32, 32);
                        int directionOfDoor = getDoorDirection(roomDesign);
                        Door newDoor = new Door(locationOfDoor, directionOfDoor);
                        doorList.Add(newDoor);
                    }
                }
            }
        }

        private int getDoorDirection(int design)
        {
            if (northDoorTiles.Contains(design))
                return Room.UP;
            else if (eastDoorTiles.Contains(design))
                return Room.RIGHT;
            else if (westDoorTiles.Contains(design))
                return Room.LEFT;
            else if (southDoorTiles.Contains(design))
                return Room.DOWN;
            throw new Exception("Incorrect Door rotation!");
        }

        public static Boolean isDoor(int design)
        {
            return eastDoorTiles.Contains(design) ||
                northDoorTiles.Contains(design) ||
                westDoorTiles.Contains(design) ||
                southDoorTiles.Contains(design);
        }

        public static int typeOfDoor(int design)
        {
            if (eastDoorTiles.Contains(design))
                return Room.RIGHT;
            if (northDoorTiles.Contains(design))
                return Room.UP;
            if (southDoorTiles.Contains(design))
                return Room.DOWN;
            if (westDoorTiles.Contains(design))
                return Room.LEFT;
            return Room.UNDEFINED;
        }

        /// </summary>
        /// <param name="dir">The direction you are coming from</param>
        /// <returns></returns>
        public Vector2 getLocationStartFrom(int dir)
        {
            List<Vector2> candidateLocations = new List<Vector2>();
            Vector2 result = new Vector2();
            float amount = 0;
            foreach (Door door in doorList)
            {
                // If you are going 'down', then get all the 'up' doors.
                if (door.getDirection() == (dir + 2) % 4)
                {
                    // Get the destinations of the doors.
                    Vector2 v = new Vector2(door.getDestination().X, door.getDestination().Y);
                    result = Vector2.Add(v, result);
                    amount += 1;
                }
            }

            result.X /= amount;
            result.Y /= amount;
            return result;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (Entity e in listOfEntities)
            {
                if (e is HidingSpot)
                    ((HidingSpot)e).Update(gameTime, playerReference);
                else
                    e.Update(gameTime, playerReference);
            }
            durationInTheRoom += gameTime.ElapsedGameTime.Milliseconds;
            if (durationInTheRoom > 33)
            {
                if (flashlightFamiliarity >= 0.94F)
                {
                    flashlightFamiliarity -= 0.0003F;
                    flashlightFamiliarity = MathHelper.Clamp(flashlightFamiliarity, 0.94F, 1F);
                }
            }

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, TileDatabase tdb)
        {
            for (int x = 0; x < ROOM_WIDTH; x++)
            {
                for (int y = 0; y < ROOM_HEIGHT; y++)
                {
                    //Automatically Draws the tile given the position and tile kind
                    tdb.drawTile(spriteBatch,new Vector2(x * TILE_SIZE, y * TILE_SIZE),roomDesign[x, y]);
                }
            }
            
            // Entities
            foreach (Entity e in listOfEntities)
            {
                e.Draw(spriteBatch);
            }

            // Items
            foreach (Item i in itemsSpawned)
            {
                i.Draw(spriteBatch);
            }


        }

        /// <summary>
        /// Given the selected room type and the list of possible textures to select from,
        /// return the correct room texture for map debugging purposes.
        /// This returns the 3x3 Texture2D content (later stretched) that is used in
        /// displaying the whole map.
        /// </summary>
        /// <param name="roomType">The room type</param>
        /// <param name="textures">The list of textures to select from</param>
        /// <returns></returns>
        private Texture2D getTextureFromRoomType(int roomType, List<Texture2D> textures)
        {
            switch (roomType)
            {
                case Room.DEAD_END: return textures.ElementAt(0);
                case Room.CORRIDOR: return textures.ElementAt(1);
                case Room.TRIAD: return textures.ElementAt(2);
                case Room.INTERSECTION: return textures.ElementAt(3);
                case Room.CORNER: return textures.ElementAt(4);
            }
            return null;
        }

        /// <summary>
        /// This method creates a list of rectangles 'wallHitTests' which holds all
        /// the walls in the room.
        /// </summary>
        public void initWallHitTests()
        {
            for (int y = 0; y < Room.ROOM_HEIGHT; y++)
            {
                for (int x = 0; x < Room.ROOM_WIDTH; x++)
                {
                    int design = roomDesign[x, y];
                    //if (wallTiles.Contains(design))
                    if (isWall(design))
                    {
                        Rectangle wallRectangle = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                        wallHitTests.Add(wallRectangle);
                    }
                }
            }
        }

        private Boolean isWall(int design)
        {
            return isWallForEntity(design) || design == 0;
        }

        private Boolean isWallForEntity(int design)
        {
            return design >= 200 && design <= 300;
        }

        /// <summary>
        /// This method creates a list of rectangles 'shadowHandlers' which holds
        /// all the walls that should display shadows.
        /// </summary>
        public void initShadowHandlers()
        {
            for (int y = 0; y < Room.ROOM_HEIGHT; y++)
            {
                for (int x = 0; x < Room.ROOM_WIDTH; x++)
                {
                    int design = roomDesign[x, y];
                    if (shadowHandlerTiles.Contains(design))
                    {
                        Vector2 position = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
                        Texture2D supposedTileTexture;
                            supposedTileTexture = tileTexture;
                        ShadowHandler shadowHandler = new ShadowHandler(shadowTexture, playerReference, position, supposedTileTexture);
                        shadowHandlers.Add(shadowHandler);
                    }
                }
            }
        }

        /*************************
         * Getters and setters
         *************************/

        public void setRotation(int r)
        {
            rotation = r;
        }

        public int getRotation()
        {
            return rotation;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        public void setRoomType(int r)
        {
            roomType = r;
        }

        public int getRoomType()
        {
            return roomType;
        }

        public void setRoomDesign(int[,] design)
        {
            roomDesign = design;
        }

        public List<Door> getDoorList()
        {
            return doorList;
        }

        public List<ShadowHandler> getShadows()
        {
            return shadowHandlers;
        }

        public List<Rectangle> getWalls()
        {
            return wallHitTests;
        }

        public List<Entity> getEntities()
        {
            return listOfEntities;
        }

        public List<Item> getItems()
        {
            return itemsSpawned;
        }

        public void removeItem(Item m)
        {
            if (itemsSpawned.Contains(m))
                itemsSpawned.Remove(m);
        }

        public float getFlashlight()
        {
            switch(roomBrightnessType){
                case REGULAR:
                    return flashlightFamiliarity;
                case DARKER:
                    return 1F;
                case BRIGHT:
                    return 0.90F;
            }
            return 1F;
        }

        public float getShadowLight()
        {
            switch (roomBrightnessType)
            {
                case REGULAR:
                    return 0.50F;
                case DARKER:
                    return 1.00F;
                case BRIGHT:
                    return 0.30F;
            }
            return 1F;
        }
    }
}
