using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ItalianGuys
{
    class Player : Sprite
    {
        public float gravityTimer = 0f;
        public float gravityTimerMax = 10f;
        public float gravity = 25f;
        private xTile.Map map;

        public Player(
           Vector2 location,
           Texture2D texture,
           Rectangle initialFrame,
           Vector2 velocity,
           xTile.Map map)
            : base(location, texture, initialFrame, velocity)
        {
            this.map = map;
        }

        public xTile.Tiles.Tile CollisionTest(Vector2 point)
        {
            if (point.X < map.DisplaySize.Width / 48 && point.Y < map.DisplaySize.Height / 48 && point.X >= 0 && point.Y >= 0)
            {
                xTile.Tiles.Tile tile = map.GetLayer("Foreground").Tiles[(int)point.X, (int)point.Y];

                return tile;
            }

            return null;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            gravityTimer += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (gravityTimer > gravityTimerMax)
            {
                this.velocity.Y += gravity;
                gravityTimer = 0f;
            }


            // Check collision below
            xTile.Tiles.Tile tile = CollisionTest(new Vector2((int)(this.Center.X / 48 + World.viewport.X / 48), (int)((this.Location.Y + this.BoundingBoxRect.Height) / 48)));

            if (tile != null)
            {
                if (this.velocity.Y != 0)
                {
                    this.velocity.Y = 0;
                    this.location.Y -= this.location.Y % 48;
                }
            }

            // Check collision above
            tile = CollisionTest(new Vector2((int)(this.Center.X / 48 + World.viewport.X / 48), (int)((this.Location.Y) / 48)));

            if (tile != null)
            {
                if (this.velocity.Y != 0)
                {
                    this.velocity.Y = 0;
                    this.location.Y = (int)(this.Center.Y / 48) * 48;
                }
            }

            if (kb.IsKeyDown(Keys.Space))
            {
                this.velocity = new Vector2(0, -500);
            }



            base.Update(gameTime);
        }
    }
}
