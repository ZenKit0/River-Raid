﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace River_Raid {
    class Projectile : GameObject {
        public Projectile(Texture2D texture, Vector2 position) {
            this.texture = texture;
            this.position = position;
        }
        public void UpdateProjectile() {
            position.Y -= Config.ProjectileSpeed;
        }
    }
}
