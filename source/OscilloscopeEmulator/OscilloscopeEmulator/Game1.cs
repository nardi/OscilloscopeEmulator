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
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;

namespace OscilloscopeEmulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        WaveIn mic;
        double frameRate = 40;
        bool inverted = false;

        public Game1(bool inv, int width, int height)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = width;
            graphics.PreferredBackBufferWidth = height;
            inverted = inv;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            if (inverted)
                blank.SetData(new[] { Color.Black });
            else
                blank.SetData(new[] { Color.White });

            mic = new WaveIn();
            mic.WaveFormat = new WaveFormat(44100, 2);
            mic.DataAvailable += new EventHandler<WaveInEventArgs>(mic_DataAvailable);
            mic.BufferMilliseconds = (int)(1000 / frameRate);
            mic.StartRecording();

            base.Initialize();
        }

        List<Vector2> frame;
        bool lastBufferUneven = false;
        void mic_DataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = e.Buffer;

            int offset = 0;
            if (lastBufferUneven)
                offset = 2;

            frame = new List<Vector2>();

            for (int samplePair = 0; samplePair < buffer.Length - offset - 3; samplePair += 4)
            {
                var x = -(BitConverter.ToInt16(buffer, samplePair + offset) / 32767f) * (graphics.PreferredBackBufferWidth / 2) + (graphics.PreferredBackBufferWidth / 2);
                var y = (BitConverter.ToInt16(buffer, samplePair + offset + 2) / 32767f) * (graphics.PreferredBackBufferHeight / 2) + (graphics.PreferredBackBufferWidth / 2);
                frame.Add(new Vector2(x, y));
            }

            lastBufferUneven = buffer.Length % 4 != 0 && !lastBufferUneven;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        Texture2D blank;
        
        void DrawLine(SpriteBatch batch, float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);
 
            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (inverted)
                GraphicsDevice.Clear(Color.White);
            else
                GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            Vector2 lastPoint = Vector2.Zero;
            foreach (var point in frame)
            {
                if (point != frame.First())
                {
                    var lineVector = point - lastPoint;
                    var opacity = (float)(1 / Math.Max(1, lineVector.Length()/4));
                    DrawLine(spriteBatch, 2, new Color(1f, 1f, 1f, opacity), lastPoint, point);
                }

                lastPoint = point;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
