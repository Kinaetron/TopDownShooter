using Flam.Shapes;
using System.Numerics;

namespace TopDownShooter.Utility;

public static class Helper
{
    public static Rectangle GetWorldRect(Vector2 position, Rectangle rectangle) =>
        new Rectangle(
            rectangle.Width, 
            rectangle.Height, 
            position.X + rectangle.X, 
            position.Y + rectangle.Y);

    public static Circle GetWorldCircle(Vector2 position, Circle circle) =>
        new Circle(
            circle.Radius,
            position.X + circle.X,
            position.Y + circle.Y);
}
