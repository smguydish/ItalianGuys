using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ItalianGuys
{
    class Enemy : Sprite
    {
        public string Name;
        private float gravityTimer = 0f;
        private float gravityTimerMax = 10f;
        private float gravity = 25f;
        public bool onGround = false;

        private int x_left;
        private int x_right;

        public Enemy(
           ContentManager content,
           Vector2 location,
           Vector2 velocity,
           int x_left, // Left x extend
           int x_right,
           xTile.Map map)
            : base(location, velocity, map)
        {
            this.x_left = x_left;
            this.x_right = x_right;

            animations.Add("run",
                              new AnimationStrip(
                                  content.Load<Texture2D>(@"Sprites\Badguys\goomba"),
                                  32,
                                  "run",
                                  0,
                                  2));
            animations["run"].LoopAnimation = true;
            animations["run"].FrameLength = 0.08f;

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Badguys\goomba"),
                    32,
                    "die",
                    65,
                    1));
            animations["die"].LoopAnimation = false;
            animations["die"].FrameLength = 1f;
            animations["die"].NextAnimation = "dead";

            animations.Add("dead",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Badguys\goomba"),
                    0,
                    "dead",
                    0,
                    1));

            currentAnimation = "run";
        }

        public void Die()
        {
            Dead = true;
            currentAnimation = "die";
            this.velocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            gravityTimer += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (gravityTimer > gravityTimerMax && !onGround)
            {
                this.velocity.Y += gravity;
                gravityTimer = 0f;
            }


            // Check collision below
            KeyValuePair<xTile.Tiles.Tile, Vector2> ctest = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height + 1),
                                                      new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height + 1)
                                                      );
            xTile.Tiles.Tile tile = ctest.Key;

            if (tile != null && !tile.Properties.ContainsKey("Passable"))
            {
                if (tile.Properties.Keys.Contains("causeDeath"))
                {
                    if (tile.Properties["causeDeath"])
                    {
                        if (!Dead)
                        {
                            currentAnimation = "die";
                            onGround = false;
                            this.Dead = true;
                        }

                    }
                }

                onGround = true;
                this.velocity.Y = 0;
                //this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight) % 48;
            }
            else
                onGround = false;

            base.Update(gameTime);

            if (!Dead)
            {


                // Right collision test
                ctest = CollisionEdgeTest(new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1));
                tile = ctest.Key;

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    //location = temploc;
                    this.velocity.X *= -1;
                    this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight) % 48;
                }

                // Left collision test
                ctest = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height - 1));
                tile = ctest.Key;

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    //location = temploc;
                    this.velocity.X *= -1;
                    this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight) % 48;
                }

                int x_loc = (int)(this.location.X / 48);
                if ((x_loc < x_left && x_left != -1) || (x_loc >= x_right && x_right != -1))
                {
                    this.velocity.X *= -1;
                    this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight) % 48;

                }

            }

            
        }
    }
}
