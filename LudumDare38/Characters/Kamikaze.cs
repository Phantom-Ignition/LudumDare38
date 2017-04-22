using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;

namespace LudumDare38.Characters
{
    class Kamikaze : EnemyBase
    {
        public Kamikaze(Texture2D texture) : base(texture)
        {
        }

        protected override void CreateSprite(Texture2D texture)
        {
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(47f, 12.5f);

            _sprite.CreateFrameList("stand", 0);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 94, 25));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 94, 36)
            });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            _position += _velocity * 2;
            _sprite.Rotation = (float)Math.Atan2(_velocity.Y, _velocity.X); ;
            if (_velocity.X < 0)
            {
                _sprite.Effect = SpriteEffects.FlipVertically;
            }
        }
    }
}
