using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Particles;
using System.Collections.Generic;
using LudumDare38.Objects;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Particles.Modifiers;

namespace LudumDare38.Characters
{
    //--------------------------------------------------
    // Enemy Type

    public enum EnemyType
    {
        None,
        Kamikaze,
        Shooter
    }

    abstract class EnemyBase : KillableObject
    {
        //--------------------------------------------------
        // Character sprite

        protected CharacterSprite _sprite;
        public CharacterSprite Sprite => _sprite;
        private Color[] _spriteTextureData;
        public Color[] SpriteTextureData => _spriteTextureData;

        //--------------------------------------------------
        // Target

        protected Vector2 _target;

        //--------------------------------------------------
        // Positioning

        protected Vector2 _velocity;
        public Vector2 Velocity => _velocity;

        protected Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _sprite.Position = value;
                _velocity = _target - _position;
                _velocity.Normalize();
                RecreateShotParticles();
            }
        }

        protected Vector2 _knockbackAcceleration;

        //--------------------------------------------------
        // Particles stuff
        
        protected List<ParticleEffect> _particles;
        protected Texture2D _particlesTexture;
        protected float _receivedShotRotation;

        protected virtual HslColor EnemyColor => new HslColor(0, 0, 0);
        protected ParticleEffect _shotParticles;
        protected ParticleEffect _deathParticles;

        //--------------------------------------------------
        // Bouding box

        public Rectangle BoundingBox
        {
            get
            {

                var w = _sprite.GetColliderWidth();
                var h = _sprite.GetColliderHeight();
                return new Rectangle((int)_position.X - w / 2,
                    (int)_position.Y - h / 2,
                    w,
                    h);
            }
        }

        //----------------------//------------------------//

        public EnemyBase(Texture2D texture)
        {
            _target = SceneManager.Instance.VirtualSize / 2;
            InitializeParticles();
            CreateSprite(texture);
            CreateSpriteTextureData();
        }

        #region Particles

        private void InitializeParticles()
        {
            _particles = new List<ParticleEffect>();
            _particlesTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1);
            _particlesTexture.SetData(new[] { Color.White });
            
            _shotParticles = new ParticleEffect() { Name = "Shot Particles" };
            _particles.Add(_shotParticles);

            CreateInitialParticles();
        }

        private void CreateInitialParticles()
        {
            _deathParticles = new ParticleEffect()
            {
                Name = "Death",
                Emitters = new[]
                {
                    new ParticleEmitter(500, TimeSpan.FromSeconds(1.2f), Profile.Point())
                    {
                        TextureRegion = new TextureRegion2D(_particlesTexture),
                        Parameters = new ParticleReleaseParameters()
                        {
                            Speed = new RangeF(70f, 200f),
                            Quantity = 100,
                            Rotation = new RangeF(-1f, 1f),
                            Scale = new RangeF(2.0f, 4.5f),
                            Color = EnemyColor
                        },
                        Modifiers = new IModifier[]
                        {
                            new RotationModifier { RotationRate = 10.0f },
                            new OpacityFastFadeModifier()
                        }
                    }
                }
            };
            _particles.Add(_deathParticles);
        }

        private void RecreateShotParticles()
        {
            var shotAxis = new Axis(_receivedShotRotation);
            _shotParticles.Emitters = new[]
            {
                new ParticleEmitter(500, TimeSpan.FromSeconds(0.7f), Profile.Spray(shotAxis, (float)Math.PI / 2))
                {
                    TextureRegion = new TextureRegion2D(_particlesTexture),
                    Parameters = new ParticleReleaseParameters()
                    {
                        Speed = new RangeF(70f, 130f),
                        Quantity = 10,
                        Rotation = new RangeF(-1f, 1f),
                        Scale = new RangeF(2.0f, 4.5f),
                        Color = EnemyColor
                    },
                    Modifiers = new IModifier[]
                    {
                        new LinearGravityModifier { Direction = shotAxis, Strength = -10f },
                        new RotationModifier { RotationRate = 3.0f },
                        new OpacityFastFadeModifier()
                    }
                }
            };
        }

        #endregion

        protected abstract void CreateSprite(Texture2D texture);

        private void CreateSpriteTextureData()
        {
            var texture = _sprite.TextureRegion.Texture;
            _spriteTextureData = new Color[texture.Width * texture.Height];
            texture.GetData(_spriteTextureData);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            _deathParticles.Trigger(_position);
        }

        public void aa()
        {
            _deathParticles.Trigger(_position);
        }

        public virtual void GetShot(int damage, Vector2 point, float shotRotation)
        {
            if (_immunityTime <= 0.0f)
            {
                GetDamaged(damage);
                _receivedShotRotation = shotRotation;
                _shotParticles.Trigger(point);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _particles.ForEach(particle => particle.Update((float)gameTime.ElapsedGameTime.TotalSeconds));
            Sprite.Alpha = _dyingAlpha;

            if (_dying) return;

            if (_knockbackAcceleration.X > 0.0f || _knockbackAcceleration.Y > 0.0f)
            {
                _knockbackAcceleration *= 0.9f;
                if (Math.Abs(_knockbackAcceleration.X) < 10f) _knockbackAcceleration.X = 0.0f;
                if (Math.Abs(_knockbackAcceleration.Y) < 10f) _knockbackAcceleration.Y = 0.0f;
            }

            _position += _knockbackAcceleration;
            _sprite.SetPosition(_position);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, _sprite.Position);
            _particles.ForEach(particle => spriteBatch.Draw(particle));
        }
    }
}
