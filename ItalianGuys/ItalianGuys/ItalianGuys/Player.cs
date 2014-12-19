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
    class Player : Sprite
    {
        public float gravityTimer = 0f;
        public float gravityTimerMax = 10f;
        public float gravity = 25f;
        public Rectangle WalkableArea = new Rectangle(250, 0, 250, 400);

        public bool onGround = false;
        public bool isBig = true;

        public Player(
           ContentManager content,
           Vector2 location,
           Vector2 velocity,
           xTile.Map map)
            : base(location, velocity, map)
        {
            // mario, new Rectangle(0, 0, 48, 48)

            animations.Add("idle",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\Mario"),
                    30,
                    "idle",
                    139,
                    1));
            animations["idle"].LoopAnimation = true;

            animations.Add("run",
                              new AnimationStrip(
                                  content.Load<Texture2D>(@"Sprites\Player\Mario"),
                                  34,
                                  "run",
                                  0,
                                  2));
            animations["run"].LoopAnimation = true;
            animations["run"].FrameLength = 0.08f;

            animations.Add("jump",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\Mario"),
                    34,
                    "jump",
                    102,
                    1));
            float i = animations["jump"].FrameLength;

            animations["jump"].LoopAnimation = true;
            animations["jump"].FrameLength = 0.7f;
            animations["jump"].NextAnimation = "idle";

            animations.Add("die",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\Mario"),
                    34,
                    "die",
                    171,
                    1));
            animations["die"].LoopAnimation = false;


            animations.Add("idleBig",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\marioBig"),
                    32,
                    "idleBig",
                    128,
                    1));
            animations["idleBig"].LoopAnimation = true;

            animations.Add("runBig",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\marioBig"),
                    32,
                    "run",
                    0,
                    3));
            animations["runBig"].LoopAnimation = true;

            animations.Add("crouch",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\marioBig"),
                    32,
                    "run",
                    160,
                    1));
            animations["crouch"].LoopAnimation = true;

            animations.Add("jumpBig",
                new AnimationStrip(
                    content.Load<Texture2D>(@"Sprites\Player\marioBig"),
                    32,
                    "jumpBig",
                    96,
                    1));
            float k = animations["jumpBig"].FrameLength;

            animations["jumpBig"].LoopAnimation = true;
            animations["jumpBig"].FrameLength = 0.5f;
            animations["jumpBig"].NextAnimation = "idleBig";

            currentAnimation = "idle" + (isBig ? "Big" : "");
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
                                                      new Vector2(this.Location.X+this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height + 1)
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
                            isBig = false;
                            onGround = false;
                            this.velocity = new Vector2(0, -1000);
                            this.Dead = true;
                        }

                    }
                }

                onGround = true;
                this.velocity.Y = 0;
                this.location.Y -= (this.location.Y+animations[currentAnimation].FrameHeight) % 48;
            }
            else
                onGround = false;

            if (!Dead)
            {
                currentAnimation = "idle" + (isBig ? "Big" : "");
                if (!onGround)
                {
                    currentAnimation = "jump" + (isBig ? "Big" : "");
                }


                // Check collision above
                tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y));

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {

                    if (this.velocity.Y != 0)
                    {
                        this.velocity.Y = 0.1f;
                        this.location.Y = (int)(this.Center.Y / 48) * 48;
                    }
                }


                if (kb.IsKeyDown(Keys.Space) && onGround)
                {
                    this.velocity = new Vector2(0, -650);
                    currentAnimation = "jump" + (isBig ? "Big" : "");
                    onGround = false;
                }

                if (kb.IsKeyDown(Keys.Down) && onGround && isBig)
                {
                    currentAnimation = "crouch";
                }

                if (kb.IsKeyDown(Keys.Left))
                {
                    if (onGround) currentAnimation = "run" + (isBig ? "Big" : "");
                    this.FlipHorizontal = true;

                    if (this.location.X > (World.viewport.X + WalkableArea.Left))
                    {
                        this.location.X -= 5;
                    }
                    else if (World.viewport.X > 0)
                    {
                        World.viewport.X -= 5;
                        this.location.X -= 5;
                    }

                }

                if (kb.IsKeyDown(Keys.Right))
                {
                    if (onGround) currentAnimation = "run" + (isBig ? "Big" : "");
                    this.FlipHorizontal = false;

                    this.location.X += 5;

                    if (this.location.X > (World.viewport.X + WalkableArea.Right))
                    {

                        World.viewport.X += 5;
                    }

                }

                // Right collision test
                tile = CollisionEdgeTest(new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1));

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    this.location.X -= 5;

                    if (this.location.X >= (World.viewport.X + WalkableArea.Right))
                    {
                        World.viewport.X -= 5;
                    }
                }

                // Left collision test
                tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height - 1));

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    this.location.X += 5;

                    if (this.location.X <= (World.viewport.X + WalkableArea.Left))
                    {
                        World.viewport.X += 5;
                    }

                    //this.location.X = (int)(this.Center.X / 48) * 48;

                }
            }

            base.Update(gameTime);
        }
    }
}
