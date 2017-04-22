using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;

namespace LudumDare38.Characters
{
    class Shooter : EnemyBase
    {
        protected override float InitialImmunityTime => 50.0f;
        protected override HslColor EnemyColor => new HslColor(317, 0.55f, 0.39f);

        public Shooter(Texture2D texture) : base(texture)
        {
            _hp = 6;
        }

        protected override void CreateSprite(Texture2D texture)
        {
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(47f, 18);

            _sprite.CreateFrameList("stand", 0);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 114, 78));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 114, 78)
            });
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
            _sprite.Rotation = (float)Math.Atan2(_velocity.Y, _velocity.X); ;
            if (_velocity.X < 0)
            {
                _sprite.Effect = SpriteEffects.FlipVertically;
            }

            _sprite.Position = _position;
        }
    }
}
