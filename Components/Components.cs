using Flam.Shapes;
using System.Numerics;

namespace TopDownShooter.Components;

public struct Timer
{
    public float Max { get; }
    public float Time { get; private set; }
    public float Remaining
    {
        get { return Time / Max; }
    }

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

public readonly record struct CanFreeze();
public readonly record struct Freeze(float FreezeTime);
public readonly record struct Frozen();
public readonly record struct Solid();
public readonly record struct Player();
public readonly record struct BasicEnemy();
public readonly record struct Velocity(Vector2 Value);
public readonly record struct Accerlation(float Value);
public readonly record struct Direction(Vector2 Value);
public readonly record struct Remainder(Vector2 Value);
public readonly record struct CircleBounds(Circle Value);
public readonly record struct MaxSpeed(float Value);
public readonly record struct Position(Vector2 Value);