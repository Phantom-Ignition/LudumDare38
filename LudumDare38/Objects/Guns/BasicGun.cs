using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LudumDare38.Objects.Guns
{
    class BasicGun : GameGunBase
    {
        public BasicGun(GunType gunType, OrbitField orbitField) : base(gunType, orbitField)
        {
            _cooldown = 90.0f;
        }

        protected override void CreateSprite()
        {
            var texture = ImageManager.LoadGun("Basic");
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(10f, 10f);

            _sprite.CreateFrameList("stand", 0);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 20, 20));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 20, 20)
            });

            _sprite.CreateFrameList("shot", 30);
            _sprite.AddCollider("shot", new Rectangle(0, 0, 20, 20));
            _sprite.AddFrames("shot", new List<Rectangle>()
            {
                new Rectangle(20, 0, 20, 20),
                new Rectangle(40, 0, 20, 20),
                new Rectangle(60, 0, 20, 20)
            });
        }

        public override void Update(GameTime gameTime, float rotation, float floating)
        {
            base.Update(gameTime, rotation, floating);
            if (_sprite.CurrentFrameList == "shot" && _sprite.Looped)
            {
                _sprite.SetFrameList("stand");
            }
        }

        public override bool Shot(out GameProjectile projectile)
        {
            base.Shot(out projectile);
            _sprite.SetFrameList("shot");
            var rotation = _sprite.Rotation - (float)Math.PI / 2;
            var position = _sprite.Position - new Vector2(-(float)Math.Sin(rotation) * 10f, (float)Math.Cos(rotation) * 10f);
            projectile = new GameProjectile(ProjectileType.BasicProjectile, position, rotation, 5, 1, ProjectileSubject.FromPlayer);
            return true;
        }
    }
}
