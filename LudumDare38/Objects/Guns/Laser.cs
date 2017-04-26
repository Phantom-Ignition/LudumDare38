using LudumDare38.Helpers;
using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LudumDare38.Objects.Guns
{
    class Laser : ICollidableObject
    {
        private CharacterSprite _laserSprite;
        public CharacterSprite Sprite => _laserSprite;

        private Vector2 _position;
        private float _rotation;

        private Texture2D _texture;
        private Color[] _textureData;

        public Laser(Texture2D texture)
        {
            _laserSprite = new CharacterSprite(texture);
            _laserSprite.Origin = new Vector2(4.5f, 0f);
            _laserSprite.Scale = new Vector2(1, 100);

            _laserSprite.CreateFrameList("stand", 100);
            _laserSprite.AddCollider("stand", new Rectangle(0, 0, 9, 2));
            _laserSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 9, 2)
            });

            _laserSprite.CreateFrameList("attack", 600);
            _laserSprite.AddCollider("attack", new Rectangle(0, 0, 9, 2));
            _laserSprite.AddFrames("attack", new List<Rectangle>()
            {
                new Rectangle(0, 0, 9, 2)
            });

            _laserSprite.CreateFrameList("dispose", 40);
            _laserSprite.AddCollider("dispose", new Rectangle(0, 0, 9, 2));
            _laserSprite.AddFrames("dispose", new List<Rectangle>()
            {
                new Rectangle(9, 0, 9, 2),
                new Rectangle(18, 0, 9, 2),
                new Rectangle(27, 0, 9, 2)
            });

            _laserSprite.IsVisible = false;

            CreateTexture();
        }

        public void CreateTexture()
        {
            var rect = Rect();
            var colorData = Enumerable.Range(0, rect.Width * rect.Height).Select(i => Color.Red).ToArray();
            var texture = new Texture2D(SceneManager.Instance.GraphicsDevice, rect.Width, rect.Height);
            texture.SetData(colorData);
            _texture = texture;
            
            _textureData = new Color[_texture.Width * _texture.Height];
            texture.GetData(_textureData);
        }

        public void Update(GameTime gameTime, Vector2 position, float rotation)
        {
            _laserSprite.Update(gameTime);
            _laserSprite.Position = position;
            _laserSprite.Rotation = rotation + (float)Math.PI;
            _position = position;
            _rotation = rotation;
        }

        public float Rotation()
        {
            return _rotation;
        }

        public Rectangle Rect()
        {
            return new Rectangle(0, 0, 9, (int)_laserSprite.Scale.Y);
        }

        public Rectangle BoundingRectangle()
        {
            return CollisionHelper.CalculateBoundingRectangle(Rect(), Transform());
        }

        public Matrix Transform()
        {
            return Matrix.CreateTranslation(new Vector3(-_laserSprite.Origin, 0.0f)) *
                        Matrix.CreateRotationZ(_laserSprite.Rotation) *
                        Matrix.CreateTranslation(new Vector3(_position, 0.0f));
        }

        public Texture2D Texture()
        {
            return _texture;
        }

        public Color[] TextureData()
        {
            return _textureData;
        }
    }
}
