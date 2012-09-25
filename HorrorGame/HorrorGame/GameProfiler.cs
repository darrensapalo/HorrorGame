using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Phone.Info;

namespace HorrorGame
{
    public class GameProfiler : DrawableGameComponent
    {
        TimeSpan timeElapsed;
        TimeSpan oneSecond = TimeSpan.FromSeconds(1);

        int frameCount = 0;

        long memCur;

        SpriteFont font;

        SpriteBatch spriteBatch;
        ContentManager content;

        public int FPS
        {
            get;
            private set;
        }

        public GameProfiler(Game game, ContentManager content)
            : base(game)
        {
            this.content = content;
        }

        protected override void LoadContent()
        {
            font = content.Load<SpriteFont>("arial");

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime;

            if (timeElapsed > oneSecond)
            {
                FPS = frameCount;
                frameCount = 0;

                memCur = DeviceStatus.ApplicationCurrentMemoryUsage;

                timeElapsed = TimeSpan.Zero;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            string info = "";

            info += "FPS: " + FPS + "\n";
            info += "MEM: " + (memCur / (1024 * 1024)) + "Mb\n";

            frameCount++;

            spriteBatch.Begin();

            //spriteBatch.DrawString(font, info, new Vector2(10, 100), Color.White);

            spriteBatch.End();
        }
    }
}
