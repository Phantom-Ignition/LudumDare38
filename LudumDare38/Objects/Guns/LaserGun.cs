using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Objects.Guns
{
    class LaserGun : GameGunBase
    {
        private Laser _laser;
        public Laser Laser => _laser;
        
        private bool _isShooting;

        public LaserGun(int orbitLevel, GunType gunType, float angle) : base(orbitLevel, gunType, angle)
        {
            _cooldown = 1200.0f;
            CreateLaser();
        }

        private void CreateLaser()
        {
            var laserTexture = ImageManager.LoadGun("LaserBeam");
            _laser = new Laser(laserTexture);
        }

        protected override void CreateSprite()
        {
            var texture = ImageManager.LoadGun("Laser");
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(12.5f, 12.5f);

            _sprite.CreateFrameList("stand", 50);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 25, 25));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 25, 25),
                new Rectangle(25, 0, 25, 25),
                new Rectangle(50, 0, 25, 25),
                new Rectangle(75, 0, 25, 25)
            });

            _sprite.CreateFrameList("preparation", 100);
            _sprite.AddCollider("preparation", new Rectangle(0, 0, 25, 25));
            _sprite.AddFrames("preparation", new List<Rectangle>()
            {
                new Rectangle(0, 25, 25, 25),
                new Rectangle(25, 25, 25, 25)
            });

            _sprite.CreateFrameList("loading", 100);
            _sprite.AddCollider("loading", new Rectangle(0, 0, 25, 25));
            _sprite.AddFrames("loading", new List<Rectangle>()
            {
                new Rectangle(0, 50, 25, 25),
                new Rectangle(25, 50, 25, 25)
            });

            _sprite.CreateFrameList("shoting", 100);
            _sprite.AddCollider("shoting", new Rectangle(0, 0, 25, 25));
            _sprite.AddFrames("shoting", new List<Rectangle>()
            {
                new Rectangle(0, 75, 25, 25),
                new Rectangle(25, 75, 25, 25)
            });

            _sprite.CreateFrameList("recover", 40);
            _sprite.AddCollider("recover", new Rectangle(0, 0, 25, 25));
            _sprite.AddFrames("recover", new List<Rectangle>()
            {
                new Rectangle(0, 100, 25, 25),
                new Rectangle(25, 100, 25, 25),
                new Rectangle(50, 100, 25, 25),
            });
        }

        public override void Update(GameTime gameTime, float rotation, float floating)
        {
            base.Update(gameTime, rotation, floating);
            _laser.Update(gameTime, _sprite.Position, _sprite.Rotation);
            UpdateLaserShot();
        }

        private void UpdateLaserShot()
        {
            var spriteFrame = _sprite.CurrentFrameList;
            if (_sprite.Looped)
            {
                switch (spriteFrame)
                {
                    case "preparation":
                        _sprite.SetFrameList("loading");
                        break;
                    case "loading":
                        _sprite.SetFrameList("shoting");
                        _isShooting = true;
                        _laser.Sprite.IsVisible = true;
                        _laser.Sprite.Scale = Vector2.One;
                        _laser.Sprite.SetFrameList("attack");
                        _laser.Sprite.ResetCurrentFrameList();
                        break;
                    case "recover":
                        _sprite.SetFrameList("stand");
                        break;
                }
            }

            if (_isShooting)
            {
                var y = Math.Min(_laser.Sprite.Scale.Y * 2f, SceneManager.Instance.VirtualSize.X / 2);
                _laser.Sprite.Scale = new Vector2(1, y);
                if (_laser.Sprite.Looped)
                {
                    if (_laser.Sprite.CurrentFrameList == "attack")
                    {
                        _laser.Sprite.SetFrameList("dispose");
                        _sprite.SetFrameList("recover");
                    }
                    else if (_laser.Sprite.CurrentFrameList == "dispose")
                    {
                        _isShooting = false;
                        _laser.Sprite.IsVisible = false;
                    }
                }
            }
        }

        public override bool Shot(out GameProjectile projectile)
        {
            base.Shot(out projectile);

            _sprite.SetFrameList("preparation");

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _laser.Sprite.Draw(spriteBatch, _laser.Sprite.Position);
            base.Draw(spriteBatch);
        }
    }
}
