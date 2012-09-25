//#define MAPDEBUG
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
    public class LocationHandler
    {
        private Player player;
        private Sadako sadako;
        public Room sadakoRoom;
        private Vector2 currentLocation;
        public Room currentRoom;
        private Map map;
        SongHandler songs;
        Random rand = new Random();
        Vector2 entryPoint;

        //Chase Variables
        Boolean chaseStart = false;
        Vector2 startLocationViaChase;
        public int chaseDelay = 0;
        int chaseCounter = 0;
        Room roomTransition;

        //lurk
        int lurkDelay = 0;

        EventHandler events;
        
        public LocationHandler()
        {
        }

        public void Initialize(Player playerGet, Sadako sadakoGet, Map mapGet, SongHandler songsGet, EventHandler eventsGet)
        {
            songs = songsGet;
            events = eventsGet;
            player = playerGet;
            sadako = sadakoGet;
            map = mapGet;
            currentLocation = new Vector2(0, 0);
            currentRoom = map.getRoom(currentLocation);
            sadakoRoom = currentRoom;
            sadako.disable = true;
            entryPoint = player.position;
        }

        private void moveSadakoTo(int dir)
        {
            Room checkRoom = map.getRoomAt(currentRoom, dir);
            if (checkRoom != null)
            {
                Vector2 start = checkRoom.getLocationStartFrom(dir);
                if (!start.Equals(Vector2.Zero))
                {

                    sadako.teleport(checkRoom);
                    checkRoom.sadakoPositionInRoom = sadako.position;
                    sadakoRoom = checkRoom;
                    sadako.disable = true;
                }
            }
        }

        private void moveTo(int dir)
        {
            Room checkRoom = map.getRoomAt(currentRoom, dir);
            if (checkRoom != null)
            {
                Vector2 start = checkRoom.getLocationStartFrom(dir);
                if (!start.Equals(Vector2.Zero))
                {
                    player.position = start;
                    player.currentPosition = start;
                    entryPoint = start;
                    Room tempRoom = currentRoom;
                    currentRoom.sadakoPositionInRoom = sadako.position;
                    currentRoom = checkRoom;
                    lurkDelay = 0;
                    if (sadako.disturbed)
                    { 
                        transferRoomViaChase(start, sadako.wayPoints.Count, currentRoom);
                    }
                    else
                    {
                        float difference = player.maxPresence - tempRoom.roomPresence;
                        int threshold = (int)(difference / 1000);

                        int chance = rand.Next(0, threshold);

                        if (chance == 0 && sadakoRoom != currentRoom)
                        {
                            transferSadakoRandomly(currentRoom);
                        }
                        if (chance == 1 && sadakoRoom != tempRoom)
                        {
                            transferSadakoRandomly(tempRoom);
                        }
                    }
                    sadako.wayPoints = new List<Vector2>();
                    sadako.wayPoints.Add(player.position);
                }

                if (currentRoom == sadakoRoom)
                {
                    sadako.disable = false;
                    sadako.position = currentRoom.sadakoPositionInRoom;
                }
                else sadako.disable = true;
            }
        }

        public void transferSadakoRandomly(Room roomTransfer) 
        {
            sadako.teleport(roomTransfer);
            sadakoRoom = roomTransfer;
            sadakoRoom.sadakoPositionInRoom = sadako.position;
            if (currentRoom == sadakoRoom)
            {
                sadako.disable = false;
                sadako.position = currentRoom.sadakoPositionInRoom;
            }
            else sadako.disable = true;
        }

        private void transferRoomViaChase(Vector2 start, int delay, Room currentRoom) 
        {
            startLocationViaChase = start;
            chaseDelay = delay;
            chaseStart = true;
            roomTransition = currentRoom;
        }

        private void roomChaseHandler(GameTime gameTime) 
        {
            if (chaseStart)
            {
                chaseCounter += gameTime.ElapsedGameTime.Milliseconds;
                if (chaseDelay * 300 < chaseCounter)
                {
                    chaseStart = false;
                    chaseCounter = 0;
                    sadako.disable = false;
                    if (sadakoRoom != roomTransition)
                    {
                        sadako.position = startLocationViaChase;
                        sadakoRoom = roomTransition;
                    }
                }
            }
        }

        private void transferSadakoToCurrentRoom() 
        {
            if (entryPoint != null)
            {
                sadako.position = entryPoint;
                currentRoom.sadakoPositionInRoom = sadako.position;
                sadakoRoom = currentRoom;
                sadako.disable = false;
                songs.songGirl.Play();
            }
        }

        int counterDelay = 0;
        public void Update(GameTime gameTime)
        {
            List<Door> listOfDoors = currentRoom.getDoorList();
            foreach (Door currentDoor in listOfDoors)
            {
                Rectangle doorLocation = currentDoor.getLocation();

                // If not yet entered a room, then move to room
                if (player.hitBox.Intersects(doorLocation))
                {
                    moveTo(getRoomDirection(currentDoor));
                break;
                }
            }

            List<Door> listOfDoors2 = sadakoRoom.getDoorList();
            if (sadako.lurking)
            {
                counterDelay += gameTime.ElapsedGameTime.Milliseconds;
                foreach (Door currentDoor in listOfDoors2)
                {
                    Rectangle doorLocation = currentDoor.getLocation();

                    // If not yet entered a room, then move to room
                    if (sadako.hitBox.Intersects(doorLocation) && counterDelay > 50000)
                    {
                        counterDelay = 0;
                        moveSadakoTo(getRoomDirection(currentDoor));
                        break;
                    }
                }
            }

            currentRoom.Update(gameTime);
            roomPresenceHandler(gameTime);
            roomChaseHandler(gameTime);
        }

        int soundCounter = 0;
        private void roomPresenceHandler(GameTime gameTime)
        {

            lurkDelay += gameTime.ElapsedGameTime.Milliseconds;
            soundCounter += gameTime.ElapsedGameTime.Milliseconds;
            if (soundCounter > 10)
            {
                soundCounter = 0;
                if (player.currentSound > player.targetSound) player.currentSound-=2.5F;
                else player.currentSound+=2.5F;
            }
            if (player.maxPresence >= currentRoom.roomPresence)
            {
                if (player.isFlashLightOn)
                {
                    currentRoom.roomPresence += gameTime.ElapsedGameTime.Milliseconds;
                }

                if (player.isMoving)
                {
                    currentRoom.roomPresence += gameTime.ElapsedGameTime.Milliseconds * 2; player.targetSound=20;
                    if (player._speed > 0.1F)
                    {
                        currentRoom.roomPresence += gameTime.ElapsedGameTime.Milliseconds * 3; player.targetSound =40;
                    }
                }
                else player.targetSound =0;
            }

            if (player.isHidden && currentRoom.roomPresence > 0) currentRoom.roomPresence -= gameTime.ElapsedGameTime.Milliseconds * 2;

            foreach (Room room in map.listOfRooms) 
            {
                if(room.roomPresence > 0)
                room.roomPresence -= gameTime.ElapsedGameTime.Milliseconds * 2;
            }

            if (player.currentPresence >= player.maxPresence && currentRoom!= sadakoRoom && lurkDelay > 5000) 
            { 
                transferSadakoToCurrentRoom();
                player.currentPresence -= player.currentPresence / 2;
                if (rand.Next(0, 15) == 5) 
                {
                    events.startFalling();
                    sadako.position = player.position;
                }
            }
            if (player.currentPresence < player.maxPresence) player.currentPresence += currentRoom.roomPresence;


        }

        public void Draw(SpriteBatch spriteBatch, TileDatabase tdb)
        {
            currentRoom.Draw(spriteBatch, tdb);
#if MAPDEBUG
            map.Draw(spriteBatch, currentRoom);
#endif
        }

        public int getRoomDirection(Door d)
        {
            return d.getDirection();
        }

        public Room getCurrentRoom()
        {
            return currentRoom;
        }
    }
}
