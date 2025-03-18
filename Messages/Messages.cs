using MoonTools.ECS;
using System.Numerics;

namespace TopDownShooter.Messages;

public readonly record struct EndGame();
public readonly record struct Collided(Entity A, Entity B);
public readonly record struct ChaseTowards(Entity Entity, Vector2 Direction);
public readonly record struct ChaseStop(Entity Entity);