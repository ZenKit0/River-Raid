﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Projectile {
        public Texture2D texture;
        public Vector2 position;
        public Projectile(Texture2D texture, Vector2 position) {
            this.texture = texture;
            this.position = position;
        }
        public void UpdateProjectile() {
            position.Y -= Config.ProjectileSpeed;
        }

        public bool CheckCollision(Texture2D OtherTexture, Vector2 OtherPosition, int FrameCount = 1) {
            if (position.Y - 50f <= OtherPosition.Y &&
                position.Y + texture.Height >= OtherPosition.Y + (OtherTexture.Height / FrameCount) &&
                position.X >= OtherPosition.X &&
                position.X + texture.Width <= OtherPosition.X + (OtherTexture.Width / FrameCount))
                return true;
            return false;
        }
    }
}