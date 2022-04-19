using Microsoft.Xna.Framework;
namespace Chapter6Game.Content
{   public struct Circle
    {
        public Vector2 Center;
        public float Radius;
        // Constructor to be used for our Circle
        public Circle(Vector2 Position, float radius)
        {
            Center = Position;
            Radius = radius;
        }
        public bool Intersects(Rectangle rectangle)
        {
            Vector2 v = new Vector2(MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right),
                                    MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom));

            Vector2 direction = Center - v;
            float distanceSquared = direction.LengthSquared();

            return ((distanceSquared > 0) && (distanceSquared < Radius * Radius));
        }
    }
}
