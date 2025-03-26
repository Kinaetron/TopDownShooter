using Flam.Shapes;
using Flam.Collision;

using System.Numerics;

namespace TopDownShooter.Components;

public record struct Timer
{
    public float Max { get; }
    public float Time { get; private set; }
    public readonly float Remaining => Time / Max;

    public Timer(float time)
    {
        Time = Max = time;
    }

    public Timer Update(float newTime)
    {
        Time = newTime;
        return this;
    }
}

public readonly record struct ColliderUnion
{
    public enum ColliderType { Circle, LineSegment, Rectangle }
    public ColliderType Type { get; }
    public Circle Circle { get; }
    public LineSegment LineSegment { get; }
    public Rectangle Rectangle { get; }
    public ColliderUnion(Circle circle)
    {
        Type = ColliderType.Circle;
        Circle = circle;
        Rectangle = default;
        LineSegment = default;
    }

    public ColliderUnion(LineSegment lineSegment)
    {
        Type = ColliderType.LineSegment;
        Circle = default;
        Rectangle = default;
        LineSegment = lineSegment;
    }

    public ColliderUnion(Rectangle rectangle)
    {
        Type = ColliderType.Rectangle;
        Circle = default;
        Rectangle = rectangle;
        LineSegment = default;
    }

    public bool CollidesWith(ColliderUnion other)
    {
        return Type switch
        {
            ColliderType.Circle => other.Type switch
            {
                ColliderType.Circle => CollisionDetection.CircleCollidesCircle(Circle, other.Circle),
                ColliderType.LineSegment => CollisionDetection.CircleCollidesLineSegment(Circle, other.LineSegment),
                ColliderType.Rectangle => CollisionDetection.CircleCollidesRectangle(Circle, other.Rectangle),
                _ => throw new NotImplementedException(),
            },
            ColliderType.LineSegment => other.Type switch
            {
                ColliderType.Circle => CollisionDetection.CircleCollidesLineSegment(other.Circle, LineSegment),
                ColliderType.LineSegment => throw new NotImplementedException(),
                ColliderType.Rectangle => CollisionDetection.LineSegmentCollidesRectangle(LineSegment, other.Rectangle),
                _ => throw new NotImplementedException(),
            },
            ColliderType.Rectangle => other.Type switch
            {
                ColliderType.Circle => CollisionDetection.CircleCollidesRectangle(other.Circle, Rectangle),
                ColliderType.LineSegment => CollisionDetection.LineSegmentCollidesRectangle(other.LineSegment, Rectangle),
                ColliderType.Rectangle => CollisionDetection.RectangleCollidesRectangle(Rectangle, other.Rectangle),
                _ => throw new NotImplementedException(),
            },
            _ => throw new NotImplementedException(),
        };
    }

    public static ColliderUnion GetWorldCollider(Vector2 position, ColliderUnion collider)
    {
        return collider.Type switch
        {
            ColliderType.Rectangle => new ColliderUnion(
                                new Rectangle(
                                collider.Rectangle.Width,
                                collider.Rectangle.Height,
                                position.X + collider.Rectangle.X,
                                position.Y + collider.Rectangle.Y)),
            ColliderType.Circle => new ColliderUnion(
                                new Circle(
                                collider.Circle.Radius,
                                position.X + collider.Circle.X,
                                position.Y + collider.Circle.Y)),
            ColliderType.LineSegment => new ColliderUnion(
                                new LineSegment(
                                    position + collider.LineSegment.Point1,
                                    position + collider.LineSegment.Point2)),
            _ => throw new NotImplementedException(),
        };
    }
}
public readonly record struct Freezes(float Value);
public readonly record struct CanBeFrozen();
public readonly record struct Frozen(float Value);
public readonly record struct MarkedToFreeze(float Value);
public readonly record struct Solid();
public readonly record struct Player();
public readonly record struct Rat();
public readonly record struct Velocity(Vector2 Value);
public readonly record struct Accerlation(float Value);
public readonly record struct Direction(Vector2 Value);
public readonly record struct Remainder(Vector2 Value);
public readonly record struct MaxSpeed(float Value);
public readonly record struct Position(Vector2 Value);
public readonly record struct DistanceCheck(float Value);
public readonly record struct DestroyOnHit();
public readonly record struct MarkedToDestroy();
public readonly record struct CanDieOnHit();
public readonly record struct CanKillOnHit();
public readonly record struct Translate(Vector2 Value);
public readonly record struct SessionTimer(float Value);
public readonly record struct SpawnTime(float Value);
public readonly record struct Chaser();
public readonly record struct Chased();
public readonly record struct Dog();
public readonly record struct DisableShoot();
public readonly record struct Turret();
public readonly record struct ProjectileCreator();
public readonly record struct Radius(float Value);
public readonly record struct DisableTime(float Value);
public readonly record struct ShootAwayFromPosition(float Value);
public readonly record struct Center(Vector2 Value);
public readonly record struct ShotDistanceCheck(float Value);
public readonly record struct CreatesBullets();
public readonly record struct Blob();
public readonly record struct ExplosionTrigger(float Value);
public readonly record struct ExplosionCountDown();
public readonly record struct ExplosionRadius(Circle Value);
public readonly record struct Exploding();