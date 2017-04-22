using LudumDare38.Managers;
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
    class GameHud
    {
        private Sprite _background;

        public GameHud()
        {
            _background = new Sprite(ImageManager.LoadHud("bg"));
            _background.Position = SceneManager.Instance.VirtualSize / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background);
        }
    }
}
