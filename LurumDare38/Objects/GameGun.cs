using LudumDare38.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;

namespace LudumDare38.Objects
{
    public enum GunType
    {
        Basic
    }

    class GameGun
    {
        private int _orbitLevel;
        private GunType _gunType;
        private float _angle;
        private Sprite _sprite;

        public GameGun(int orbitLevel, GunType gunType, float angle)
        {
            _orbitLevel = orbitLevel;
            _gunType = gunType;
            _angle = angle;
            CreateSprite();
        }

        private void CreateSprite()
        {
            var texture = ImageManager.LoadGun(_gunType.ToString());
            _sprite = new Sprite(texture);
            _sprite.Position = SceneManager.Instance.VirtualSize / 2;
        }

        public void Update(float deltaTime, float rotation, float floating)
        {
            var floatVector = (float)Math.Sin(floating) * 7 * Vector2.UnitY;
            var center = SceneManager.Instance.VirtualSize / 2;
            rotation = (3 - _orbitLevel) * 0.7f * rotation;
            rotation += _angle;
            var orbitDistance = 17 + _orbitLevel * 30;
            var position = center + new Vector2(orbitDistance * (float)Math.Cos(rotation), orbitDistance * (float)Math.Sin(rotation)) +
                floating * Vector2.UnitY;

            _sprite.Rotation = rotation + (float)Math.PI / 2;
            _sprite.Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }
    }
}
