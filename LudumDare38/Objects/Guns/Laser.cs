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
    class Laser : GameGunBase
    {
        private CharacterSprite _laserSprite;
        private bool _isShooting;

        public Laser(int orbitLevel, GunType gunType, float angle) : base(orbitLevel, gunType, angle)
        {
            _cooldown = 1200.0f;
            CreateLaserSprite();
        }

        private void CreateLaserSprite()
        {
            var laserTexture = ImageManager.LoadGun("LaserBeam");
            _laserSprite = new CharacterSprite(laserTexture);
            _laserSprite.Origin = new Vector2(4.5f, 0f);
            _laserSprite.Scale = new Vector2(1, 100);

            _laserSprite.CreateFrameList("stand", 100);
            _laserSprite.AddCollider("stand", new Rectangle(0, 0, 9, 2));
            _laserSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 9, 2)
            });

            _laserSprite.CreateFrameList("attack", 400);
            _laserSprite.AddCollider("attack", new Rectangle(0, 0, 9, 2));
            _laserSprite.AddFrames("attack", new List<Rectangle>()
            {
                new Rectangle(0, 0, 9, 2)
            });

            _laserSprite.CreateFrameList("dispose", 40);
            _laserSprite.AddCollider("dispose", new Rectangle(0, 0, 9, 2));
            _laserSprite.AddFrames("dispose", new List<Rectangle>()
            {
                new Rectangle(9, 0, 9, 2),
                new Rectangle(18, 0, 9, 2),
                new Rectangle(27, 0, 9, 2)
            });

            _laserSprite.IsVisible = false;
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
            _laserSprite.Update(gameTime);
            _laserSprite.Position = _sprite.Position;
            _laserSprite.Rotation = _sprite.Rotation + (float)Math.PI;
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
                        _laserSprite.IsVisible = true;
                        _laserSprite.Scale = Vector2.One;
                        _laserSprite.SetFrameList("attack");
                        _laserSprite.ResetCurrentFrameList();
                        break;
                    case "recover":
                        _sprite.SetFrameList("stand");
                        break;
                }
            }

            if (_isShooting)
            {
                _laserSprite.Scale = new Vector2(1, _laserSprite.Scale.Y * 2f);
                if (_laserSprite.Looped)
                {
                    if (_laserSprite.CurrentFrameList == "attack")
                    {
                        _laserSprite.SetFrameList("dispose");
                        _sprite.SetFrameList("recover");
                    }
                    else if (_laserSprite.CurrentFrameList == "dispose")
                    {
                        _isShooting = false;
                        _laserSprite.IsVisible = false;
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
            _laserSprite.Draw(spriteBatch, _laserSprite.Position);
            base.Draw(spriteBatch);
        }
    }
}
