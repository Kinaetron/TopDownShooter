using Flam.Collision;
using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class Motion : MoonTools.ECS.System
{
    private readonly Filter _solidFilter;
    private readonly Filter _velocityFilter;

    public Motion(World world)
        :base(world)
    {
        _solidFilter =
            FilterBuilder
            .Include<Solid>()
            .Include<Position>()
            .Include<ColliderUnion>()
            .Build();

        _velocityFilter =
            FilterBuilder
            .Include<Velocity>()
            .Include<Position>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var velEntity in _velocityFilter.Entities)
        {
            var velocity = Get<Velocity>(velEntity).Value;
            var position = Get<Position>(velEntity).Value;

            position += velocity;

            foreach (var velEntity2 in _velocityFilter.Entities)
            {
                if(velEntity == velEntity2)
                {
                    continue;
                }

                if(Has<CanBeFrozen>(velEntity) && Has<Freezes>(velEntity2))
                {
                     if(Has<ColliderUnion>(velEntity) && Has<ColliderUnion>(velEntity2))
                     {
                        var freezesTime = Get<Freezes>(velEntity2).Value;

                        var freezesCollder = Get<ColliderUnion>(velEntity2);
                        var freezePosition = Get<Position>(velEntity2).Value;

                        freezesCollder =  ColliderUnion.GetWorldCollider(freezePosition, freezesCollder);

                        var canBeFrozenPosition = Get<Position>(velEntity).Value;
                        var canBeFrozenCollider = Get<ColliderUnion>(velEntity);

                        canBeFrozenCollider = ColliderUnion.GetWorldCollider(
                            canBeFrozenPosition,
                            canBeFrozenCollider);

                        if (freezesCollder.CollidesWith(canBeFrozenCollider))
                        {
                            Set(velEntity, new Frozen(freezesTime));
                        }
                    }
                }
            }

            foreach (var solidEntity in _solidFilter.Entities)
            {
                if(!Has<ColliderUnion>(velEntity))
                {
                    continue;
                }

                var solidCollider = Get<ColliderUnion>(solidEntity);
                var solidPosition = Get<Position>(solidEntity).Value;
                var solidColliderWorld = ColliderUnion.GetWorldCollider(solidPosition, solidCollider);

                var velocityCollider = Get<ColliderUnion>(velEntity);
                var velocityColliderWorld = ColliderUnion.GetWorldCollider(position, velocityCollider);

                if(solidColliderWorld.Type == ColliderUnion.ColliderType.Rectangle && 
                   velocityColliderWorld.Type == ColliderUnion.ColliderType.Rectangle)
                {
                    var collisionResult = 
                        CollisionDetection.MovingRectangleCollidesRectangle(
                            velocityColliderWorld.Rectangle, 
                            solidColliderWorld.Rectangle, 
                            velocity);

                    if (collisionResult.Hit)
                    {
                        float epsilon = 0.01f;
                        position = collisionResult.ContactPoint + collisionResult.Normal * epsilon;

                        var remainingVelocity = velocity - Vector2.Dot(velocity, collisionResult.Normal) * collisionResult.Normal;
                        velocity = remainingVelocity;
                    }
                }
            }

            Set(velEntity, new Velocity(velocity));
            Set(velEntity, new Position(position));
        }
    }
}
