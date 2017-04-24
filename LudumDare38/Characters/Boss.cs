using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using LudumDare38.Objects;
using LudumDare38.Managers;

namespace LudumDare38.Characters
{
    class Boss : EnemyBase, ISuicidable
    {
        public override EnemyType Type => EnemyType.Boss;
        protected override float InitialImmunityTime => 50.0f;
        protected override HslColor EnemyColor => new HslColor(317, 0.55f, 0.39f);

        private float _shotCooldown;

        private List<TripleShooter> _enemiesQueued;
        public List<TripleShooter> EnemiesQueued => _enemiesQueued;
        private bool _needCollectExplosionDamage;

        public Boss(Texture2D texture) : base(texture)
        {
            _hp = 20;
            _gold = 20;
            _shotCooldown = 2000f;
            _enemiesQueued = new List<TripleShooter>();
        }

        protected override void CreateSprite(Texture2D texture)
        {
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(85f, 85f);

            _sprite.CreateFrameList("stand", 80);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 170, 170));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 170, 170),
                new Rectangle(170, 0, 170, 170),
                new Rectangle(340, 0, 170, 170),
                new Rectangle(510, 0, 170, 170)
            });

            _sprite.CreateFrameList("shooting", 100, false);
            _sprite.AddCollider("shooting", new Rectangle(0, 0, 170, 170));
            _sprite.AddFrames("shooting", new List<Rectangle>()
            {
                new Rectangle(0, 170, 170, 170),
                new Rectangle(170, 170, 170, 170),
                new Rectangle(340, 170, 170, 170),
                new Rectangle(510, 170, 170, 170)
            });
        }

        public int ContactDamage()
        {
            return 100;
        }

        public void CollectExplosionDamage()
        {
            _needCollectExplosionDamage = false;
        }

        public bool NeedCollectExplosionDamage()
        {
            return _needCollectExplosionDamage;
        }

        private void QueueEnemies()
        {
            var rotationIncrease = (float)Math.PI / 10;
            var a = -1;
            for (var i = 0; i < 3; i++, a++)
            {
                var rotation = _sprite.Rotation + rotationIncrease * a;
                var enemy = new TripleShooter(ImageManager.LoadEnemy("TripleShooter"));
                enemy.Position = _position + new Vector2((float)Math.Cos(rotation) * 130, (float)Math.Sin(rotation) * 200);
                var velocity = enemy.Velocity;
                var angle = new Vector2(velocity.X * enemy.Sprite.TextureRegion.Width / 2, velocity.Y * enemy.Sprite.TextureRegion.Height / 2);
                enemy.Position -= angle;
                enemy.Sprite.Rotation = (float)Math.Atan2(angle.Y, angle.X);
                _enemiesQueued.Add(enemy);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!_dying)
            {
                UpdateMovement(gameTime);
            }
            UpdateSprite();
        }

        private void UpdateMovement(GameTime gameTime)
        {
            var distanceToTarget = Math.Sqrt(Math.Pow(_target.X - _position.X, 2) + Math.Pow(_target.Y - _position.Y, 2));
            if (distanceToTarget > 230 || _shotCooldown > 0.0f)
            {
                _position += _velocity * (distanceToTarget > 230 ? 5 : 0.2f);
                _sprite.Rotation = (float)Math.Atan2(_velocity.Y, _velocity.X);
            }

            var distance = Math.Sqrt(Math.Pow(_target.X - _position.X, 2) + Math.Pow(_target.Y - _position.Y, 2));
            if (distance < GamePlanet.Radius + _sprite.GetColliderWidth() / 3)
            {
                Explode();
                return;
            }

            UpdateShots(gameTime);
            _sprite.Position = _position;
        }

        private void UpdateShots(GameTime gameTime)
        {
            if (_shotCooldown <= 0.0f)
            {
                _sprite.SetFrameList("shooting");
            }
            else
            {
                _shotCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        private void UpdateSprite()
        {
            if (_sprite.CurrentFrameList == "shooting")
            {
                if (_shotCooldown <= 0.0f && _sprite.CurrentFrame == 2)
                {
                    _shotCooldown = 5000.0f;
                    QueueEnemies();
                }
                if (_sprite.Looped)
                {
                    _sprite.SetFrameList("stand");
                }
            }
            _sprite.Effect = SpriteEffects.None;
            if (_velocity.X < 0)
            {
                _sprite.Effect = SpriteEffects.FlipVertically;
            }
        }

        public void Explode()
        {
            _needCollectExplosionDamage = true;
            GetDamaged(999);
        }
    }
}
