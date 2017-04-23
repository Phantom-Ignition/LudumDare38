using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using LudumDare38.Helpers;

namespace LudumDare38.Objects.Guns
{
    class Shield : GameGunBase, ICollidableObject
    {
        public Shield(int orbitLevel, GunType gunType, float angle) : base(orbitLevel, gunType, angle)
        {
            Static = true;
        }

        protected override void CreateSprite()
        {
            var texture = ImageManager.LoadGun("Shield");
            _sprite = new CharacterSprite(texture);
            _sprite.Origin = new Vector2(19.5f, 7.5f);

            _sprite.CreateFrameList("stand", 100);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 39, 15));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 39, 15),
                new Rectangle(39, 0, 39, 15)
            });

            _sprite.CreateFrameList("damaged_1", 30);
            _sprite.AddCollider("damaged_1", new Rectangle(0, 0, 20, 20));
            _sprite.AddFrames("damaged_1", new List<Rectangle>()
            {
                new Rectangle(0, 15, 39, 15),
                new Rectangle(39, 15, 39, 15)
            });

            _sprite.CreateFrameList("damaged_2", 30);
            _sprite.AddCollider("damaged_2", new Rectangle(0, 0, 20, 20));
            _sprite.AddFrames("damaged_2", new List<Rectangle>()
            {
                new Rectangle(0, 30, 39, 15),
                new Rectangle(39, 30, 39, 15)
            });
        }

        public Rectangle BoundingRectangle()
        {
            return CollisionHelper.CalculateBoundingRectangle(Rect(), Transform());
        }

        public Rectangle Rect()
        {
            return new Rectangle(0, 0, _sprite.GetColliderWidth(), _sprite.GetColliderHeight());
        }

        public float Rotation()
        {
            return _sprite.Rotation;
        }

        public Texture2D Texture()
        {
            return _sprite.TextureRegion.Texture;
        }

        public Color[] TextureData()
        {
            var frameRect = _sprite.GetCurrentFrameRectangle();
            var textureData = new Color[frameRect.Width * frameRect.Height];
            _sprite.TextureRegion.Texture.GetData(0,
                new Rectangle(frameRect.X, frameRect.Y, frameRect.Width, frameRect.Height),
                textureData,
                0,
                textureData.Length);
            return textureData;
        }

        public Matrix Transform()
        {
            return Matrix.CreateTranslation(new Vector3(-_sprite.Origin, 0.0f)) *
                        Matrix.CreateRotationZ(_sprite.Rotation) *
                        Matrix.CreateTranslation(new Vector3(_sprite.Position, 0.0f));
        }
    }
}
