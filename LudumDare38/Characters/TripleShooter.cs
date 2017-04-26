using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using LudumDare38.Objects;
using Microsoft.Xna.Framework;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework.Audio;
using LudumDare38.Managers;

namespace LudumDare38.Characters
{
    class TripleShooter : EnemyBase
    {
        public override EnemyType Type => EnemyType.TripleShooter;
        protected override float InitialImmunityTime => 50.0f;
        protected override HslColor EnemyColor => new HslColor(317, 0.55f, 0.39f);

        private float _shotCooldown;
        private SoundEffect _shotSe;

        private List<GameProjectile> _projectilesQueued;
        public List<GameProjectile> ProjectilesQueued => _projectilesQueued;

        public TripleShooter(Texture2D texture) : base(texture)
        {
            _hp = 2;
            _gold = 5;
            _projectilesQueued = new List<GameProjectile>();
            _shotSe = SoundManager.LoadSe("Alien_atk");
        }

        protected override void CreateSprite(Texture2D texture)
        {
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(60f, 40f);

            _sprite.CreateFrameList("stand", 80);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 80, 80));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 80, 80, 80),
                new Rectangle(80, 80, 80, 80),
                new Rectangle(160, 80, 80, 80),
                new Rectangle(240, 80, 80, 80)
            });

            _sprite.CreateFrameList("shooting", 100, false);
            _sprite.AddCollider("shooting", new Rectangle(0, 0, 80, 80));
            _sprite.AddFrames("shooting", new List<Rectangle>()
            {
                new Rectangle(0, 0, 80, 80),
                new Rectangle(80, 0, 80, 80),
                new Rectangle(160, 0, 80, 80),
                new Rectangle(240, 0, 80, 80)
            });

            _sprite.GenerateTextureData();
        }

        private void QueueProjectile()
        {
            _shotSe.PlaySafe();
            var rotation = _sprite.Rotation;
            var rotationIncrease = (float)Math.PI / 10;
            for (var i = -1; i < 2; i++)
            {
                var rot = _sprite.Rotation - rotationIncrease * i;
                var proj = new GameProjectile(ProjectileType.AlienProjectile, _position, rot, 2, 1, ProjectileSubject.FromEnemy);
                _projectilesQueued.Add(proj);
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
            if (distanceToTarget > 200)
            {
                _position += _velocity * 5;
                _sprite.Rotation = (float)Math.Atan2(_velocity.Y, _velocity.X);
            }
            else
            {
                UpdateShots(gameTime);
            }

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
                    _shotCooldown = 1500.0f;
                    QueueProjectile();
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
    }
}
