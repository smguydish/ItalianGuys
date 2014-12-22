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
using xTile;
using xTile.Display;

namespace ItalianGuys
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ParticleManager particleManager;

        Map map;
        IDisplayDevice mapDisplayDevice;
        //xTile.Dimensions.Rectangle viewport;

        Texture2D mario;
        Player player;
        Enemy enemy;

        List<Enemy> enemies = new List<Enemy>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = Content.Load<Map>(@"Levels\Level1");

            mapDisplayDevice = new XnaDisplayDevice(this.Content, this.GraphicsDevice);

            map.LoadTileSheets(mapDisplayDevice);
            xTile.Layers.Layer layer = map.GetLayer("Foreground");

            World.viewport = new xTile.Dimensions.Rectangle(new xTile.Dimensions.Size(800, 600));

            particleManager = new ParticleManager(map);
            particleManager.LoadContent(Content);

            mario = Content.Load<Texture2D>("tiles");
            player = new Player(this.Content, new Vector2(350, this.Window.ClientBounds.Height - (48 * 2) - 48), Vector2.Zero, map, particleManager);

            xTile.Layers.Layer elayer = map.GetLayer("Enemy");
            elayer.Visible = false;
            for (int x = 0; x < elayer.LayerWidth; x++)
            {
                for (int y = 0; y < elayer.LayerHeight; y++)
                {
                    xTile.Tiles.Tile tile = elayer.Tiles[x,y];

                    if (tile != null)
                    {
                        if (tile.Properties.Keys.Contains("type"))
                        {
                            Vector2 vel = Vector2.Zero;

                            if (tile.Properties.Keys.Contains("velocity")) 
                                vel = new Vector2(90 * tile.Properties["velocity"], 0);

                            int x_left = -1, x_right = -1;

                            if (tile.Properties.Keys.Contains("x_left"))
                                x_left = tile.Properties["x_left"];

                            if (tile.Properties.Keys.Contains("x_right"))
                                x_right = tile.Properties["x_right"];

                            if (tile.Properties.Keys.Contains("type") && tile.Properties["type"] == "goomba")
                            {
                                enemies.Add(new Enemy(this.Content, new Vector2(x*48, y*48), vel, x_left, x_right, map));
                            }

                            

                            
                        }
                    }
                }
            }

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            map.Update(gameTime.ElapsedGameTime.Milliseconds);
            player.Update(gameTime);
            particleManager.Update(gameTime);

            for (int i = 0; i < enemies.Count; i++)
            {
                if (Vector2.Distance(player.Center, enemies[i].Center) < World.viewport.Width)
                {
                    enemies[i].Update(gameTime);
                    if (enemies[i].IsBoxColliding(player.BoundingBoxRect) && !enemies[i].Dead)
                    {
                        if (player.onGround && !player.Invulnerable)
                        {
                            if (player.isBig)
                            {
                                player.isBig = false;
                                player.MakeInvulnerable(1000);
                            }
                            else
                                player.Die();
                        }
                        else if (!player.Invulnerable && !player.Dead)
                        {
                            enemies[i].Die();
                            player.Jump();
                        }
                    }
                }
            }
            //World.viewport.Y++;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            map.Draw(mapDisplayDevice, World.viewport);

            spriteBatch.Begin();
            player.Draw(spriteBatch);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            particleManager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
