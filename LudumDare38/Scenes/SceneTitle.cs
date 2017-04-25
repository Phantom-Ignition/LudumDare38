using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using LudumDare38.Managers;
using Microsoft.Xna.Framework.Input;

namespace LudumDare38.Scenes
{
    class SceneTitle : SceneBase
    {
        private Sprite _backgroundSprite;
        private Sprite _titleSprite;
        private Sprite _cgSprite;

        private bool _showCG;

        public override void LoadContent()
        {
            base.LoadContent();

            var center = SceneManager.Instance.VirtualSize / 2;

            _backgroundSprite = new Sprite(ImageManager.LoadHud("Background"));
            _backgroundSprite.Position = center;
            _titleSprite = new Sprite(ImageManager.LoadHud("Title"));
            _titleSprite.Position = center;
            _cgSprite = new Sprite(ImageManager.LoadHud("CG"));
            _cgSprite.Position = center;

            SoundManager.StartBgm("SpaceFighterLoop");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (InputManager.Instace.KeyPressed(Keys.Z))
            {
                if (_showCG)
                {
                    SceneManager.Instance.ChangeScene("ScenePlanet");
                }
                else
                {
                    _showCG = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix transformMatrix)
        {
            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_backgroundSprite);
            spriteBatch.Draw(_titleSprite);
            if (_showCG)
            {
                spriteBatch.Draw(_cgSprite);
            }
            spriteBatch.End();
        }
    }
}
