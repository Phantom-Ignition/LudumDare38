using LudumDare38.Helpers;
using LudumDare38.Helpers.TinyTween;
using LudumDare38.Managers;
using LudumDare38.Objects.Guns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private bool _complete;
        public bool Complete => _complete;

        private bool _exiting;

        //--------------------------------------------------
        // Tweens

        private List<ITween> _tweens;
        private Vector2Tween _selectionTween;
        private Vector2Tween _cursorTween;
        private FloatTween _backgroundTween;
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
        // Buy Phase

        private int _cursorIndex;
        private int[] _cursorIndexOffset;

        //--------------------------------------------------
        // Install Gun Phase

        private GameGunBase _placeholderGun;
        private int _orbitIndex;
        private int _angleIndex;

        //--------------------------------------------------
        // SEs

        private SoundEffect _buySe;

        //----------------------//------------------------//

        public UpgradeSelectionHelper()
        {
            _tweens = new List<ITween>();
            _selectionTween = new Vector2Tween();
            _cursorTween = new Vector2Tween();
            _cursorIndexOffset = new[] { -87, 0, 87 };
            _backgroundTween = new FloatTween();
            _buySe = SoundManager.LoadSe("Buy");
        }

        public void Activate()
        {
            _active = true;
            _complete = false;
            _exiting = false;

            SetPhase(Phase.Buy);

            _backgroundTween.Start(0.0f, 0.5f, 1.0f, ScaleFuncs.Linear);

            _tweens.Add(_selectionTween);
            _tweens.Add(_cursorTween);
            _tweens.Add(_backgroundTween);
        }

        public void Deactivate()
        {
            _active = false;
            var igFrom = SceneManager.Instance.VirtualSize / 2 + 170 * Vector2.UnitY;
            var igTo = SceneManager.Instance.VirtualSize / 2 + 340 * Vector2.UnitY;
            _selectionTween.Start(igFrom, igTo, .5f, ScaleFuncs.SineEaseOut);
            _backgroundTween.Start(0.5f, 0.0f, 1.0f, ScaleFuncs.Linear);
        }

        public void LoadContent()
        {
            var bgTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            bgTexture.SetData(new Color[] { Color.Black });
            _blackBackgroundSprite = new Sprite(bgTexture);
            _blackBackgroundSprite.Scale = SceneManager.Instance.VirtualSize;
            _blackBackgroundSprite.Position = SceneManager.Instance.VirtualSize / 2;
            _blackBackgroundSprite.Alpha = 0.0f;

            _hudBackSprite = new Sprite(ImageManager.LoadHudUpgrade("Background"));
            _hudBackSprite.Position = new Vector2(-_hudBackSprite.TextureRegion.Width, -_hudBackSprite.TextureRegion.Height);
            _cursorSprite = new Sprite(ImageManager.LoadHudUpgrade("Cursor"));

            _guns = new GunsData[]
            {
                new GunsData()
                {
                    Price = 10,
                    Enabled = new Sprite(ImageManager.LoadHudUpgrade("GunEnabled")),
                    Disabled = new Sprite(ImageManager.LoadHudUpgrade("GunDisabled"))
                },
                new GunsData()
                {
                    Price = 20,
                    Enabled = new Sprite(ImageManager.LoadHudUpgrade("LaserEnabled")),
                    Disabled = new Sprite(ImageManager.LoadHudUpgrade("LaserDisabled"))
                },
                new GunsData()
                {
                    Price = 10,
                    Enabled = new Sprite(ImageManager.LoadHudUpgrade("ShieldEnabled")),
                    Disabled = new Sprite(ImageManager.LoadHudUpgrade("ShieldDisabled"))
                }
            };
            var texture = _guns[0].Enabled.TextureRegion.Texture; // All the images have the same height and width
            for (var i = 0; i < _guns.Length; i++)
            {
                _guns[i].Enabled.Position = new Vector2(-texture.Width, -texture.Height);
                _guns[i].Disabled.Position = new Vector2(-texture.Width, -texture.Height);
            }
        }

        private void RefreshGunsData()
        {
            var gold = PlanetManager.Instance.Gold;
            var hasSpaceToMoreGuns = PlanetManager.Instance.AvailableOrbits.Length > 0;
            for (var i = 0; i < _guns.Length; i++)
            {
                _guns[i].IsEnabled = gold >= _guns[i].Price && hasSpaceToMoreGuns;
            }
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
                    RefreshGunsData();
                    break;

                case Phase.InstallGun:
                    var igFrom = SceneManager.Instance.VirtualSize / 2 + 170 * Vector2.UnitY;
                    var igTo = SceneManager.Instance.VirtualSize / 2 + 340 * Vector2.UnitY;
                    _selectionTween.Start(igFrom, igTo, .5f, ScaleFuncs.SineEaseOut);

                    _orbitIndex = 0;
                    _angleIndex = 0;

                    var firstOrbitField = PlanetManager.Instance.AvailableOrbits[0];
                    switch (_cursorIndex)
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
            if (!_active && _selectionTween.CurrentTime >= _selectionTween.Duration) return;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update tweens
            _tweens.ForEach(tween => tween.Update(deltaTime));

            // Update position of layout
            var position = _selectionTween.CurrentValue;
            _hudBackSprite.Position = position;
            foreach (var gun in _guns)
            {
                gun.SetPosition(position);
            }

            // Update position of the cursor
            var vec = new Vector2(position.X + _cursorIndexOffset[_cursorIndex], position.Y - 65 + (float)Math.Sin(_cursorFloatingAngle) * 5);
            _cursorSprite.Position = vec;
            _cursorFloatingAngle = (_cursorFloatingAngle + (float)Math.PI / 180 * deltaTime * 500) % ((float)Math.PI * 2);
            _cursorSprite.IsVisible = _guns.Any(gun => gun.IsEnabled);

            // Update input
            UpdateInput();

            // Update install gun
            if (_phase == Phase.InstallGun)
            {
                _placeholderGun.Update(gameTime, rotation, floating);
            }

            // Update background
            _blackBackgroundSprite.Alpha = _backgroundTween.CurrentValue;
        }

        private void UpdateInput()
        {
            if (_complete) return;
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
            var lastIndex = _cursorIndex;
            if (InputManager.Instace.KeyPressed(Keys.Left))
            {
                _cursorIndex = _cursorIndex - 1 < 0 ? 2 : _cursorIndex - 1;
                SoundManager.PlaySelectSe();
            }
            if (InputManager.Instace.KeyPressed(Keys.Right))
            {
                _cursorIndex = _cursorIndex + 1 > 2 ? 0 : _cursorIndex + 1;
                SoundManager.PlaySelectSe();
            }
            if (InputManager.Instace.KeyPressed(Keys.Z))
            {
                if (_guns[_cursorIndex].IsEnabled)
                {
                    HandleGunBuy(_cursorIndex);
                }
            }
            if (InputManager.Instace.KeyPressed(Keys.X))
            {
                SoundManager.PlaySelectSe();
                _exiting = true;
                _complete = true;
                Deactivate();
            }
        }

        private void HandleGunBuy(int index)
        {
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
                    var newOrbitFields = PlanetManager.Instance.AvailableOrbits.Where(of => of.OrbitLevel == orbitLevels[_orbitIndex]).ToList();
                    _angleIndex = newOrbitFields.FindIndex(of => of.Angle == lastAngle);
                    _placeholderGun.SetOrbitLevel(orbitLevels[_orbitIndex]);
                    _placeholderGun.SetAngle(lastAngle);
                }
                else
                {
                    _orbitIndex = lastIndex;
                }
            }

            // Update Confirm
            if (InputManager.Instace.KeyPressed(Keys.Z))
            {
                PlanetManager.Instance.Gold -= _guns[_cursorIndex].Price;
                PlanetManager.Instance.CreateGun(_placeholderGun);
                _placeholderGun = null;
                SetPhase(Phase.Buy);
                _buySe.PlaySafe();
            }

            // Update Cancel
            if (InputManager.Instace.KeyPressed(Keys.X))
            {
                SoundManager.PlayConfirmSe();
                SetPhase(Phase.Buy);
                _placeholderGun = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            if (_active || (!_active && _selectionTween.CurrentTime < _selectionTween.Duration))
            {
                spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(_hudBackSprite);
                foreach (var gun in _guns)
                {
                    spriteBatch.Draw(gun.IsEnabled ? gun.Enabled : gun.Disabled);
                }
                spriteBatch.Draw(_cursorSprite);

                var a = _selectionTween.CurrentTime / _selectionTween.Duration;
                if (_exiting) a = 1 - a;

                if (_phase == Phase.Buy)
                {
                    // Draw instructions
                    var font = SceneManager.Instance.GameFontSmall;
                    var texts = new string[]
                    {
                        "Use ← and → to move the cursor",
                        "Press Z to select an equip",
                        "Press X when you are done",
                    };
                    for (var i = 0; i < texts.Length; i++)
                    {
                        var posx = (SceneManager.Instance.VirtualSize.X - font.MeasureString(texts[i]).X) / 2;
                        spriteBatch.DrawString(font, texts[i], new Vector2(posx, 20 + 25 * i), Color.White * a);
                    }

                    var bellowTexts = new string[]
                    {
                        "Gold: " + PlanetManager.Instance.Gold.ToString()
                    };
                    for (var i = 0; i < bellowTexts.Length; i++)
                    {
                        var posx = (SceneManager.Instance.VirtualSize.X - font.MeasureString(bellowTexts[i]).X) / 2;
                        spriteBatch.DrawString(font, bellowTexts[i], new Vector2(posx, 470), Color.White * a);
                    }
                }
                else
                {
                    _placeholderGun.Sprite.Draw(spriteBatch, _placeholderGun.Sprite.Position);

                    // Draw instructions
                    var font = SceneManager.Instance.GameFontSmall;
                    var texts = new string[]
                    {
                        "Use ← and → to change the angle of",
                        "the equip",
                        "Press Z to buy the equip",
                        "Press X to cancel",
                    };
                    for (var i = 0; i < texts.Length; i++)
                    {
                        var posx = (SceneManager.Instance.VirtualSize.X - font.MeasureString(texts[i]).X) / 2;
                        var yInc = i == 1 ? 22 : 25;
                        spriteBatch.DrawString(font, texts[i], new Vector2(posx, 20 + yInc * i), Color.White * a);
                    }

                    var bellowTexts = new string[]
                    {
                        "Use ↑ and ↓ to change the orbit of",
                        "the equip (from 1 to 3)",
                    };
                    for (var i = 0; i < bellowTexts.Length; i++)
                    {
                        var posx = (SceneManager.Instance.VirtualSize.X - font.MeasureString(bellowTexts[i]).X) / 2;
                        var yInc = i == 1 ? 22 : 25;
                        spriteBatch.DrawString(font, bellowTexts[i], new Vector2(posx, 430 + yInc * i), Color.White * a);
                    }
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
