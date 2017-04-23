using LudumDare38.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Objects
{
    class GamePlanet : KillableObject
    {
        private Sprite _sprite;
        public Sprite Sprite => _sprite;
        
        public const int Radius = 32;

        private readonly Vector2 _initPosition;
        private Vector2 _position;
        public int X => (int)_position.X;
        public int Y => (int)_position.Y;
        private float _floating;
        
        public GamePlanet(Texture2D texture, Vector2 initPosition)
        {
            _hp = 500;
            _sprite = new Sprite(texture);
            _position = initPosition;
            _initPosition = initPosition;
            _sprite.Position = _position;
        }

        public void Update(GameTime gameTime, out float floating)
        {
            base.Update(gameTime);
            _floating = (_floating + (float)Math.PI / 180) % ((float)Math.PI * 2);
            _position = new Vector2(_initPosition.X, _initPosition.Y + (float)Math.Sin(_floating) * 5);
            floating = (float)Math.Sin(_floating) * 5;
            _sprite.Position = _position;
        }

        public void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            PreDraw(spriteBatch, viewportAdapter);
            spriteBatch.Draw(_sprite);
            spriteBatch.End();
        }
    }
}
