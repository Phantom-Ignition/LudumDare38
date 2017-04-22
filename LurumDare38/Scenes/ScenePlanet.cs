using LudumDare38.Managers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Maps.Tiled;
using LudumDare38.Objects;
using Microsoft.Xna.Framework.Input;

namespace LudumDare38.Scenes
{
    class ScenePlanet : SceneBase
    {
        //--------------------------------------------------
        // HUD

        private GameHud _gameHud;

        //--------------------------------------------------
        // Planet

        private GamePlanet _planet;

        //--------------------------------------------------
        // Guns

        private List<GameGun> _guns;

        //--------------------------------------------------
        // Rotation

        private float _rotation;

        //--------------------------------------------------
        // Floating

        private float _floatingRotation;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            _gameHud = new GameHud();

            CreatePlanet();
            CreateGuns();
        }

        private void CreatePlanet()
        {
            var center = SceneManager.Instance.VirtualSize / 2;
            var planetTexture = ImageManager.Load("Planet");
            _planet = new GamePlanet(planetTexture, center);
        }

        private void CreateGuns()
        {
            _guns = new List<GameGun>();
            _guns.Add(new GameGun(1, GunType.Basic, 0.0f));
            _guns.Add(new GameGun(1, GunType.Basic, (float)Math.PI));
            _guns.Add(new GameGun(2, GunType.Basic, 0.0f));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Update the planet
            float floating;
            _planet.Update(gameTime, out floating);

            // Update the guns
            _guns.ForEach(gun => gun.Update(deltaTime, _rotation, floating));

            // Update the rotation
            if (InputManager.Instace.KeyDown(Keys.Left))
                _rotation -= 0.05f;
            if (InputManager.Instace.KeyDown(Keys.Right))
                _rotation += 0.05f;
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            spriteBatch.Begin(transformMatrix: viewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);

            // Draw the HUD
            _gameHud.Draw(spriteBatch);

            // Draw the planet
            _planet.Draw(spriteBatch);

            // Draw the guns
            _guns.ForEach(gun => gun.Draw(spriteBatch));

            spriteBatch.End();


            base.Draw(spriteBatch, viewportAdapter);
        }
    }
}
