using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using LudumDare38.Objects;
using Microsoft.Xna.Framework.Audio;
using LudumDare38.Managers;

namespace LudumDare38.Characters
{
    class Kamikaze : EnemyBase, ISuicidable
    {
        public override EnemyType Type => EnemyType.Kamikaze;
        protected override float InitialImmunityTime => 50.0f;
        protected override HslColor EnemyColor => new HslColor(317, 0.55f, 0.39f);

        private bool _needCollectExplosionDamage;
        
        private SoundEffect _explosionSe;

        public Kamikaze(Texture2D texture) : base(texture)
        {
            _hp = 3;
            _gold = 5;
            _explosionSe = SoundManager.LoadSe("Explosion");
        }

        protected override void CreateSprite(Texture2D texture)
        {
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(50f, 20f);

            _sprite.CreateFrameList("stand", 80);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 100, 40));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 100, 40),
                new Rectangle(100, 0, 100, 40),
                new Rectangle(200, 0, 100, 40),
                new Rectangle(300, 0, 100, 40)
            });
        }

        public int ContactDamage()
        {
            return 1;
        }

        public void CollectExplosionDamage()
        {
            _needCollectExplosionDamage = false;
        }

        public bool NeedCollectExplosionDamage()
        {
            return _needCollectExplosionDamage;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!_dying)
            {
                UpdateMovement();
            }
        }

        private void UpdateMovement()
        {
            _position += _velocity * 1;

            var distance = Math.Sqrt(Math.Pow(_target.X - _position.X, 2) + Math.Pow(_target.Y - _position.Y, 2));
            if (distance < GamePlanet.Radius + _sprite.GetColliderWidth() / 2)
            {
                Explode();
                return;
            }

             _sprite.Rotation = (float)Math.Atan2(_velocity.Y, _velocity.X);
            if (_velocity.X < 0)
            {
                _sprite.Effect = SpriteEffects.FlipVertically;
            }
            _sprite.Position = _position;
        }

        public void Explode()
        {
            _needCollectExplosionDamage = true;
            GetDamaged(999);
            _explosionSe.PlaySafe();
        }
    }
}
