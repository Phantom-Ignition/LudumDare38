using LudumDare38.Managers;
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
using MonoGame.Extended.BitmapFonts;
using LudumDare38.Helpers.TinyTween;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Audio;

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
        // Wave Interval

        private bool _waveInterval;
        private float _waveIntervalTick;

        //--------------------------------------------------
        // Tweens

        private List<ITween> _tweens;
        private Vector2Tween _waveTextPositionTween;
        private FloatTween _waveTextAlphaTween;
        private FloatTween _waveBackgroundAlphaTween;
        private FloatTween _planetRotationTween;

        //--------------------------------------------------
        // Wave Clear Text

        private Sprite _waveClearBackgroundSprite;
        private const string WaveClearText = "Wave Clear!";
        private const string CurrentWaveText = "Wave #{0}";
        private int _waveTextPhase;
        private Vector2 _waveClearPosition;
        private float _waveClearAlpha;
        private int _waveTextsCompleted;

        //--------------------------------------------------
        // Upgrade Helper

        private UpgradeSelectionHelper _upgradeSelectionHelper;

        //--------------------------------------------------
        // SEs

        private SoundEffect _enemyHitSe;
        private SoundEffect _planetHitSe;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            _rand = new Random();
            _upgradeSelectionHelper = new UpgradeSelectionHelper();
            _upgradeSelectionHelper.LoadContent();

            LoadSounds();
            CreatePlanet();
            CreateHud();
            CreateGuns();
            InitializeProjectiles();
            InitializeEnemies();
            InitializeSpawnManager();
            InitializeWaveClear();

            SoundManager.StartBgm("Rhinoceros");
        }

        private void LoadSounds()
        {
            _enemyHitSe = SoundManager.LoadSe("Alien_hit");
            _planetHitSe = SoundManager.LoadSe("Planet_hit");
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


            //_guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));

            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            /*
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new Shield(GunType.Shield, PlanetManager.Instance.AvailableOrbits[0])));

            /*
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new LaserGun(GunType.LaserGun, PlanetManager.Instance.AvailableOrbits[0])));
            /*
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));
            _guns.Add(PlanetManager.Instance.CreateGun(new BasicGun(GunType.Basic, PlanetManager.Instance.AvailableOrbits[0])));

            /*
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

        private void InitializeWaveClear()
        {
            var viewportSize = SceneManager.Instance.VirtualSize;

            var bgTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            bgTexture.SetData(new Color[] { Color.Black });
            _waveClearBackgroundSprite = new Sprite(bgTexture);
            _waveClearBackgroundSprite.Scale = viewportSize;
            _waveClearBackgroundSprite.Position = viewportSize / 2;
            _waveClearBackgroundSprite.Alpha = 0.0f;

            _waveTextPositionTween = new Vector2Tween();
            _waveTextAlphaTween = new FloatTween();
            _waveBackgroundAlphaTween = new FloatTween();
            _planetRotationTween = new FloatTween();

            _tweens = new List<ITween>();
            _tweens.Add(_waveTextPositionTween);
            _tweens.Add(_waveTextAlphaTween);
            _tweens.Add(_waveBackgroundAlphaTween);
            _tweens.Add(_planetRotationTween);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Update tweens
            _tweens.ForEach(tween => tween.Update(deltaTime));
            if (_planetRotationTween.State == TweenState.Running)
            {
                _rotation = _planetRotationTween.CurrentValue;
            }

            // Update the planet
            float floating;
            _planet.Update(gameTime, out floating);

            if (_planet.HP <= 0)
            {
                PlanetManager.Instance.WavesSurvived = _enemiesSpawnManager.CurrentWave + 1;
                SceneManager.Instance.ChangeScene("SceneGameover");
            }
            
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
                                _enemyHitSe.PlaySafe();
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
                        PlanetManager.Instance.RestoreOrbitField(shield.OrbitField);
                        _gunsToRemove.Add(gun);
                    }
                }
            }

            // Clear the projectiles
            _gunsToRemove.ForEach(gun => _guns.Remove(gun));
            _gunsToRemove.Clear();

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
                        _planetHitSe.PlaySafe();
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
            var enemiesToAdd = new List<EnemyBase>();
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                if (!enemy.Alive)
                {
                    ISuicidable enemySuicidable = enemy as ISuicidable;
                    if (enemySuicidable != null)
                    {
                        if (enemySuicidable.NeedCollectExplosionDamage())
                        {
                            if (!enemySuicidable.ExplodedByShield())
                            {
                                _planetHitSe.PlaySafe();
                                _planet.GetDamaged(enemySuicidable.ContactDamage());
                            }
                            enemySuicidable.CollectExplosionDamage();
                        }
                    }
                }
                if (enemy.RequestErase)
                {
                    _enemiesToRemove.Add(enemy);
                }
                else if (!enemy.Dying && enemy.Alive)
                {
                    if (enemy.Type == EnemyType.Boss)
                    {
                        var boss = (Boss)enemy;
                        while (boss.EnemiesQueued.Count > 0)
                        {
                            var newEnemy = boss.EnemiesQueued[0];
                            enemiesToAdd.Add(newEnemy);
                            boss.EnemiesQueued.Remove(newEnemy);
                        }
                    }
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
                    else if (enemy.Type == EnemyType.TripleShooter)
                    {
                        var shooter = (TripleShooter)enemy;
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
                                    cShield.GetDamaged(enemySuicidable.ContactDamage());
                                    enemySuicidable.Explode(true);
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
                                _enemyHitSe.PlaySafe();
                                enemy.GetShot(projectile.Damage, collisionPoint, projectile.Rotation());
                                projectile.Destroy();
                            }
                        }
                    }
                }
            }

            // Clear the enemies
            _enemiesToRemove.ForEach(enemy => _enemies.Remove(enemy));
            _enemiesToRemove.Clear();
            _enemies.AddRange(enemiesToAdd);

            // Update the wave system
            UpdateWave(gameTime);

            // Update the Hud
            _gameHud.CurrentHP = _planet.HP;

            if (_upgradeSelectionHelper.IsActive || _waveInterval) return;

            // Update the enemy spawn manager
            UpdateEnemiesSpawn(gameTime);

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

            // Update the rotation
            if (InputManager.Instace.KeyDown(Keys.Left))
                _rotation -= 0.05f;
            if (InputManager.Instace.KeyDown(Keys.Right))
                _rotation += 0.05f;
            if (InputManager.Instace.KeyPressed(Keys.P))
            {
                SceneManager.Instance.StartCameraShake(1, 500);
            }

            DebugValues["current wave"] = (_enemiesSpawnManager.CurrentWave + 1).ToString();
            DebugValues["kamikaze count"] = _enemiesSpawnManager.KamikazeCount.ToString();
            DebugValues["shooter count"] = _enemiesSpawnManager.ShooterCount.ToString();
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
                    case EnemyType.TripleShooter:
                        enemy = new TripleShooter(ImageManager.LoadEnemy("TripleShooter"));
                        break;
                    case EnemyType.Boss:
                        enemy = new Boss(ImageManager.LoadEnemy("Boss"));
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

        private void UpdateWave(GameTime gameTime)
        {
            if (_waveInterval)
            {
                if (_waveTextPhase == 0)
                {
                    var viewportSize = SceneManager.Instance.VirtualSize;
                    var textWidth = SceneManager.Instance.GameFont.MeasureString(WaveText()).X;
                    var positionFrom = new Vector2((viewportSize.X - textWidth) / 2, viewportSize.Y / 2 - 100);
                    var positionTo = new Vector2((viewportSize.X - textWidth) / 2, viewportSize.Y / 2 - 15);
                    _waveTextPositionTween.Start(positionFrom, positionTo, 1500.0f, ScaleFuncs.QuadraticEaseInOut);
                    _waveTextAlphaTween.Start(0.0f, 1.0f, 1500.0f, ScaleFuncs.Linear);
                    _waveTextPhase = 1;
                    return;
                }
                _waveIntervalTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_waveTextPhase == 3 && _upgradeSelectionHelper.Complete)
                {
                    _waveTextsCompleted++;
                    _waveTextPhase = 0;
                    _upgradeSelectionHelper.Deactivate();
                    _waveIntervalTick = 2000.0f;
                }
                else if (_waveTextPhase == 2 && _waveIntervalTick <= 0.0f)
                {
                    _waveTextPhase = 3;
                    if (_waveTextsCompleted == 0)
                    {
                        _upgradeSelectionHelper.Activate();
                        _enemiesSpawnManager.StartNextWave();
                    }
                    else
                    {
                        PlanetManager.Instance.Paused = false;
                        _waveBackgroundAlphaTween.Start(0.5f, 0.0f, 500.0f, ScaleFuncs.Linear);
                        _waveInterval = false;
                        _waveIntervalTick = 0.0f;
                        _waveTextsCompleted = 0;
                        _waveTextPhase = 0;
                    }
                }
                else if (_waveTextPhase == 1 && _waveIntervalTick <= 500.0f)
                {
                    _waveTextPhase = 2;
                    _waveTextAlphaTween.Start(1.0f, 0.0f, 500.0f, ScaleFuncs.Linear);
                    
                    var viewportSize = SceneManager.Instance.VirtualSize;
                    var textWidth = SceneManager.Instance.GameFont.MeasureString(WaveText()).X;
                    var positionFrom = new Vector2((viewportSize.X - textWidth) / 2, viewportSize.Y / 2 - 15);
                    var positionTo = new Vector2((viewportSize.X - textWidth) / 2, viewportSize.Y / 2 + 50);
                    _waveTextPositionTween.Start(positionFrom, positionTo, 500.0f, ScaleFuncs.QuadraticEaseIn);
                }
            }
            else
            {
                if (_enemies.Count == 0 && _enemiesSpawnManager.WaveCompleted)
                {
                    PlanetManager.Instance.Paused = true;
                    _waveInterval = true;
                    _waveIntervalTick = 2000.0f;
                    _waveBackgroundAlphaTween.Start(0.0f, 0.5f, 500.0f, ScaleFuncs.Linear);
                    _waveTextPhase = 0;
                    _waveClearAlpha = 0.0f;
                    _planetRotationTween.Start(_rotation, 0.0f, 1000.0f, ScaleFuncs.CubicEaseInOut);
                }
            }

            _waveClearPosition = _waveTextPositionTween.CurrentValue;
            _waveClearAlpha = _waveTextAlphaTween.CurrentValue;
            _waveClearBackgroundSprite.Alpha = _waveBackgroundAlphaTween.CurrentValue;
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

            // Draw the wave text
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_waveClearBackgroundSprite);
            spriteBatch.DrawString(SceneManager.Instance.GameFont, WaveText(), _waveClearPosition + 1 * Vector2.UnitY, Color.Black * _waveClearAlpha);
            spriteBatch.DrawString(SceneManager.Instance.GameFont, WaveText(), _waveClearPosition, Color.White * _waveClearAlpha);
            spriteBatch.End();

            // Draw the upgrade helper
            _upgradeSelectionHelper.Draw(spriteBatch, transformMatrix);

            base.Draw(spriteBatch, transformMatrix);
        }

        private string WaveText()
        {
            return _waveTextsCompleted < 1 ? WaveClearText : String.Format(CurrentWaveText, _enemiesSpawnManager.CurrentWave + 1);
        }
    }
}
