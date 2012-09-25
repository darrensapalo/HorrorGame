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
    public class SongHandler
    {
        Song ambient;
        Song chase;
        Song heart;
        public SoundEffect chaseEnd;
        public SoundEffect mummy;
        public SoundEffect presence;
        public SoundEffect psycho;
        public SoundEffect songGirl;
        public SoundEffect stun;
        public Boolean isAmbientPlaying;
        public Boolean isChasePlaying;

        public SongHandler()
        {

        }

        public void Initialize(ContentManager Content)
        {
            Song ambient = Content.Load<Song>("ambient");
            Song chase = Content.Load<Song>("chase");
            SoundEffect chaseEnd = Content.Load<SoundEffect>("chaseEnd");

            Song heart = Content.Load<Song>("heart");
            SoundEffect mummy = Content.Load<SoundEffect>("mummy");
            SoundEffect presence = Content.Load<SoundEffect>("presence");
            SoundEffect psycho = Content.Load<SoundEffect>("psycho");
            SoundEffect songGirl = Content.Load<SoundEffect>("songGirl");
            SoundEffect stun = Content.Load<SoundEffect>("stun");
            PrepareSounds(ambient, chase, chaseEnd, heart, mummy, presence, psycho, songGirl, stun);
        }


        public void PrepareSounds(Song ambientGet, Song chaseGet, SoundEffect chaseEndGet, Song heartGet, SoundEffect mummyGet, SoundEffect presenceGet, SoundEffect psychoGet, SoundEffect songGirlGet, SoundEffect stunGet) 
        {
            heart = heartGet;
            mummy = mummyGet;
            presence = presenceGet;
            psycho = psychoGet;
            songGirl = songGirlGet;
            stun = stunGet;
            ambient = ambientGet;
            chase = chaseGet;
            chaseEnd = chaseEndGet;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.8F;
        }

        public void playAmbient() 
        {
            if (!isAmbientPlaying)
            {
                MediaPlayer.Play(ambient);
                isAmbientPlaying = true;
            }
        }
        public void playChase() 
        {
            if (!isChasePlaying)
            {
                MediaPlayer.Volume = 0.094F;
                MediaPlayer.Play(chase);
                isChasePlaying = true;
            }
        }

        public void switchSongToAmbient() 
        {
            MediaPlayer.Volume = 0.8F;
            MediaPlayer.Play(ambient);
            isAmbientPlaying = true;
            isChasePlaying = false;
        }

        public void stopAllSongs() 
        {
            MediaPlayer.Stop(); 
            isAmbientPlaying = false;
            isChasePlaying = false;
        }
    }
}
