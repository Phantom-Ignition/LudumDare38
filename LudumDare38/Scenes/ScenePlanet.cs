﻿using LudumDare38.Managers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;
using LudumDare38.Objects;
using Microsoft.Xna.Framework.Input;
using LudumDare38.Objects.Guns;
using LudumDare38.Characters;
using LudumDare38.Helpers;

namespace LudumDare38.Scenes
{
    class ScenePlanet : SceneBase
    {
        //--------------------------------------------------
        // HUD

        private GameHud _gameHud;

        //--------------------------------------------------
        // Planet

        private GamePlanet _planet;

        //--------------------------------------------------
        // Guns

        private List<GameGunBase> _guns;
        private List<GameGunBase> _gunsToRemove;

        //--------------------------------------------------
        // Projectiles

        private Texture2D _projectilesColliderTexture;
        private List<GameProjectile> _projectilesToRemove;
        private List<GameProjectile> _projectiles;

        //--------------------------------------------------
        // Enemies

        private List<EnemyBase> _enemies;
        private List<EnemyBase> _enemiesToRemove;

        //--------------------------------------------------
        // Rotation

        private float _rotation;

        //--------------------------------------------------
        // Random

        private Random _rand;

        //--------------------------------------------------
        // Enemies Spawn Manager

        private EnemiesSpawnManager _enemiesSpawnManager;

        //--------------------------------------------------
        // Upgrade Helper

        private UpgradeSelectionHelper _upgradeSelectionHelper;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            _rand = new Random();
            _upgradeSelectionHelper = new UpgradeSelectionHelper();
            _upgradeSelectionHelper.LoadContent();

            CreatePlanet();
            CreateHud();
            CreateGuns();
            InitializeProjectiles();
            InitializeEnemies();
            InitializeSpawnManager();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _upgradeSelectionHelper.Dispose();
        }

        private void CreatePlanet()
        {
            var center = SceneManager.Instance.VirtualSize / 2;
            var planetTexture = ImageManager.Load("Planet");
            _planet = new GamePlanet(planetTexture, center);
        }

        private void CreateHud()
        {
            _gameHud = new GameHud()
            {
                MaxHP = _planet.HP,
                CurrentHP = _planet.HP
            };
        }

        private void CreateGuns()
        {
            _guns = new List<GameGunBase>();
            _gunsToRemove = new List<GameGunBase>();

            /*
            var basicOrbitField = PlanetManager.Instance.AvailableOrbits[0];
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, basicOrbitField)));

            var aorbitField = PlanetManager.Instance.AvailableOrbits[0];
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, aorbitField)));

            var borbitField = PlanetManager.Instance.AvailableOrbits[0];
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, borbitField)));*/
        }

        private void InitializeProjectiles()
        {
            _projectilesToRemove = new List<GameProjectile>();
            _projectiles = new List<GameProjectile>();
            _projectilesColliderTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1);
            _projectilesColliderTexture.SetData(new Color[] { Color.Orange });
        }

        private void InitializeEnemies()
        {
            _enemies = new List<EnemyBase>();
            _enemiesToRemove = new List<EnemyBase>();
        }

        private void InitializeSpawnManager()
        {
            _enemiesSpawnManager = new EnemiesSpawnManager();
            _enemiesSpawnManager.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Update the planet
            float floating;
            _planet.Update(gameTime, out floating);
            
            // Update the upgrade selection
            _upgradeSelectionHelper.Update(gameTime, _rotation, floating);

            // Update the PlanetManager guns queue
            while (PlanetManager.Instance.GunsQueue.Count > 0)
            {
                var gun = PlanetManager.Instance.GunsQueue[0];
                _guns.Add(gun);
                PlanetManager.Instance.GunsQueue.Remove(gun);
            }

            // Update the guns
            foreach (var gun in _guns)
            {
                gun.Update(gameTime, _rotation, floating);
                if (gun.GunType == GunType.LaserGun)
                {
                    var laserGun = (LaserGun)gun;
                    if (laserGun.Laser.Sprite.CurrentFrameList == "attack")
                    {
                        foreach (var enemy in _enemies)
                        {
                            Vector2 collisionPoint;
                            if (CollisionHelper.IsColliding(laserGun.Laser, enemy, out collisionPoint))
                            {
                                enemy.GetShot(1, collisionPoint, laserGun.Laser.Rotation());
                            }
                        }
                    }
                }
                if (gun.GunType == GunType.Shield)
                {
                    var shield = (Shield)gun;
                    if (shield.RequestingErase)
                    {
                        _gunsToRemove.Add(gun);
                    }
                }
            }

            if (_upgradeSelectionHelper.IsActive) return;

            // Clear the projectiles
            _gunsToRemove.ForEach(gun => _guns.Remove(gun));
            _gunsToRemove.Clear();

            // Shot!
            if (InputManager.Instace.KeyPressed(Keys.Z))
            {
                foreach (var gun in _guns)
                {
                    if (!gun.Static && gun.CurrentCooldown == 0.0f)
                    {
                        GameProjectile newProjectile;
                        if (gun.Shot(out newProjectile))
                        {
                            _projectiles.Add(newProjectile);
                        }
                    }
                }
            }

            // Update the projectiles
            foreach (var projectile in _projectiles)
            {
                projectile.Update(gameTime);

                if (projectile.RequestErase)
                {
                    _projectilesToRemove.Add(projectile);
                }
                else if (projectile.Subject == ProjectileSubject.FromEnemy)
                {
                    var pos = projectile.BoundingRectangle().Center.ToVector2();
                    var distance = Math.Sqrt(Math.Pow(_planet.X - pos.X, 2) + Math.Pow(_planet.Y - pos.Y, 2));
                    if (distance < GamePlanet.Radius + projectile.Sprite.GetColliderWidth() / 3)
                    {
                        _planet.GetDamaged(projectile.Damage);
                        projectile.Destroy();
                    }
                    if (!projectile.RequestErase)
                    {
                        var shields = _guns.Where(gun => gun.GunType == GunType.Shield).ToArray();
                        foreach (var shield in shields)
                        {
                            Vector2 collisionPoint;
                            var cShield = (Shield)shield;
                            if (cShield.Sprite.Alpha > 0.1f && CollisionHelper.IsColliding(cShield, projectile, out collisionPoint))
                            {
                                cShield.GetDamaged(1);
                                projectile.Destroy();
                            }
                        }
                    }
                }
            }

            // Clear the projectiles
            _projectilesToRemove.ForEach(projectile => _projectiles.Remove(projectile));
            _projectilesToRemove.Clear();

            // Update the enemies
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                if (enemy.RequestErase)
                {
                    _enemiesToRemove.Add(enemy);
                }
                else if (!enemy.Dying && enemy.Alive)
                {
                    if (enemy.Type == EnemyType.Shooter)
                    {
                        var shooter = (Shooter)enemy;
                        while (shooter.ProjectilesQueued.Count > 0)
                        {
                            var proj = shooter.ProjectilesQueued[0];
                            _projectiles.Add(proj);
                            shooter.ProjectilesQueued.Remove(proj);
                        }
                    }
                    else
                    {
                        ISuicidable enemySuicidable = enemy as ISuicidable;
                        if (enemySuicidable != null)
                        {
                            var shields = _guns.Where(gun => gun.GunType == GunType.Shield).ToArray();
                            foreach (var shield in shields)
                            {
                                Vector2 collisionPoint;
                                var cShield = (Shield)shield;
                                if (cShield.Sprite.Alpha > 0.1f && CollisionHelper.IsColliding(cShield, enemy, out collisionPoint))
                                {
                                    cShield.GetDamaged(1);
                                    enemySuicidable.Explode();
                                }
                            }
                        }
                    }
                    if (enemy.ImmunityTime <= 0.0f)
                    {
                        foreach (var projectile in _projectiles)
                        {
                            if (projectile.RequestErase) continue;
                            Vector2 collisionPoint;
                            if (projectile.Subject == ProjectileSubject.FromPlayer &&
                                CollisionHelper.IsColliding(projectile, enemy, out collisionPoint))
                            {
                                enemy.GetShot(1, collisionPoint, projectile.Rotation());
                                projectile.Destroy();
                            }
                        }
                    }
                }
            }

            // Clear the enemies
            _enemiesToRemove.ForEach(enemy => _enemies.Remove(enemy));
            _enemiesToRemove.Clear();

            // Update the enemy spawn manager
            UpdateEnemiesSpawn(gameTime);

            // Update the Hud
            _gameHud.CurrentHP = _planet.HP;

            // Update the rotation
            if (InputManager.Instace.KeyDown(Keys.Left))
                _rotation -= 0.05f;
            if (InputManager.Instace.KeyDown(Keys.Right))
                _rotation += 0.05f;
            if (InputManager.Instace.KeyPressed(Keys.P))
            {
                SceneManager.Instance.StartCameraShake(1, 500);
            }

            _upgradeSelectionHelper.Activate();
        }

        private void UpdateEnemiesSpawn(GameTime gameTime)
        {
            _enemiesSpawnManager.Update(gameTime);
            while (_enemiesSpawnManager.Queue.Count > 0)
            {
                var model = _enemiesSpawnManager.ShiftModelFromQueue();
                EnemyBase enemy = null;
                switch (model.Type)
                {
                    case EnemyType.Kamikaze:
                        enemy = new Kamikaze(ImageManager.LoadEnemy("Kamikaze"));
                        break;
                    case EnemyType.Shooter:
                        enemy = new Shooter(ImageManager.LoadEnemy("Shooter"));
                        break;
                }
                if (enemy != null)
                {
                    enemy.Position = model.Position;
                    var velocity = enemy.Velocity;
                    var angle = new Vector2(velocity.X * enemy.Sprite.TextureRegion.Width / 2, velocity.Y * enemy.Sprite.TextureRegion.Height / 2);
                    enemy.Position -= angle;
                    _enemies.Add(enemy);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            // Draw the HUD
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            _gameHud.Draw(spriteBatch);
            spriteBatch.End();

            // Draw the planet
            _planet.Draw(spriteBatch, transformMatrix);

            // Draw the guns
            _guns.ForEach(gun => gun.Draw(spriteBatch, transformMatrix));

            // Draw the enemies
            _enemies.ForEach(enemy => enemy.Draw(spriteBatch, transformMatrix));

            // Draw the projectiles
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            _projectiles.ForEach(projectile => projectile.Sprite.Draw(spriteBatch, projectile.Position));
            spriteBatch.End();

            // Draw the upgrade helper
            _upgradeSelectionHelper.Draw(spriteBatch, transformMatrix);

            base.Draw(spriteBatch, transformMatrix);
        }
    }
}
