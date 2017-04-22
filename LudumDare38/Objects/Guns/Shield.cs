using LudumDare38.Managers;
using LudumDare38.Sprites;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LudumDare38.Objects.Guns
{
    class Shield : GameGunBase
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
    }
}
