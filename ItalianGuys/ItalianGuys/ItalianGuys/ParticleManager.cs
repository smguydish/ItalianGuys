using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ItalianGuys
{
    class Particle : Sprite
    {
        private float lifeSpanTimer = 0f;
        private float lifeSpan;
        private float angularvelocity;

        private float rotationTimer = 0;
        private float rotationTimerMax = 10f;

        private float gravityTimer = 0f;
        private float gravityTimerMax = 10f;
        private float gravity = 25f;
        public bool onGround = false;

        public bool IgnoreCollisions = false;

        public Particle(
            Texture2D texture,
            Vector2 location,
            Vector2 velocity,
            float lifeSpan,
            float angularVelocity,
            bool ignoreCollisions,
            xTile.Map map)
            : base(location, velocity, map)
        {
            this.lifeSpan = lifeSpan;
            this.angularvelocity = angularVelocity;
            this.Dead = false;
            this.IgnoreCollisions = ignoreCollisions;
        }

        public override void Update(GameTime gameTime)
        {
            lifeSpanTimer += gameTime.ElapsedGameTime.Milliseconds;
            rotationTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (lifeSpanTimer > lifeSpan)
                Dead = true;

            if (lifeSpanTimer < 500)
            {
                this.TintColor *= 1-(1 / (lifeSpanTimer-100)); // new Color(1f, 1f, 1f, 0.35f);
            }

            if (rotationTimer > rotationTimerMax)
            {
                this.Rotation += angularvelocity;
                rotationTimer = 0;
            }

            gravityTimer += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (gravityTimer > gravityTimerMax && !onGround)
            {
                this.velocity.Y += gravity;
                gravityTimer = 0f;
            }

            xTile.Tiles.Tile tile = null;

            if (!IgnoreCollisions)
            {
                // Check collision below
                KeyValuePair<xTile.Tiles.Tile, Vector2> ctest = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height + 1),
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
            }


            base.Update(gameTime);

            if (!IgnoreCollisions)
            {

                if (!Dead)
                {


                    // Right collision test
                    KeyValuePair<xTile.Tiles.Tile, Vector2> ctest = CollisionEdgeTest(new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1));
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

                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Dead)
                return;

            base.Draw(spriteBatch);
        }
    }

    class ParticleManager
    {
        xTile.Map map;
        List<Particle> particles = new List<Particle>();
        Dictionary<string, AnimationStrip> particleList;

        public ParticleManager(xTile.Map map)
        {
            this.map = map;
            particleList = new Dictionary<string, AnimationStrip>();
        }

        public void LoadContent(ContentManager content)
        {
            AddParticleAnimation("block1", new AnimationStrip(content.Load<Texture2D>("tiles"), "default", new Rectangle(0,48+3,24,24), 1));
            AddParticleAnimation("block2", new AnimationStrip(content.Load<Texture2D>("tiles"), "default", new Rectangle(24, 48 + 3, 24, 24), 1));
            AddParticleAnimation("block3", new AnimationStrip(content.Load<Texture2D>("tiles"), "default", new Rectangle(0, 48 + 3 + 24, 24, 24), 1));
            AddParticleAnimation("block4", new AnimationStrip(content.Load<Texture2D>("tiles"), "default", new Rectangle(24, 48 + 3 + 24, 24, 24), 1));

            AddParticleAnimation("coin", new AnimationStrip(content.Load<Texture2D>("tiles"), "default", new Rectangle(11, 1029, 28, 28), 1));

        }

        public void AddParticleAnimation(string name, AnimationStrip animation)
        {
            particleList.Add(name, animation);
        }

        public void SpawnParticle(String name, Vector2 location, Vector2 velocity, float lifeSpan, float angularVelocity, bool ignoreCollisions)
        {
            if (!particleList.ContainsKey(name))
                throw new Exception("Particle name " + name + " not defined using AddParticleAnimation");

            AnimationStrip animation = particleList[name];

            Particle p = new Particle(animation.Texture, location, velocity, lifeSpan, angularVelocity, ignoreCollisions, this.map);
            p.AddAnimation("default", animation);

            

            particles.Add(p);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);

                if (particles[i].Location.Y > World.viewport.Height || particles[i].Dead)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }
        }

    }
}
