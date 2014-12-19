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
        public float gravityTimer = 0f;
        public float gravityTimerMax = 10f;
        public float gravity = 25f;
        public bool onGround = false;


        public Enemy(
           ContentManager content,
           Vector2 location,
           Vector2 velocity,
           xTile.Map map)
            : base(location, velocity, map)
        {
            // mario, new Rectangle(0, 0, 48, 48)

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
                    content.Load<Texture2D>(@"Sprites\Player\Mario"),
                    32,
                    "die",
                    64,
                    1));
            animations["die"].LoopAnimation = false;


            currentAnimation = "run";
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
            xTile.Tiles.Tile tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height + 1),
                                                      new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height + 1)
                                                      );

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
                tile = CollisionEdgeTest(new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1));

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    //location = temploc;
                    this.velocity.X *= -1;
                    this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight) % 48;
                }

                // Left collision test
                tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height - 1));

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    //location = temploc;
                    this.velocity.X *= -1;
                    this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight) % 48;
                }

               

            }

            
        }
    }
}
