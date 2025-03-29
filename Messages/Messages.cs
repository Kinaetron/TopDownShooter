using MoonTools.ECS;
using System.Numerics;

namespace TopDownShooter.Messages;

public readonly record struct EndGame();
public readonly record struct Collided(Entity A, Entity B);
public readonly record struct SelectChaseOffSet(Entity Value);
public readonly record struct ChaseTowards(Entity Entity, Vector2 Direction);
public readonly record struct ChaseStop(Entity Entity);
public readonly record struct CreateProjectile(Entity Entity, Vector2 Direction);
public readonly record struct InitializeExplosion(Entity Entity);
public readonly record struct Exploded(Entity Entity, bool MarkDeath);
public readonly record struct OffsetChaseTowards(Entity Entity);
public readonly record struct OffsetChaseStop(Entity Entity);