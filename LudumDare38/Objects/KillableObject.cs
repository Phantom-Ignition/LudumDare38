using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare38.Objects
{
    class KillableObject
    {
        //--------------------------------------------------
        // HP

        protected int _hp;
        public int HP => _hp;
        public bool Alive => _hp > 0;

        //--------------------------------------------------
        // Immunity

        protected float _immunityTime;
        public float ImmunityTime => _immunityTime;
        protected virtual float InitialImmunityTime => 0.0f;

        //--------------------------------------------------
        // Dying

        protected bool _dying;
        public bool Dying => _dying;

        protected float _dyingAlpha;

        //--------------------------------------------------
        // Request erase

        public bool RequestErase { get; set; }

        //----------------------//------------------------//

        public KillableObject()
        {
            _dyingAlpha = 1.0f;
        }

        protected virtual void GetDamaged(int damage)
        {
            _hp = Math.Max(_hp - damage, 0);
            _immunityTime = InitialImmunityTime;
            if (_hp == 0)
            {
                OnDeath();
            }
        }

        protected virtual void OnDeath()
        {
            _dying = true;
            _dyingAlpha = 1.0f;
        }

        public virtual void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_immunityTime > 0.0f)
            {
                _immunityTime -= deltaTime;
                if (_immunityTime < 0.0f) _immunityTime = 0.0f;
            }
            if (_dying)
            {
                _dyingAlpha -= deltaTime / 500;
                // -1.5f because we wan't to give time the particles to fade out
                if (_dyingAlpha <= -1.5f)
                {
                    RequestErase = true;
                }
            }
        }
    }
}
