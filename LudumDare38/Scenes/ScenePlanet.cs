using LudumDare38.Managers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Maps.Tiled;
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
        // Enemies Spawn Manager

        private EnemiesSpawnManager _enemiesSpawnManager;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            _gameHud = new GameHud();

            CreatePlanet();
            CreateGuns();
            InitializeProjectiles();
            InitializeEnemies();
            InitializeSpawnManager();
        }

        private void CreatePlanet()
        {
            var center = SceneManager.Instance.VirtualSize / 2;
            var planetTexture = ImageManager.Load("Planet");
            _planet = new GamePlanet(planetTexture, center);
        }

        private void CreateGuns()
        {
            _guns = new List<GameGunBase>();
            _guns.Add(new BasicGun(1, GunType.Basic, 0.0f));
            _guns.Add(new Laser(1, GunType.Basic, (float)Math.PI));
            _guns.Add(new Shield(2, GunType.Basic, (float)Math.PI * 0.5f));
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

            // Update the guns
            _guns.ForEach(gun => gun.Update(gameTime, _rotation, floating));
            if (InputManager.Instace.KeyPressed(Keys.S))
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
                    _projectilesToRemove.Add(projectile);
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
                if (!enemy.Dying && enemy.Alive && enemy.ImmunityTime <= 0.0f)
                {
                    var rectEnemy = enemy.BoundingBox;
                    var dataEnemy = enemy.SpriteTextureData;
                    foreach (var projectile in _projectiles)
                    {
                        if (projectile.RequestErase) continue;
                        var textureProjectile = projectile.BoundingBox;
                        var dataProjectile = projectile.SpriteTextureData;
                        Vector2 collisionPoint;
                        if (CollisionHelper.IntersectPixels(rectEnemy, dataEnemy, textureProjectile, dataProjectile, out collisionPoint))
                        {
                            enemy.GetShot(1, collisionPoint, projectile.Rotation);
                            projectile.Destroy();
                        }
                    }
                }
            }

            // Clear the enemies
            _enemiesToRemove.ForEach(enemy => _enemies.Remove(enemy));
            _enemiesToRemove.Clear();

            // Update the enemy spawn manager
            UpdateEnemiesSpawn(gameTime);

            // Update the rotation
            if (InputManager.Instace.KeyDown(Keys.Left))
                _rotation -= 0.03f;
            if (InputManager.Instace.KeyDown(Keys.Right))
                _rotation += 0.03f;
            if (InputManager.Instace.KeyPressed(Keys.P))
                _enemies[0].aa();
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

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            spriteBatch.Begin(transformMatrix: viewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);

            // Draw the HUD
            _gameHud.Draw(spriteBatch);

            // Draw the planet
            _planet.Draw(spriteBatch);

            // Draw the guns
            _guns.ForEach(gun => gun.Draw(spriteBatch));

            // Draw the enemies
            _enemies.ForEach(enemy => enemy.Draw(spriteBatch));

            // Draw the projectiles
            _projectiles.ForEach(projectile => projectile.Sprite.Draw(spriteBatch, projectile.Position));

            spriteBatch.End();

            base.Draw(spriteBatch, viewportAdapter);
        }
    }
}
