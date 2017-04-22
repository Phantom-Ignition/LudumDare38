using System;

using LudumDare38.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using LudumDare38.Characters;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Particles.Modifiers;
using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;

namespace LudumDare38.Scenes
{
    class SceneMap : SceneBase
    {
        //--------------------------------------------------
        // Camera stuff

        private Camera2D _camera;
        private const float CameraSmooth = 0.1f;
        private const int PlayerCameraOffsetX = 20;
        private const int PlayerCameraOffsetY = 0;

        //--------------------------------------------------
        // Player

        private Player _player;

        public Player Player { get { return _player; } }

        //--------------------------------------------------
        // Particle Effects

        private List<ParticleEffect> _particleEffects;

        //--------------------------------------------------
        // Random

        private Random _rand;

        //--------------------------------------------------
        // Test particles

        private ParticleEffect _particleTest;

        //----------------------//------------------------//

        public Camera2D GetCamera()
        {
            return _camera;
        }

        public override void LoadContent()
        {

            base.LoadContent();


            var viewportSize = SceneManager.Instance.VirtualSize;
            _camera = new Camera2D(SceneManager.Instance.ViewportAdapter);

            // Player init
            _player = new Player(ImageManager.LoadCharacter("Player"));

            // Particles init
            var particleTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1);
            particleTexture.SetData(new[] { Color.White });
            ParticlesInit(new TextureRegion2D(particleTexture));

            // Random init
            _rand = new Random();

            // Load the map
            LoadMap(MapManager.FirstMap);
        }

        private void ParticlesInit(TextureRegion2D textureRegion)
        {
            _particleTest = new ParticleEffect()
            {
                Name = "Test",
                Emitters = new[]
                {
                    new ParticleEmitter(500, TimeSpan.FromSeconds(1.5), Profile.Spray(Axis.Up, (float)Math.PI))
                    {
                        TextureRegion = textureRegion,
                        Parameters = new ParticleReleaseParameters()
                        {
                            Speed = new RangeF(30f, 60f),
                            Quantity = 10,
                            Rotation = new RangeF(-1f, 1f),
                            Scale = new RangeF(2.0f, 4.5f),
                            Color = new HslColor(186, 0.13f, 0.96f)
                        },
                        Modifiers = new IModifier[]
                        {
                            new LinearGravityModifier { Direction = Axis.Down, Strength = 150f },
                            new RotationModifier { RotationRate = 2.0f },
                            new OpacityFastFadeModifier()
                        }
                    }
                }
            };
            _particleEffects = new List<ParticleEffect>();
            _particleEffects.Add(_particleTest);
        }

        private void LoadMap(int mapId)
        {
            MapManager.Instance.LoadMap(Content, mapId);
            InitMapObjects();
        }

        private void MapLoadedFromTransition(int mapId)
        {
            InitMapObjects();
        }

        private void InitMapObjects()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            var spawnPoint = new Vector2(MapManager.Instance.GetPlayerSpawn().X,
                MapManager.Instance.GetPlayerSpawn().Y);
            _player.Position = new Vector2(spawnPoint.X, spawnPoint.Y - _player._sprite.GetColliderHeight());
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _player.Update(gameTime);
            UpdateCamera();
            UpdateParticles(deltaTime);
            base.Update(gameTime);

            if (InputManager.Instace.KeyPressed(Keys.M))
            {
                MapManager.Instance.LoadMapWithTransition(2, MapLoadedFromTransition);
            }

            if (InputManager.Instace.KeyPressed(Keys.P))
            {
                _particleTest.Trigger(SceneManager.Instance.VirtualSize / 2);
            }

            DebugValues["Delta Time"] = gameTime.ElapsedGameTime.TotalMilliseconds.ToString();
        }

        private void UpdateCamera()
        {
            var size = SceneManager.Instance.WindowSize;
            var viewport = SceneManager.Instance.ViewportAdapter;
            var newPosition = _player.Position - new Vector2(viewport.VirtualWidth / 2f, viewport.VirtualHeight / 2f);
            var playerOffsetX = PlayerCameraOffsetX + _player._sprite.GetColliderWidth() / 2;
            var playerOffsetY = PlayerCameraOffsetY + _player._sprite.GetFrameHeight() / 2;
            var x = MathHelper.Lerp(_camera.Position.X, newPosition.X + playerOffsetX, CameraSmooth);
            x = MathHelper.Clamp(x, 0.0f, MapManager.Instance.MapWidth - viewport.VirtualWidth);
            var y = MathHelper.Lerp(_camera.Position.Y, newPosition.Y + playerOffsetY, CameraSmooth);
            y = MathHelper.Clamp(y, 0.0f, MapManager.Instance.MapHeight - viewport.VirtualHeight);
            _camera.Position = new Vector2(x, y);
        }

        private void UpdateParticles(float deltaTime)
        {
            _particleEffects.ForEach(particle => particle.Update(deltaTime));
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);
            var debugMode = SceneManager.Instance.DebugMode;

            // Draw the camera (with the map)
            MapManager.Instance.Draw(_camera, spriteBatch);

            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            // Draw the player
            _player.DrawCharacter(spriteBatch);
            if (debugMode) _player.DrawColliderBox(spriteBatch);

            // Draw the particles
            _particleEffects.ForEach(particle => spriteBatch.Draw(particle));

            spriteBatch.End();
        }
    }
}
