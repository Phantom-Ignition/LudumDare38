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
        private Sprite _hpBack;
        private Sprite _hpFill;

        public float CurrentHP { get; set; }
        public float MaxHP { get; set; }

        public GameHud()
        {
            _background = new Sprite(ImageManager.LoadHud("Background"));
            _background.Position = SceneManager.Instance.VirtualSize / 2;
            _hpBack = new Sprite(ImageManager.LoadHud("HpBack"));
            _hpBack.OriginNormalized = Vector2.Zero;
            _hpBack.Position = new Vector2(20, 20);
            _hpFill = new Sprite(ImageManager.LoadHud("HpFill"));
            _hpFill.OriginNormalized = Vector2.Zero;
            _hpFill.Position = new Vector2(20, 20);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background);
            spriteBatch.Draw(_hpBack);
            var hpFillTexture = _hpFill.TextureRegion.Texture;
            var hpRate = CurrentHP / MaxHP;
            if (hpRate != 0)
            {
                var w = hpFillTexture.Width * hpRate;
                var h = hpFillTexture.Height;
                var rect = new Rectangle(_hpFill.Position.ToPoint(), new Point((int)w, (int)h));
                spriteBatch.Draw(hpFillTexture, rect, hpFillTexture.Bounds, Color.White);
            }
        }
    }
}
