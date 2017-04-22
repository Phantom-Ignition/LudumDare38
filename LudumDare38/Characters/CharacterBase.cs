using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LudumDare38.Objects;
using LudumDare38.Sprites;

namespace LudumDare38.Characters
{
    public abstract class CharacterBase : PhysicalObject
    {
        //--------------------------------------------------
        // Character sprite

        public CharacterSprite _sprite;

        //--------------------------------------------------
        // Combat system

        protected bool _requestAttack;
        protected bool _isAttacking;
        public bool IsAttacking { get { return _isAttacking; } }
        protected int _attackType;
        protected float _attackCooldownTick;
        protected string[] _attackFrameList;
        protected bool _requestErase;
        public bool RequestErase { get { return _requestErase; } }
        public float AttackCooldown { get; set; }
        public bool IsImunity { get { return _sprite.ImmunityAnimationActive; } }
        protected bool _shot;
        protected int _hp;
        public int HP { get { return _hp; } }

        //--------------------------------------------------
        // Damage stuff

        protected bool _canReceiveAttacks;
        public virtual bool CanReceiveAttacks { get { return _canReceiveAttacks; } }

        protected bool _contactDamageEnabled;
        public virtual bool ContactDamageEnabled { get { return _contactDamageEnabled; } }

        //--------------------------------------------------
        // Random

        protected Random _rand;

        //--------------------------------------------------
        // Prevent first OnGroundLand

        private bool _firstGroudLand;

        //--------------------------------------------------
        // Bounding Rectangle

        public override Rectangle BoundingRectangle
        {
            get
            {
                var collider = _sprite.GetBlockCollider();
                int left = (int)Math.Round(Position.X) + collider.OffsetX;
                int top = (int)Math.Round(Position.Y) + collider.OffsetY;
                return new Rectangle(left, top, collider.Width, collider.Height);
            }
        }

        //----------------------//------------------------//

        public CharacterBase(Texture2D texture)
        {
            _sprite = new CharacterSprite(texture);

            // Physics variables init
            _knockbackAcceleration = 0f;
            _dyingAcceleration = 0f;
            IgnoreGravity = false;

            // Battle system init
            _hp = 1;
            _requestAttack = false;
            _isAttacking = false;
            _attackType = -1;
            _attackCooldownTick = 0f;
            AttackCooldown = 0f;
            _shot = false;
            _dying = false;
            _canReceiveAttacks = true;
            _contactDamageEnabled = true;

            // Rand init
            _rand = new Random();

            _firstGroudLand = false;
        }

        public void RequestAttack(int type)
        {
            if (_attackCooldownTick <= 0f)
            {
                _requestAttack = true;
                _attackType = type;
            }
        }

        public virtual void ReceiveAttack(int damage, Vector2 subjectPosition)
        {
            if (_dying || IsImunity) return;

            _sprite.RequestImmunityAnimation();

            _knockbackAcceleration = Math.Sign(BoundingRectangle.Center.X - subjectPosition.X) * 5000f;
            _velocity.Y = -300f;

            GainHP(-damage);
            if (GetHp() <= 0)
            {
                _dyingAcceleration = Math.Sign(Position.X - subjectPosition.X) * 0.7f;
                OnDie();
            }
        }

        public virtual void GainHP(int amount)
        {
            _hp += amount;
        }

        public virtual int GetHp()
        {
            return _hp;
        }

        public void ReceiveAttackWithPoint(int damage, Rectangle subjectRect)
        {
            var position = new Vector2(subjectRect.Center.X, subjectRect.Center.Y);
            ReceiveAttack(damage, position);
        }

        public virtual void ReceiveAttackWithCollider(int damage, Rectangle subjectRect, SpriteCollider colider)
        {
            ReceiveAttackWithPoint(damage, subjectRect);
        }

        public virtual void OnDie()
        {
            _sprite.RequestDyingAnimation();
            _velocity.Y -= 100f;
            _dying = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateAttackCooldown(gameTime);
            UpdateAttack(gameTime);
            UpdateSprite(gameTime);
            if (_sprite.DyingAnimationEnded) _requestErase = true;
        }

        private void UpdateAttackCooldown(GameTime gameTime)
        {
            if (_attackCooldownTick > 0f)
            {
                _attackCooldownTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public virtual void UpdateAttack(GameTime gameTime)
        {
            if (_isAttacking)
            {
                if (_sprite.Looped)
                {
                    _isAttacking = false;
                    _attackType = -1;
                    _shot = false;
                }
                else
                {
                    var sprite = _sprite;
                    if (sprite.GetCurrentFramesList().FramesToAttack.Contains(sprite.CurrentFrame))
                    {
                        DoAttack();
                    }
                }
            }

            if (_requestAttack)
            {
                _isAttacking = true;
                _requestAttack = false;
                _attackCooldownTick = AttackCooldown;
            }
        }

        public virtual void DoAttack() { }

        public virtual void UpdateFrameList()
        {
            if (_dying)
                _sprite.SetIfFrameListExists("dying");
            else if (_sprite.ImmunityAnimationActive)
                _sprite.SetIfFrameListExists("damage");
            else if (_isAttacking)
                _sprite.SetFrameList(_attackFrameList[_attackType]);
            else if (!_isOnGround)
                _sprite.SetFrameList("jumping");
            else
                _sprite.SetFrameList("stand");
        }

        protected virtual void UpdateSprite(GameTime gameTime)
        {
            UpdateFrameList();
            _sprite.SetPosition(Position);
            _sprite.Update(gameTime);
        }

        #region Draw

        public virtual void DrawCharacter(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch, new Vector2(BoundingRectangle.X, BoundingRectangle.Y));
        }

        public virtual void DrawColliderBox(SpriteBatch spriteBatch)
        {
            _sprite.DrawColliders(spriteBatch);
        }

        #endregion
    }
}
