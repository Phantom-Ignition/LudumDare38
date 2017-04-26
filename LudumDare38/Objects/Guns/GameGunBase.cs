using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace LudumDare38.Objects.Guns
{
    public enum GunType
    {
        Basic,
        Shield,
        LaserGun
    }

    abstract class GameGunBase
    {
        protected GunType _gunType;
        public GunType GunType => _gunType;
        
        protected float _angle;
        protected CharacterSprite _sprite;
        public CharacterSprite Sprite => _sprite;

        private OrbitField _orbitField;
        public OrbitField OrbitField => _orbitField;
        
        //--------------------------------------------------
        // Cooldown

        protected float _cooldown;
        protected float _currentCooldown;
        public float CurrentCooldown => _currentCooldown;

        //--------------------------------------------------
        // Static

        public bool Static { get; set; }

        //----------------------//------------------------//

        public GameGunBase(GunType gunType, OrbitField orbitField)
        {
            _orbitField = orbitField;
            _gunType = gunType;
            _currentCooldown = 0;
            CreateSprite();
        }

        protected abstract void CreateSprite();
        public virtual bool Shot(out GameProjectile projectile)
        {
            projectile = null;
            _currentCooldown = _cooldown;
            return false;
        }

        public void SetOrbitLevel(int orbitLevel)
        {
            _orbitField.OrbitLevel = orbitLevel;
        }

        public void SetAngle(float angle)
        {
            _orbitField.Angle = angle;
        }

        public void SetOrbitFieldAvailable(bool available)
        {
            _orbitField.Available = available;
        }

        public virtual void Update(GameTime gameTime, float rotation, float floating)
        {
            var orbitLevel = _orbitField.OrbitLevel;
            var floatingMultiplier = (orbitLevel) * 1.1f;
            var center = SceneManager.Instance.VirtualSize / 2;
            rotation = Math.Max((3 - orbitLevel), 0.7f) * rotation;
            rotation += _orbitField.Angle;
            var orbitDistance = 17 + orbitLevel * 30;
            var position = center + new Vector2(orbitDistance * (float)Math.Cos(rotation), orbitDistance * (float)Math.Sin(rotation)) +
                floating * floatingMultiplier * Vector2.UnitY;

            _sprite.Rotation = rotation + (float)Math.PI / 2;
            _sprite.Position = position;
            _sprite.Update(gameTime);

            if (_currentCooldown > 0)
            {
                _currentCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_currentCooldown < 0)
                    _currentCooldown = 0.0f;
            }
        }

        public virtual void PreDraw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            PreDraw(spriteBatch, transformMatrix);
            _sprite.Draw(spriteBatch, _sprite.Position);
            spriteBatch.End();
        }
    }
}
