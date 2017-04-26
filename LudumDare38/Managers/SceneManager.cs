using System;

using LudumDare38.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using LudumDare38.Helpers;
using MonoGame.Extended.BitmapFonts;

namespace LudumDare38.Managers
{
    class SceneManager
    {
        //--------------------------------------------------
        // Public variables
        
        public Vector2 WindowSize = new Vector2(500, 500);
        public Vector2 VirtualSize = new Vector2(500, 500);

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
            set
            {
                _graphicsDevice = value;
                _sceneRenderTarget = new RenderTarget2D(value, (int)VirtualSize.X, (int)VirtualSize.Y, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                _scanlinesRenderTarget = new RenderTarget2D(value, (int)VirtualSize.X, (int)VirtualSize.Y, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            }
        }

        public SpriteBatch SpriteBatch;
        public ViewportAdapter ViewportAdapter { get { return GameMain.ViewportAdapter; } }
        public GameWindow GameMap { get { return GameMain.GameWindow; } }
        public ContentManager Content { private set; get; }

        public bool RequestingExit = false;

        //--------------------------------------------------
        // SceneManager Singleton variables

        private static SceneManager _instance = null;
        private static readonly object _padlock = new object();
        public static SceneManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new SceneManager();
                    return _instance;
                }
            }
        }

        //--------------------------------------------------
        // Transition

        private SceneBase _currentScene, _newScene;
        private Sprite _transitionImage;
        private bool _isTransitioning = false;
        public bool IsTransitioning { get { return _isTransitioning; } }
        private bool _beginTransitionFade = false;

        //--------------------------------------------------
        // Camera Shake

        private bool _shakeEnabled;
        private float _shakeDuration;
        private float _shakeElapsed;
        private float _shakeMagnitude;
        private Vector2 _shakeOffset;

        //--------------------------------------------------
        // Render target

        private RenderTarget2D _sceneRenderTarget;
        private RenderTarget2D _scanlinesRenderTarget;
        private Effect _scanlinesEffect;
        private BloomFilter _bloomFilter;

        //--------------------------------------------------
        // Random

        private Random _rand;

        //--------------------------------------------------
        // Game fonts

        private BitmapFont _gameFont;
        public BitmapFont GameFont => _gameFont;

        private BitmapFont _gameFontSmall;
        public BitmapFont GameFontSmall => _gameFontSmall;

        private BitmapFont _gameFontBig;
        public BitmapFont GameFontBig => _gameFontBig;

        //--------------------------------------------------
        // Debug mode

        public bool DebugMode { get; set; } = false;

        //----------------------//------------------------//

        private SceneManager()
        {
            _rand = new Random();
            _currentScene = new SceneTitle();
        }

        public void RequestExit()
        {
            RequestingExit = true;
        }

        public SceneBase GetCurrentScene()
        {
            return _currentScene;
        }

        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            var transitionTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            transitionTexture.SetData<Color>(new Color[] { Color.Black });
            _transitionImage = new Sprite(transitionTexture);
            _transitionImage.Scale = new Vector2(VirtualSize.X, VirtualSize.Y);
            _transitionImage.Alpha = 0.0f;
            _transitionImage.IsVisible = false;

            _gameFont = Content.Load<BitmapFont>("fonts/MediumFont");
            _gameFontSmall = Content.Load<BitmapFont>("fonts/SmallFont");
            _gameFontBig = Content.Load<BitmapFont>("fonts/FontBig");

            _scanlinesEffect = EffectManager.Load("Scanlines");
            _scanlinesEffect.Parameters["Attenuation"].SetValue(0.04f);
            _scanlinesEffect.Parameters["LinesFactor"].SetValue(1000f);

            _bloomFilter = new BloomFilter();
            _bloomFilter.Load(GraphicsDevice, Content, (int)VirtualSize.X, (int)VirtualSize.Y);
            _bloomFilter.BloomPreset = BloomFilter.BloomPresets.One;
            _bloomFilter.BloomThreshold = 0.5f;
            _bloomFilter.BloomStrengthMultiplier = 0.5f;
            _bloomFilter.BloomStreakLength = 2;

            SoundManager.Initialize();
            SoundManager.SetSeVolume(0.7f);
            SoundManager.SetBgmVolume(0.9f);

            _currentScene.LoadContent();
        }

        public void UnloadContent()
        {
            _currentScene.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (_isTransitioning)
                UpdateTransition(gameTime);
            else if (InputManager.Instace.KeyPressed(Keys.F2))
                DebugMode = !DebugMode;

            _currentScene.Update(gameTime);
            if (_currentScene is SceneGameover)
            {
                _bloomFilter.BloomStrengthMultiplier = 0.1f;
            }
            else if (PlanetManager.Instance.Paused || _currentScene is SceneTitle)
            {
                _bloomFilter.BloomStrengthMultiplier = 0.0f;
            }
            else
            {
                _bloomFilter.BloomStrengthMultiplier = 0.5f;
            }
            UpdateCameraShake(gameTime);

            SoundManager.Update();
        }

        private void UpdateCameraShake(GameTime gameTime)
        {
            if (_shakeEnabled)
            {
                if (_shakeElapsed > _shakeDuration)
                {
                    _shakeEnabled = false;
                }
                else
                {
                    _shakeElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    float percentComplete = _shakeElapsed / _shakeDuration;
                    float damper = 1.0f - MathHelper.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
                    float x = (float)_rand.NextDouble() * 2.0f - 1.0f;
                    float y = (float)_rand.NextDouble() * 2.0f - 1.0f;
                    x *= _shakeMagnitude * damper;
                    y *= _shakeMagnitude * damper;
                    _shakeOffset = new Vector2(x, y);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the scene to the scenes render target
            var transformMatrix = ViewportAdapter.GetScaleMatrix() * Matrix.CreateTranslation(_shakeOffset.X, _shakeOffset.Y, 0);

            GraphicsDevice.SetRenderTarget(_sceneRenderTarget);
            _currentScene.Draw(spriteBatch, transformMatrix);

            // Draw the scanlines to the scanlines render target
            GraphicsDevice.SetRenderTarget(_scanlinesRenderTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, effect: _scanlinesEffect);
            spriteBatch.Draw(_sceneRenderTarget, _sceneRenderTarget.Bounds, Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            // Draw everything to the screen with the bloom filter
            Texture2D bloomTexture = _bloomFilter.Draw(_scanlinesRenderTarget, (int)VirtualSize.X, (int)VirtualSize.Y);
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatch.Draw(_scanlinesRenderTarget, _sceneRenderTarget.Bounds, Color.White);
            spriteBatch.Draw(bloomTexture, _sceneRenderTarget.Bounds, Color.White * 0.9f);
            spriteBatch.End();

            // Draw Transition and debug values
            spriteBatch.Begin();
            spriteBatch.Draw(_transitionImage.TextureRegion.Texture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White * _transitionImage.Alpha);
            _currentScene.DrawDebugValues(spriteBatch);
            spriteBatch.End();
        }

        public void ChangeScene(string newScene)
        {
            if (_isTransitioning) return;
            _newScene = (SceneBase)Activator.CreateInstance(Type.GetType("LudumDare38.Scenes." + newScene));
            InitializeTransition();
        }

        public void StartCameraShake(float magnitude, float duration)
        {
            _shakeMagnitude = _shakeEnabled ? _shakeMagnitude + magnitude * 0.1f : magnitude;
            _shakeEnabled = true;
            _shakeElapsed = 0;
            _shakeDuration = duration;
        }

        private void InitializeTransition()
        {
            _transitionImage.Alpha = 0;
            _transitionImage.IsVisible = true;
            _isTransitioning = true;
            _beginTransitionFade = true;
        }

        private void UpdateTransition(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_beginTransitionFade)
            {
                if (_transitionImage.Alpha < 1.0f)
                    _transitionImage.Alpha += deltaTime / 200f;
                else
                    _beginTransitionFade = false;
            }
            else
            {
                if (_newScene != null)
                {
                    _currentScene.UnloadContent();
                    _currentScene = _newScene;
                    _currentScene.LoadContent();
                    _newScene = null;
                }

                if (_transitionImage.Alpha > 0.0f)
                {
                    _transitionImage.Alpha -= deltaTime / 200f;
                }
                else
                {
                    _transitionImage.IsVisible = false;
                    _isTransitioning = false;
                }
            }
        }
    }
}
