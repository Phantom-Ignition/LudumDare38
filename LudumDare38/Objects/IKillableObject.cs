using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace LudumDare38.Objects
{
    public interface IKillableObject
    {
        void GetDamaged(int damage);
        void OnDeath();
        void Update(GameTime gameTime);
        void PreDraw(SpriteBatch spriteBatch, Matrix transformMatrix);
    }
}
