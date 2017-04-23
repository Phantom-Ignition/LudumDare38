using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LudumDare38.Objects
{
    public interface ICollidableObject
    {
        float Rotation();
        Rectangle Rect();
        Rectangle BoundingRectangle();
        Matrix Transform();
        Texture2D Texture();
        Color[] TextureData();
    }
}
