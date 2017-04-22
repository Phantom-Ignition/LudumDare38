using System;
using Microsoft.Xna.Framework;
using LudumDare38.Sprites;
using LudumDare38.Managers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LudumDare38.Objects
{
    //--------------------------------------------------
    // Projectile Subject

    public enum ProjectileSubject
    {
        FromPlayer,
        FromEnemy
    }
    //--------------------------------------------------
    // Projectile Type

    public enum ProjectileType
    {
        BasicProjectile
    }

    public class GameProjectile
    {
        //--------------------------------------------------
        // Sprite

        private CharacterSprite _sprite;
        public CharacterSprite Sprite => _sprite;

        //--------------------------------------------------
        // Position

        private Vector2 _position;
        public Vector2 Position => _position;
        public Vector2 LastPosition { get; private set; }

        //--------------------------------------------------
        // Rotation & Speed

        private float _rotation;
        private float _speed;

        //--------------------------------------------------
        // Subject

        private ProjectileSubject _subject;
        public ProjectileSubject Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
            }
        }

        //--------------------------------------------------
        // Damage

        private int _damage;
        public int Damage => _damage;

        //--------------------------------------------------
        // Request erase

        public bool RequestErase { get; set; }

        //--------------------------------------------------
        // Random

        protected Random _rand;

        //--------------------------------------------------
        // Bouding box

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
            }
        }

        //----------------------//------------------------//

        public GameProjectile(ProjectileType type, Vector2 initialPosition, float rotation, int speed, int damage, ProjectileSubject subject)
        {
            _position = initialPosition;
            LastPosition = _position;
            _rotation = rotation;
            _speed = speed;
            _damage = damage;
            _subject = subject;
            _rand = new Random();
            CreateSprite(type);
        }

        private void CreateSprite(ProjectileType type)
        {
            var textureName = type.ToString();
            var texture = ImageManager.LoadProjecitile(textureName);
            _sprite = new CharacterSprite(texture);
            _sprite.Rotation = _rotation;

            _sprite.CreateFrameList("stand", 200);
            _sprite.AddCollider("stand", new Rectangle(0, 0, 20, 20));
            _sprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 20, 20),
                new Rectangle(20, 0, 20, 20)
            });
        }

        public void Update(GameTime gameTime)
        {
            LastPosition = _position;
            MoveProjectile(gameTime);
            _sprite.Position = _position;
            _sprite.Update(gameTime);

            var tileX = (int)(_position.X / MapManager.Instance.TileSize.X);
            var tileY = (int)(_position.Y / MapManager.Instance.TileSize.Y);
            var bounds = SceneManager.Instance.VirtualSize;
            if (_position.X >= bounds.X || _position.Y >= bounds.Y ||
                Position.X + Sprite.TextureRegion.Width <= 0 || Position.Y + Sprite.TextureRegion.Height <= 0)
                Destroy();
        }

        private void MoveProjectile(GameTime gameTime)
        {
            Vector2 direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
            direction.Normalize();
            _position += direction * _speed;
        }

        public void Destroy()
        {
            Sprite.Alpha = 0.0f;
            RequestErase = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
