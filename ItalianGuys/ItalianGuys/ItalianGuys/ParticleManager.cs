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

        public Particle(
            Texture2D texture,
            Vector2 location,
            Vector2 velocity,
            float lifeSpan,
            float angularVelocity,
            xTile.Map map)
            : base(location, velocity, map)
        {
            this.lifeSpan = lifeSpan;
            this.angularvelocity = angularVelocity;
            this.Dead = false;
        }

        public override void Update(GameTime gameTime)
        {
            lifeSpanTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (lifeSpanTimer > lifeSpan)
                Dead = true;

            base.Update(gameTime);
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
        List<Sprite> particles = new List<Sprite>();

        public ParticleManager()
        {
        }

        public void SpawnParticle(Texture2D texture, Rectangle source, Vector2 location, float angularvelocity, Vector2 velocity, float lifespan)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);

                if (particles[i].Location.Y > World.viewport.Height)
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
