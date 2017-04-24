using LudumDare38.Helpers;
using LudumDare38.Helpers.TinyTween;
using LudumDare38.Managers;
using LudumDare38.Objects.Guns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Scenes
{
    class UpgradeSelectionHelper : IDisposable
    {
        //--------------------------------------------------
        // WeaponSprites

        private struct GunsData
        {
            public bool IsEnabled;
            public Sprite Enabled;
            public Sprite Disabled;
            public int Price;

            public void SetPosition(Vector2 position)
            {
                Enabled.Position = position;
                Disabled.Position = position;
            }
        }

        //--------------------------------------------------
        // Sprites

        private Sprite _blackBackgroundSprite;
        private Sprite _hudBackSprite;
        private Sprite _cursorSprite;
        private GunsData[] _guns;

        //--------------------------------------------------
        // Mechanic

        private bool _active;
        public bool IsActive => _active;

        private bool _show;
        private int _index;
        private int[] _cursorIndexOffset;

        //--------------------------------------------------
        // Tweens

        private List<ITween> _tweens;
        private Vector2Tween _selectionTween;
        private Vector2Tween _cursorTween;
        private float _cursorFloatingAngle;

        //--------------------------------------------------
        // Phase

        private Phase _phase;
        private enum Phase
        {
            Buy,
            InstallGun
        }

        //--------------------------------------------------
        // Install Gun Phase

        private GameGunBase _placeholderGun;
        private int _orbitIndex;
        private int _angleIndex;

        //----------------------//------------------------//

        public UpgradeSelectionHelper()
        {
            _tweens = new List<ITween>();
            _selectionTween = new Vector2Tween();
            _cursorTween = new Vector2Tween();
            _cursorIndexOffset = new[] { -87, 0, 87 };
        }

        public void Activate()
        {
            _active = true;
            _show = true;

            SetPhase(Phase.Buy);

            _tweens.Add(_selectionTween);
            _tweens.Add(_cursorTween);
        }

        public void LoadContent()
        {
            var bgTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            bgTexture.SetData(new Color[] { Color.Black });
            _blackBackgroundSprite = new Sprite(bgTexture);
            _blackBackgroundSprite.Scale = SceneManager.Instance.VirtualSize;
            _blackBackgroundSprite.Position = SceneManager.Instance.VirtualSize / 2;
            _blackBackgroundSprite.Alpha = 0.5f;

            _hudBackSprite = new Sprite(ImageManager.LoadHudUpgrade("Background"));
            _cursorSprite = new Sprite(ImageManager.LoadHudUpgrade("Cursor"));

            var gold = PlanetManager.Instance.Gold;
            _guns = new GunsData[]
            {
                new GunsData()
                {
                    Price = 10,
                    IsEnabled = gold >= 10,
                    Enabled = new Sprite(ImageManager.LoadHudUpgrade("GunEnabled")),
                    Disabled = new Sprite(ImageManager.LoadHudUpgrade("GunDisabled"))
                },
                new GunsData()
                {
                    Price = 20,
                    IsEnabled = gold >= 20,
                    Enabled = new Sprite(ImageManager.LoadHudUpgrade("LaserEnabled")),
                    Disabled = new Sprite(ImageManager.LoadHudUpgrade("LaserDisabled"))
                },
                new GunsData()
                {
                    Price = 10,
                    IsEnabled = gold >= 10,
                    Enabled = new Sprite(ImageManager.LoadHudUpgrade("ShieldEnabled")),
                    Disabled = new Sprite(ImageManager.LoadHudUpgrade("ShieldDisabled"))
                }
            };
        }

        private void SetPhase(Phase phase)
        {
            _phase = phase;
            switch (phase)
            {
                case Phase.Buy:
                    var from = SceneManager.Instance.VirtualSize / 2 + 255 * Vector2.UnitY;
                    var to = SceneManager.Instance.VirtualSize / 2 + 170 * Vector2.UnitY;
                    _selectionTween.Start(from, to, .5f, ScaleFuncs.SineEaseOut);
                    break;

                case Phase.InstallGun:
                    var igFrom = SceneManager.Instance.VirtualSize / 2 + 170 * Vector2.UnitY;
                    var igTo = SceneManager.Instance.VirtualSize / 2 + 340 * Vector2.UnitY;
                    _selectionTween.Start(igFrom, igTo, .5f, ScaleFuncs.SineEaseOut);

                    _orbitIndex = 0;
                    _angleIndex = 0;

                    var firstOrbitField = PlanetManager.Instance.AvailableOrbits[0];
                    switch (_index)
                    {
                        case 0: _placeholderGun = new BasicGun(GunType.Basic, firstOrbitField); break;
                        case 1: _placeholderGun = new LaserGun(GunType.LaserGun, firstOrbitField); break;
                        case 2: _placeholderGun = new Shield(GunType.Shield, firstOrbitField); break;
                    }
                    break;
            }
        }

        public void Update(GameTime gameTime, float rotation, float floating)
        {
            if (!_active) return;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update position of layout
            var position = _selectionTween.CurrentValue;
            _hudBackSprite.Position = position;
            foreach (var gun in _guns)
            {
                gun.SetPosition(position);
            }

            // Update position of the cursor
            var vec = new Vector2(position.X + _cursorIndexOffset[_index], position.Y - 65 + (float)Math.Sin(_cursorFloatingAngle) * 5);
            _cursorSprite.Position = vec;
            _cursorFloatingAngle = (_cursorFloatingAngle + (float)Math.PI / 180 * deltaTime * 500) % ((float)Math.PI * 2);

            // Update tweens
            UpdateInput();

            // Update install gun
            if (_phase == Phase.InstallGun)
            {
                _placeholderGun.Update(gameTime, rotation, floating);
            }

            // Update tweens
            _tweens.ForEach(tween => tween.Update(deltaTime));
        }

        private void UpdateInput()
        {
            switch (_phase)
            {
                case Phase.Buy:
                    UpdateBuyInput();
                    break;
                case Phase.InstallGun:
                    UpdateInstallGunInput();
                    break;
            }
        }

        private void UpdateBuyInput()
        {
            var lastIndex = _index;
            if (InputManager.Instace.KeyPressed(Keys.Left))
            {
                _index = _index - 1 < 0 ? 2 : _index - 1;
            }
            if (InputManager.Instace.KeyPressed(Keys.Right))
            {
                _index = _index + 1 > 2 ? 0 : _index + 1;
            }
            if (InputManager.Instace.KeyPressed(Keys.Enter))
            {
                if (PlanetManager.Instance.Gold >= _guns[_index].Price)
                {
                    HandleGunBuy(_index);
                }
            }
        }

        private void HandleGunBuy(int index)
        {
            PlanetManager.Instance.Gold -= _guns[_index].Price;

            for (var i = 0; i < _guns.Length; i++)
            {
                _guns[i].IsEnabled = PlanetManager.Instance.Gold >= _guns[i].Price;
            }
            
            SetPhase(Phase.InstallGun);
        }

        private void UpdateInstallGunInput()
        {
            // Update angle choice
            var lastIndex = _angleIndex;
            var orbitLevels = PlanetManager.Instance.AvailableOrbits.Select(orbitField => orbitField.OrbitLevel).Distinct().ToList();
            var availableOrbitFields = PlanetManager.Instance.AvailableOrbits.Where(orbit => orbit.OrbitLevel == orbitLevels[_orbitIndex]).ToList();
            var maxIndex = availableOrbitFields.Count - 1;
            if (InputManager.Instace.KeyPressed(Keys.Left))
            {
                _angleIndex = _angleIndex - 1 < 0 ? maxIndex : _angleIndex - 1;
            }
            if (InputManager.Instace.KeyPressed(Keys.Right))
            {
                _angleIndex = _angleIndex + 1 > maxIndex ? 0 : _angleIndex + 1;
            }
            if (lastIndex != _angleIndex)
            {
                var angle = availableOrbitFields[_angleIndex].Angle;
                _placeholderGun.SetAngle(angle);
            }

            // Update orbit choice
            lastIndex = _orbitIndex;
            maxIndex = orbitLevels.Count - 1;
            if (InputManager.Instace.KeyPressed(Keys.Down))
            {
                _orbitIndex = _orbitIndex - 1 < 0 ? maxIndex : _orbitIndex - 1;
            }
            if (InputManager.Instace.KeyPressed(Keys.Up))
            {
                _orbitIndex = _orbitIndex + 1 > maxIndex ? 0 : _orbitIndex + 1;
            }
            if (lastIndex != _orbitIndex)
            {
                var lastAngle = availableOrbitFields[_angleIndex].Angle;
                var newAngleIndex = PlanetManager.Instance.AngleIndex(lastAngle);
                var newOrbitField = PlanetManager.Instance.Orbits[newAngleIndex + _orbitIndex * PlanetManager.Instance.NumPossibleAngles];
                if (newOrbitField.Available)
                {
                    _placeholderGun.SetOrbitLevel(orbitLevels[_orbitIndex]);
                    var newOrbitFields = PlanetManager.Instance.AvailableOrbits.Where(of => of.OrbitLevel == orbitLevels[_orbitIndex]).ToList();
                    _angleIndex = newOrbitFields.FindIndex(of => of.Angle == lastAngle);
                }
                else
                {
                    _orbitIndex = lastIndex;
                }
            }

            // Update Confirm
            if (InputManager.Instace.KeyPressed(Keys.Enter))
            {
                SetPhase(Phase.Buy);
                PlanetManager.Instance.CreateGun(_placeholderGun);
                _placeholderGun = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            if (_show)
            {
                spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(_blackBackgroundSprite);
                spriteBatch.Draw(_hudBackSprite);
                foreach (var gun in _guns)
                {
                    spriteBatch.Draw(gun.IsEnabled ? gun.Enabled : gun.Disabled);
                }
                spriteBatch.Draw(_cursorSprite);

                if (_phase == Phase.InstallGun)
                {
                    _placeholderGun.Sprite.Draw(spriteBatch, _placeholderGun.Sprite.Position);
                }

                spriteBatch.End();
            }
        }

        public void Dispose()
        {
            _blackBackgroundSprite.TextureRegion.Texture.Dispose();
        }
    }
}
