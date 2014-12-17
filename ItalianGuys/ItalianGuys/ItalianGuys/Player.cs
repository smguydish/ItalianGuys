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
        public Rectangle WalkableArea = new Rectangle(200, 0, 400, 400);

        public bool onGround = false;

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
            point.X = (int)((point.X + World.viewport.X) / 48f);
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


        public xTile.Tiles.Tile CollisionEdgeTest(Vector2 point1, Vector2 point2)
        {
            xTile.Tiles.Tile tile1 = CollisionTest(point1);
            xTile.Tiles.Tile tile2 = CollisionTest(point2);

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
            xTile.Tiles.Tile tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height),
                                                      new Vector2(this.Location.X+this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1)
                                                      );

            if (tile != null)
            {
                onGround = true;
                this.velocity.Y = 0;
                this.location.Y -= this.location.Y % 48;
            }
            else
                onGround = false;

            // Check collision above
            tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X+this.BoundingBoxRect.Width, this.Location.Y));

            if (tile != null)
            {
                
                if (this.velocity.Y != 0)
                {
                    this.velocity.Y = 0.1f;
                    this.location.Y = (int)(this.Center.Y / 48) * 48;
                }
            }


            if (kb.IsKeyDown(Keys.Space) && onGround)
            {
                this.velocity = new Vector2(0, -700);
                onGround = false;
            }

            if (kb.IsKeyDown(Keys.Left) && World.viewport.X > 0)
            {
                if (this.location.X > WalkableArea.Left)
                    this.location.X -= 5;
                else
                    World.viewport.X -= 5;

                tile = CollisionEdgeTest(new Vector2(this.Location.X, this.Location.Y), new Vector2(this.Location.X, this.Location.Y + this.BoundingBoxRect.Height - 1));

                if (tile != null)
                {
                    if (this.location.X > WalkableArea.Left)
                        this.location.X += 5;
                    else
                        World.viewport.X += 5;
                }
            }

            if (kb.IsKeyDown(Keys.Right))
            {
                if (this.location.X < WalkableArea.Right)
                    this.location.X += 5;
                else
                    World.viewport.X += 5;

                tile = CollisionEdgeTest(new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y), new Vector2(this.Location.X + this.BoundingBoxRect.Width, this.Location.Y + this.BoundingBoxRect.Height - 1));

                if (tile != null)
                {
                    if (this.location.X < WalkableArea.Right)
                        this.location.X -= 5;
                    else
                        World.viewport.X -= 5;
                }
            }



            base.Update(gameTime);
        }
    }
}
