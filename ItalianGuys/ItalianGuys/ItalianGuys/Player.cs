using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using xTile.Tiles;

namespace ItalianGuys
{
    class Player : Sprite
    {
        public float gravityTimer = 0f;
        public float gravityTimerMax = 10f;
        public float gravity = 25f;

        public float invulnerabilityTimer = 0f;
        public float invulnerabilityTimerMax = 10f;

        public bool Invulnerable { get { return invulnerabilityTimer > 0; } set { invulnerabilityTimer = invulnerabilityTimerMax; } }

        public Rectangle WalkableArea = new Rectangle(250, 0, 250, 400);

        public bool onGround = false;
        public bool isBig = true;

        private ParticleManager particleManager;

        public Player(
           ContentManager content,
           Vector2 location,
           Vector2 velocity,
           xTile.Map map,
           ParticleManager particleManager)
            : base(location, velocity, map)
        {
            // mario, new Rectangle(0, 0, 48, 48
            this.particleManager = particleManager;

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

        public void MakeInvulnerable(float time)
        {
            invulnerabilityTimer = time;
        }

        public void Jump()
        {
            this.velocity = new Vector2(0, -650);
            currentAnimation = "jump" + (isBig ? "Big" : "");
            onGround = false;
        }

        public void Die()
        {
            currentAnimation = "die";
            isBig = false;
            onGround = false;
            this.velocity = new Vector2(0, -800);
            this.Dead = true;
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

            invulnerabilityTimer -= (float)gameTime.ElapsedGameTime.Milliseconds;
            if (invulnerabilityTimer <= 0)
            {
                invulnerabilityTimer = 0;
            }

            // Check collision below
            KeyValuePair<xTile.Tiles.Tile, Vector2> ctest = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height + 1),
                                                      new Vector2(this.Location.X+this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height + 1)
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
                            Die();
                        }

                    }
                }

                onGround = true;

                if (!Dead)
                {
                    this.velocity.Y = 0;
                    this.location.Y -= (this.location.Y + animations[currentAnimation].FrameHeight + 1) % 48;
                }
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
                ctest = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y));
                tile = ctest.Key;

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {

                    if (this.velocity.Y != 0)
                    {
                        this.velocity.Y = 0.01f;
                        this.location.Y = (int)(this.Center.Y / 48) * 48;

                        xTile.Layers.Layer layer = map.GetLayer("Foreground");

                        if (tile.TileIndex >= 29 && tile.TileIndex <= 32)
                        {
                            if (isBig)
                            {
                                Vector2 loc = ctest.Value * 48;
                                particleManager.SpawnParticle("block1", new Vector2(loc.X, loc.Y), new Vector2(-60, -650), 1200, 0f, true);
                                particleManager.SpawnParticle("block2", new Vector2(loc.X + 24, loc.Y), new Vector2(60, -750), 1200, 0f, true);
                                particleManager.SpawnParticle("block3", new Vector2(loc.X, loc.Y - 24), new Vector2(-20, -625), 1200, 0f, true);
                                particleManager.SpawnParticle("block4", new Vector2(loc.X + 24, loc.Y - 24), new Vector2(20, -590), 1200, 0f, true);

                                layer.Tiles[(int)ctest.Value.X, (int)ctest.Value.Y] = null;
                            }
                        }
                        else if (tile.TileIndex >= 0 && tile.TileIndex <= 3)
                        {
                            layer.Tiles[(int)ctest.Value.X, (int)ctest.Value.Y] = new StaticTile(layer, tile.TileSheet, tile.BlendMode, 122);

                            Vector2 loc = ctest.Value * 48;
                            particleManager.SpawnParticle("coin", new Vector2(loc.X + 12, loc.Y - 24), new Vector2(0, -500), 600, 0f, true);
                        }
                    }
                }


                if (kb.IsKeyDown(Keys.Space) && onGround)
                {
                    Jump();
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
                ctest = CollisionEdgeTest(new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1));
                tile = ctest.Key;

                if (tile != null && !tile.Properties.ContainsKey("Passable"))
                {
                    this.location.X -= 5;

                    if (this.location.X >= (World.viewport.X + WalkableArea.Right))
                    {
                        World.viewport.X -= 5;
                    }
                }

                // Left collision test
                ctest = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height - 1));
                tile = ctest.Key;

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
