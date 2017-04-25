using LudumDare38.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Scenes
{
    class SceneGameover : SceneBase
    {
        private Sprite _backgroundSprite;

        public override void LoadContent()
        {
            base.LoadContent();

            var center = SceneManager.Instance.VirtualSize / 2;

            _backgroundSprite = new Sprite(ImageManager.LoadHud("Gameover"));
            _backgroundSprite.Position = center;

            SoundManager.StartBgm("SpaceFighterLoop");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (InputManager.Instace.KeyPressed(Keys.Z))
            {
                PlanetManager.Instance.Reset();
                SceneManager.Instance.ChangeScene("SceneTitle");
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_backgroundSprite);

            var center = new Vector2(199, 111);
            var text = PlanetManager.Instance.WavesSurvived.ToString();
            var font = SceneManager.Instance.GameFontBig;
            spriteBatch.DrawString(font, text, new Vector2(170, 92), Color.White);

            spriteBatch.End();
        }
    }
}
