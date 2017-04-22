using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Objects
{
    class GamePlanet : Sprite
    {
        private readonly Vector2 _initPosition;
        private Vector2 _position;
        private float _floating;

        public GamePlanet(Texture2D texture, Vector2 initPosition) : base(texture)
        {
            _position = initPosition;
            _initPosition = initPosition;
        }

        public void Update(GameTime gameTime, out float floating)
        {
            _floating = (_floating + (float)Math.PI / 180) % ((float)Math.PI * 2);
            _position = new Vector2(_initPosition.X, _initPosition.Y + (float)Math.Sin(_floating) * 5);
            floating = (float)Math.Sin(_floating) * 5;
            Position = _position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this);
        }
    }
}
