using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Messages;

namespace TopDownShooter.Systems;

public class ChaseOffset : MoonTools.ECS.System
{
    public ChaseOffset(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        var deltaTime = (float)delta.TotalSeconds;

        foreach (var message in ReadMessages<OffsetChaseTowards>())
        {
            if (HasInRelation<Frozen>(message.Entity))
            {
                Set(message.Entity, new Velocity(Vector2.Zero));
                continue;
            }

            var position = Get<Position>(message.Entity).Value;

            var targetEntity = OutRelationSingleton<ChasingOffSet>(message.Entity);
            var targetPosition = Get<Position>(targetEntity).Value;

            var direction = Vector2.Normalize(targetPosition - position);
            var maxSpeed = Get<MaxSpeed>(message.Entity).Value;
            var velocity = maxSpeed * direction * deltaTime;
            Set(message.Entity, new Velocity(velocity));
        }
    }
}
