using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItalianGuys
{
    public class AnimationStrip
    {
        #region Declarations
        private Texture2D texture;
        private int frameWidth;
        private int frameHeight;
        private int frameCount;

        private int startx;

        private float frameTimer = 0f;
        private float frameDelay = 0.05f;

        private int currentFrame;

        private bool loopAnimation = true;
        private bool finishedPlaying = false;

        private string name;
        private string nextAnimation;
        #endregion

        #region Properties
        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        public int FrameHeight
        {
            get { return frameHeight; }
            set { frameHeight = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string NextAnimation
        {
            get { return nextAnimation; }
            set { nextAnimation = value; }
        }

        public bool LoopAnimation
        {
            get { return loopAnimation; }
            set { loopAnimation = value; }
        }

        public bool FinishedPlaying
        {
            get { return finishedPlaying; }
        }

        public int FrameCount
        {
            get { return frameCount; }
        }

        public float FrameLength
        {
            get { return frameDelay; }
            set { frameDelay = value; }
        }

        public Rectangle FrameRectangle
        {
            get
            {
                return new Rectangle(
                    currentFrame * frameWidth + startx,
                    0,
                    frameWidth,
                    frameHeight);
            }
        }
        #endregion

        #region Constructor
        public AnimationStrip(Texture2D texture, int frameWidth, string name)
        {
            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = texture.Height;
            this.name = name;
            this.frameCount = texture.Width / frameWidth;
            this.startx = 0;
        }


        public AnimationStrip(Texture2D texture, int frameWidth, string name, int startx, int frameCount)
        {
            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = texture.Height;
            this.name = name;
            this.frameCount = frameCount;
            this.startx = startx;
        }

        #endregion

        #region Public Methods
        public void Play()
        {
            currentFrame = 0;
            finishedPlaying = false;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameTimer += elapsed;

            if (frameTimer >= frameDelay)
            {
                currentFrame++;
                if (currentFrame >= FrameCount)
                {
                    if (loopAnimation)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        currentFrame = FrameCount - 1;
                        finishedPlaying = true;
                    }
                }

                frameTimer = 0f;
            }
        }
        #endregion

    }
}
