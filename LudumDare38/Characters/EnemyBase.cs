using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LudumDare38.Managers;
using LudumDare38.Sprites;

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

    abstract class EnemyBase
    {
        //--------------------------------------------------
        // Character sprite

        protected CharacterSprite _sprite;
        public CharacterSprite Sprite => _sprite;

        //--------------------------------------------------
        // Target

        protected Vector2 _target;

        //--------------------------------------------------
        // Combat system

        protected int _hp;
        public int HP => _hp;

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
            }
        }

        protected Vector2 _knockbackAcceleration;

        //----------------------//------------------------//

        public EnemyBase(Texture2D texture)
        {
            _target = SceneManager.Instance.VirtualSize / 2;
            CreateSprite(texture);
        }

        protected abstract void CreateSprite(Texture2D texture);

        public virtual void Update(GameTime gameTime)
        {
            if (_knockbackAcceleration.X > 0.0f || _knockbackAcceleration.Y > 0.0f)
            {
                _knockbackAcceleration *= 0.9f;
                if (Math.Abs(_knockbackAcceleration.X) < 10f) _knockbackAcceleration.X = 0.0f;
                if (Math.Abs(_knockbackAcceleration.Y) < 10f) _knockbackAcceleration.Y = 0.0f;
            }

            _position += _knockbackAcceleration;
            _sprite.Position = _position;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, _sprite.Position);
        }
    }
}
