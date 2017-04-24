using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace LudumDare38.Characters
{
    class Boss : EnemyBase
    {
        public Boss(Texture2D texture) : base(texture)
        {
            _hp = 20;
            _gold = 20;
        }

        protected override void CreateSprite(Texture2D texture)
        {
            throw new NotImplementedException();
        }
    }
}
