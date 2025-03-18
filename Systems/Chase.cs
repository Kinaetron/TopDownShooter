using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Messages;

namespace TopDownShooter.Systems;

public class Chase : MoonTools.ECS.System
{
    public Chase(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        var deltaTime = (float)delta.TotalSeconds;

        foreach (var chase in ReadMessages<ChaseTowards>())
        {
            if (HasInRelation<Frozen>(chase.Entity))
            {
                Set(chase.Entity, new Velocity(Vector2.Zero));
                continue;
            }

            var maxSpeed = Get<MaxSpeed>(chase.Entity).Value;
            var velocity = maxSpeed * chase.Direction * deltaTime;
            Set(chase.Entity, new Velocity(velocity));
        }

        foreach (var stopChase in ReadMessages<ChaseStop>())
        {
            Set(stopChase.Entity, new Velocity(Vector2.Zero));
        }
    }
}
