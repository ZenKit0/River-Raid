﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using River_Raid;

namespace River_Ride___MG
{
    public class Main : Game
    {
        #region Utils
        private GraphicsDeviceManager _graphics;
        
        private SpriteBatch _spriteBatch;
        #endregion

        #region Player
        private Texture2D Plane;
        private Vector2 PlanePosition = new Vector2(500f, 500f);

        float AnimationTime, AnimationDelay = 100f;
        int AnimationFrame;
        Rectangle PlaneAnimation;

        bool CanGoLeft = true, CanGoRight = true;

        private List<Projectile> Projectiles = new List<Projectile>();
        float ProjectileTime, ProjectileDelay = 800f;
        #endregion

        #region Textures
        private List<Background> Backgrounds = new List<Background>();
        private Texture2D Shadow;

        private Texture2D UI;
        private FuelPtr Fuel;
        private Vector2 FuelPosition = new Vector2(320f, 689f);
        bool isExploding = false, isExploded = false;
        private Texture2D ExplosionEffect;
        private Rectangle ExplosionAnimation;

        private Texture2D GameOverText;
        private Rectangle GameOverTextAnimation;

        private Texture2D ProjectileTexture;
        #endregion

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = Config.ContentRootDirectory;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = Config.PrefferedHeight;
            _graphics.PreferredBackBufferWidth = Config.PrefferedWidth;
            _graphics.ApplyChanges();
            Window.Title = Config.TitleGame;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < Config.BG_count; i++) {
                Backgrounds.Add(new Background(Content.Load<Texture2D>($"BG_{i+1}"), i));
            }
            Shadow = Content.Load<Texture2D>("Shadow");
            Plane = Content.Load<Texture2D>("Plane");
            ProjectileTexture = Content.Load<Texture2D>("Projectile");
            ExplosionEffect = Content.Load<Texture2D>("ExplosionEffect");
            UI = Content.Load<Texture2D>("UI");
            GameOverText = Content.Load<Texture2D>("GameOver");
            Fuel = new FuelPtr(Content.Load<Texture2D>("Fuel_Level"), Content.Load<Texture2D>("Fuel_UI"), 64, 320, FuelPosition);
            Fuel.OnFuelEmpty += ExplodePlane;

            Backgrounds[0].BG_position = new Vector2(Backgrounds[0].BG_position.X, -Backgrounds[0].BG_texture.Height);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Movement
            KeyboardState InputKey = Keyboard.GetState();
            if (!isExploding) {
                if ((InputKey.IsKeyDown(Keys.A) || InputKey.IsKeyDown(Keys.Left)) && CanGoLeft) {
                    PlanePosition.X -= Config.PlaneMovementSpeed;
                } else if ((InputKey.IsKeyDown(Keys.D) || InputKey.IsKeyDown(Keys.Right)) && CanGoRight) {
                    PlanePosition.X += Config.PlaneMovementSpeed;
                }
            }

            if (PlanePosition.X <= 100)
                CanGoLeft = false;
            else 
                CanGoLeft = true;

            if (PlanePosition.X >= 830)
                CanGoRight = false;
            else
                CanGoRight = true;
            // Movement

            // Animation
            AnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (AnimationTime >= AnimationDelay) {
                if (isExploding && AnimationFrame >= 3) {
                    isExploding = false;
                    isExploded = true;
                }
                if (isExploded) {
                    AnimationDelay = 500f;
                }
                   
                if (AnimationFrame >= 3) {
                    AnimationFrame = 0;
                } else {
                    AnimationFrame++;
                }
                
                AnimationTime = 0;
            }

            
            foreach (Background item in Backgrounds) {
                item.UpdatePosition();
            }

            Fuel.UpdateFuelSpend();
            foreach (Projectile item in Projectiles)
                item.UpdateProjectile();

            if (InputKey.IsKeyDown(Keys.J))
                Fuel.AddFuel(20f);

            ProjectileTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (ProjectileTime >= ProjectileDelay) {
                if (InputKey.IsKeyDown(Keys.F)) { 
                    Projectiles.Add(new Projectile(ProjectileTexture, PlanePosition + new Vector2(40f, 0f)));
                    ProjectileTime = 0;
                }
            }

            for (int i = 0; i < Projectiles.Count; i++) {
                if (Projectiles[i].ProjectilePosition.Y < -20f) {
                    Projectiles.RemoveAt(i);
                } 
            }

            PlaneAnimation = new Rectangle(Plane.Width / 4 * AnimationFrame, 0, Plane.Width/4, Plane.Height);
            ExplosionAnimation = new Rectangle(ExplosionEffect.Width / 4 * AnimationFrame, 0, ExplosionEffect.Width/4, ExplosionEffect.Height);
            GameOverTextAnimation = new Rectangle(GameOverText.Width / 4 * AnimationFrame, 0, GameOverText.Width/4, GameOverText.Height);
            // Animation

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            foreach (Background item in Backgrounds) {
                _spriteBatch.Draw(item.BG_texture, item.BG_position, new Rectangle(0, 0, item.BG_texture.Width, item.BG_texture.Height), Color.White);
            }

            foreach (Projectile item in Projectiles) {
                _spriteBatch.Draw(item.ProjectileTexture, item.ProjectilePosition, Color.White);
            }
            
            if (isExploded)
                _spriteBatch.Draw(GameOverText, new Vector2(), GameOverTextAnimation, Color.White);
            if (!isExploding && !isExploded)
                _spriteBatch.Draw(Plane, PlanePosition, PlaneAnimation, Color.White);
            if (isExploding)
                _spriteBatch.Draw(ExplosionEffect, PlanePosition - new Vector2(95f), ExplosionAnimation, Color.White);
            _spriteBatch.Draw(Shadow, new Vector2(), Color.White);
            
            _spriteBatch.Draw(UI, new Vector2(), Color.White);
            _spriteBatch.Draw(Fuel.Fuel_Pointer, Fuel.position, Color.White);
            _spriteBatch.Draw(Fuel.Fuel_UI, new Vector2(), Color.White);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ExplodePlane() {
            AnimationFrame = 0;
            isExploding = true;
            Config.BG_speed = 0f;
        }
    }
}
