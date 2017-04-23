using LudumDare38.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
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
        protected float _dyingTimer;
        public float DyingAlpha => _dyingAlpha;

        //--------------------------------------------------
        // Request erase

        public bool RequestErase { get; set; }

        //--------------------------------------------------
        // Flash effect

        protected Effect _flashEffect;
        public Effect FlashEffect => _flashEffect;
        protected float _flashProgress;
        protected bool _flashing;

        //----------------------//------------------------//

        public KillableObject(int hp = 0)
        {
            _hp = hp;
            _dyingAlpha = 1.0f;
            _flashEffect = EffectManager.Load("FlashEffect");
            _flashEffect.Parameters["Progress"].SetValue(_flashProgress);
        }

        public virtual void GetDamaged(int damage)
        {
            if (_hp == 0) return;
            _flashing = true;
            _flashProgress = 1.0f;
            _hp = Math.Max(_hp - damage, 0);
            _immunityTime = InitialImmunityTime;
            if (_hp == 0)
            {
                OnDeath();
            }
        }

        public virtual void OnDeath()
        {
            _dying = true;
            _dyingAlpha = 1.0f;
        }

        public virtual void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_flashing)
            {
                _flashProgress -= 0.1f * deltaTime / 10;
                _flashEffect.Parameters["Progress"].SetValue(_flashProgress);
                if (_flashProgress <= 0.0f)
                {
                    _flashProgress = 1.0f;
                    _flashing = false;
                }
            }
            if (_immunityTime > 0.0f)
            {
                _immunityTime -= deltaTime;
                if (_immunityTime < 0.0f) _immunityTime = 0.0f;
            }
            if (_dying)
            {
                _dyingTimer += deltaTime;
                _dyingAlpha -= deltaTime / 300;
                // -1.5f because we wan't to give time the particles to fade out
                if (_dyingAlpha <= -1.5f)
                {
                    RequestErase = true;
                }
            }
        }

        public void PreDraw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            if (_flashing)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, transformMatrix: viewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp, effect: _flashEffect);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, transformMatrix: viewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);
            }
        }
    }
}
