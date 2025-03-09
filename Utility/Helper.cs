using Flam.Shapes;
using System.Numerics;
using TopDownShooter.Components;

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

    public static ColliderUnion GetWorldCollider(Vector2 position, ColliderUnion collider)
    {
        switch(collider.Type)
        {
            case ColliderUnion.ColliderType.Rectangle:
                return new ColliderUnion(
                    new Rectangle(
                    collider.Rectangle.Width,
                    collider.Rectangle.Height,
                    position.X + collider.Rectangle.X,
                    position.Y + collider.Rectangle.Y));

            case ColliderUnion.ColliderType.Circle:
                return new ColliderUnion(
                    new Circle(
                    collider.Circle.Radius,
                    position.X + collider.Circle.X,
                    position.Y + collider.Circle.Y));

            case ColliderUnion.ColliderType.LineSegment:
                return new ColliderUnion(
                    new LineSegment(
                        position + collider.LineSegment.Point1,
                        position + collider.LineSegment.Point2));

            default:
                throw new NotImplementedException();
        }
    }
}
