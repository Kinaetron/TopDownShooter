using MoonTools.ECS;

namespace TopDownShooter.Messages;

public readonly record struct EndGame();
public readonly record struct Collided(Entity A, Entity B);