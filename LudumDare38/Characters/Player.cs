using System;
using System.Collections.Generic;

using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LudumDare38.Characters
{
    class Player : CharacterBase
    {
        //--------------------------------------------------
        // Attacks constants

        private const int NormalAttack = 0;
        private const int AerialAttack = 1;

        //--------------------------------------------------
        // Keys locked (no movement)

        private bool _keysLocked;

        //----------------------//------------------------//

        public Player(Texture2D texture) : base(texture)
        {
            // Stand
            _sprite.CreateFrameList("stand", 0);
            _sprite.AddCollider("stand", new Rectangle(11, 8, 43, 84));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(67, 196, 66, 92)
            });

            // Walking
            _sprite.CreateFrameList("walking", 120);
            _sprite.AddCollider("walking", new Rectangle(11, 8, 43, 84));
            _sprite.AddFrames("walking", new List<Rectangle>()
            {
                new Rectangle(0, 0, 72, 97),
                new Rectangle(73, 0, 72, 97),
                new Rectangle(146, 0, 72, 97),
                new Rectangle(0, 98, 72, 97),
                new Rectangle(73, 98, 72, 97),
                new Rectangle(146, 98, 72, 97),
                new Rectangle(219, 0, 72, 97),
                new Rectangle(292, 0, 72, 97),
                new Rectangle(219, 98, 72, 97),
                new Rectangle(365, 0, 72, 97),
                new Rectangle(292, 98, 72, 97)
            });

            // Jumping
            _sprite.CreateFrameList("jumping", 0);
            _sprite.AddCollider("jumping", new Rectangle(11, 8, 43, 84));
            _sprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(438, 93, 67, 94)
            });

            Position = new Vector2(32, 160);

            _keysLocked = false;
        }

        public void UpdateWithKeyLock(GameTime gameTime, bool keyLock)
        {
            _keysLocked = keyLock;
            if (!keyLock)
                CheckKeys(gameTime);
            base.Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            CheckKeys(gameTime);
            base.Update(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (_dying)
            {
                _sprite.SetIfFrameListExists("dying");
            }
            else if (_isAttacking)
            {
                _sprite.SetFrameList(_attackFrameList[_attackType]);
            }
            else if (!_isOnGround)
            {
                _sprite.SetFrameList("jumping");
            }
            else if ((InputManager.Instace.KeyDown(Keys.Left) || InputManager.Instace.KeyDown(Keys.Right)) && !_keysLocked)
            {
                _sprite.SetFrameList("walking");
            }
            else
            {
                _sprite.SetFrameList("stand");
            }
        }

        private void CheckKeys(GameTime gameTime)
        {
            // Movement
            if (InputManager.Instace.KeyDown(Keys.Left) && Math.Abs(_knockbackAcceleration) < 1200f)
            {
                _sprite.SetDirection(SpriteDirection.Left);
                _movement = -1.0f;
            }
            else if (InputManager.Instace.KeyDown(Keys.Right) && Math.Abs(_knockbackAcceleration) < 1200f)
            {
                _sprite.SetDirection(SpriteDirection.Right);
                _movement = 1.0f;
            }

            _isJumping = InputManager.Instace.KeyDown(Keys.C);
        }
    }
}
