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
using LudumDare38.Helpers;
using MonoGame.Extended.ViewportAdapters;
using LudumDare38.ParticleModifiers;

namespace LudumDare38.Characters
{
    //--------------------------------------------------
    // Enemy Type

    public enum EnemyType
    {
        None,
        Kamikaze,
        Shooter,
        TripleShooter,
        Boss
    }

    abstract class EnemyBase : KillableObject, ICollidableObject
    {
        //--------------------------------------------------
        // Enemy type
        
        public virtual EnemyType Type => EnemyType.None;

        //--------------------------------------------------
        // Character sprite

        protected CharacterSprite _sprite;
        public CharacterSprite Sprite => _sprite;
        private Color[] _spriteTextureData;

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
        // Gold drop

        protected int _gold;
        public int Gold => _gold;

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
                            Speed = new RangeF(150f, 220f),
                            Quantity = 100,
                            Rotation = new RangeF(-1f, 1f),
                            Scale = new RangeF(2.0f, 6f),
                            Color = EnemyColor
                        },
                        Modifiers = new IModifier[]
                        {
                            new VelocityModifier { VelocityThreshold = 0.98f },
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
                        Speed = new RangeF(70f, 100f),
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

        public override void OnDeath()
        {
            base.OnDeath();
            _deathParticles.Trigger(_position);
            SceneManager.Instance.StartCameraShake(3, 500);
            PlanetManager.Instance.Gold += _gold;
        }

        public virtual void GetShot(int damage, Vector2 point, float shotRotation)
        {
            if (_immunityTime <= 0.0f)
            {
                GetDamaged(damage);
                _receivedShotRotation = shotRotation;
                RecreateShotParticles();
                _shotParticles.Trigger(point + new Vector2((float)Math.Cos(shotRotation), (float)Math.Sin(shotRotation)) * 10f);
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
            _sprite.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            PreDraw(spriteBatch, transformMatrix);
            _sprite.Draw(spriteBatch, _sprite.Position);
            _particles.ForEach(particle => spriteBatch.Draw(particle));
            spriteBatch.End();
        }

        #region ICollidableObject

        public float Rotation()
        {
            return _sprite.Rotation;
        }

        public Rectangle Rect()
        {
            return new Rectangle(0, 0, _sprite.GetColliderWidth(), _sprite.GetColliderHeight());
        }

        public Rectangle BoundingRectangle()
        {
            return CollisionHelper.CalculateBoundingRectangle(Rect(), Transform());
        }

        public Matrix Transform()
        {
            return Matrix.CreateTranslation(new Vector3(-_sprite.Origin, 0.0f)) *
                        Matrix.CreateRotationZ(_sprite.Rotation) *
                        Matrix.CreateTranslation(new Vector3(_sprite.Position, 0.0f));
        }

        public Texture2D Texture()
        {
            return _sprite.TextureRegion.Texture;
        }

        public Color[] TextureData()
        {
            var frameRect = _sprite.GetCurrentFrameRectangle();
            var textureData = new Color[frameRect.Width * frameRect.Height];
            _sprite.TextureRegion.Texture.GetData(0,
                new Rectangle(frameRect.X, frameRect.Y, frameRect.Width, frameRect.Height),
                textureData,
                0,
                textureData.Length);
            if (_sprite.Effect == SpriteEffects.FlipVertically)
            {
                // Thanks to Ellye!
                Color[] flipData = new Color[frameRect.Width * frameRect.Height];
                int indexOld = 0;
                int indexNew = 0;
                for (int row = 0; row < frameRect.Height; row++)
                {
                    for (int col = 0; col < frameRect.Width; col++)
                    {
                        indexOld = (frameRect.Width * (frameRect.Height - 1 - row)) + col;
                        flipData[indexNew] = textureData[indexOld];
                        indexNew++;
                    }
                }
                textureData = flipData;
            }
            return textureData;
        }

        #endregion
    }
}
