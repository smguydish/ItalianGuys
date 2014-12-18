﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItalianGuys
{
    class Sprite
    {
        protected Dictionary<string, AnimationStrip> animations = new Dictionary<string, AnimationStrip>();
        protected string currentAnimation;

        public bool Dead = false;

        private xTile.Map map;

        private Color tintColor = Color.White;
        private float rotation = 0.0f;

        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;

        public object tag;

        protected Vector2 location = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;
        protected Vector2 origin = Vector2.Zero;

        public Sprite(
            Vector2 location,
            Vector2 velocity,
            xTile.Map map)
        {
            this.location = location;
            this.velocity = velocity;
            this.map = map;


           // this.origin = new Vector2(frameWidth / 2, frameHeight / 2);

            tag = null;

        }

        public bool FlipHorizontal { get; set; }

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(animations[currentAnimation].FrameWidth/2, animations[currentAnimation].FrameHeight/2); }
            set { origin = value; }
        }        

        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        public Rectangle Source
        {
            get { return animations[currentAnimation].FrameRectangle; }
        }

        public Rectangle Destination
        {
            get
            {
                return new Rectangle(
                    (int)location.X,
                    (int)location.Y,
                    animations[currentAnimation].FrameWidth,
                    animations[currentAnimation].FrameHeight);
            }
        }

        public Vector2 Center
        {
            get
            {
                return location +
                    new Vector2(animations[currentAnimation].FrameWidth / 2, animations[currentAnimation].FrameHeight / 2);
            }
        }

        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle(
                    (int)location.X + BoundingXPadding,
                    (int)location.Y + BoundingYPadding,
                    animations[currentAnimation].FrameWidth - (BoundingXPadding * 2),
                    animations[currentAnimation].FrameHeight - (BoundingYPadding * 2)
                    );
            }
        }

        public virtual xTile.Tiles.Tile CollisionTest(Vector2 point)
        {
            point.X = (int)((point.X) / 48f);
            point.Y = (int)(point.Y / 48);

            if (point.X < map.DisplaySize.Width / 48 && point.Y < map.DisplaySize.Height / 48 && point.X >= 0 && point.Y >= 0)
            {
                xTile.Tiles.Tile tile = map.GetLayer("Foreground").Tiles[(int)point.X, (int)point.Y];
                if (tile != null)
                {
                    
                }
                return tile;
            }

            return null;
        }


        public virtual xTile.Tiles.Tile CollisionEdgeTest(Vector2 point1, Vector2 point2)
        {
            xTile.Tiles.Tile tile1 = CollisionTest(point1);
            xTile.Tiles.Tile tile2 = CollisionTest(point2);
            xTile.Tiles.Tile tile3 = CollisionTest((point1+point2)/2);

            if (tile3 != null)
                return tile3;

            if (tile1 != null && tile2 != null)
            {
                // return whichever is closer to player center
                float dist1 = Vector2.Distance(point1, Center);
                float dist2 = Vector2.Distance(point2, Center);

                if (dist1 < dist2)
                    return tile1;
                else
                    return tile2;
            }

            if (tile1 != null)
                return tile1;

            return tile2;
        }

        public bool IsBoxColliding(Rectangle OtherBox)
        {
            return BoundingBoxRect.Intersects(OtherBox);
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            if (Vector2.Distance(Center, otherCenter) <
                (CollisionRadius + otherRadius))
                return true;
            else
                return false;
        }

        public void PlayAnimation(string name)
        {
            if (!(name == null) && animations.ContainsKey(name))
            {
                currentAnimation = name;
                animations[name].Play();
            }
        }

        private void updateAnimation(GameTime gameTime)
        {
            if (animations.ContainsKey(currentAnimation))
            {
                if (animations[currentAnimation].FinishedPlaying)
                {
                    PlayAnimation(animations[currentAnimation].NextAnimation);
                }
                else
                {
                    animations[currentAnimation].Update(gameTime);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            updateAnimation(gameTime);

            location += (velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (animations.ContainsKey(currentAnimation))
            {
                SpriteEffects effect = SpriteEffects.None;

                if (FlipHorizontal)
                {
                    effect = SpriteEffects.FlipHorizontally;
                }

                Vector2 loc = new Vector2(Center.X - World.viewport.X, Center.Y);

                if (loc.X >= -animations[currentAnimation].FrameWidth && loc.X <= World.viewport.Width)
                    spriteBatch.Draw(
                        animations[currentAnimation].Texture,
                        loc,
                        Source,
                        tintColor,
                        rotation,
                        Origin,
                        1.0f,
                        effect, 0);
            }

        }

    }
}
