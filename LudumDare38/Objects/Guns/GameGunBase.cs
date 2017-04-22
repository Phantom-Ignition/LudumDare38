using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;

namespace LudumDare38.Objects.Guns
{
    public enum GunType
    {
        Basic
    }

    abstract class GameGunBase
    {
        protected int _orbitLevel;
        protected GunType _gunType;
        protected float _angle;
        protected CharacterSprite _sprite;
        
        //--------------------------------------------------
        // Cooldown

        protected float _cooldown;
        protected float _currentCooldown;
        public float CurrentCooldown => _currentCooldown;

        //----------------------//------------------------//

        public GameGunBase(int orbitLevel, GunType gunType, float angle)
        {
            _orbitLevel = orbitLevel;
            _gunType = gunType;
            _angle = angle;
            _currentCooldown = 0;
            CreateSprite();
        }

        protected abstract void CreateSprite();
        public virtual GameProjectile Shot()
        {
            _currentCooldown = _cooldown;
            return null;
        }

        public virtual void Update(GameTime gameTime, float rotation, float floating)
        {
            var floatVector = (float)Math.Sin(floating) * 7 * Vector2.UnitY;
            var center = SceneManager.Instance.VirtualSize / 2;
            rotation = (3 - _orbitLevel) * rotation;
            rotation += _angle;
            var orbitDistance = 17 + _orbitLevel * 30;
            var position = center + new Vector2(orbitDistance * (float)Math.Cos(rotation), orbitDistance * (float)Math.Sin(rotation)) +
                floating * Vector2.UnitY;

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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, _sprite.Position);
        }
    }
}
