using Flam.Collision;
using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Messages;

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

                var position2 = Get<Position>(velEntity2).Value;

                if (Has<ColliderUnion>(velEntity) && 
                   Has<ColliderUnion>(velEntity2))
                {
                    var collider1 = Get<ColliderUnion>(velEntity);
                    collider1 = ColliderUnion.GetWorldCollider(position, collider1);

                    var collider2 = Get<ColliderUnion>(velEntity2);

                    collider2 = ColliderUnion.GetWorldCollider(position2, collider2);

                    if (collider1.CollidesWith(collider2))
                    {
                        Send(new Collided(velEntity, velEntity2));
                    }
                }

                if(Has<Chased>(velEntity) && 
                   Has<Chaser>(velEntity2))
                {
                    var vel1ToVel2Distance = (position - position2).Length();
                    var distanceAwayToChase = Get<DistanceCheck>(velEntity2).Value;

                    if(vel1ToVel2Distance <= distanceAwayToChase)
                    {
                        Send(new ChaseTowards(velEntity2, Vector2.Normalize(position - position2)));
                    }
                    else
                    {
                        Send(new ChaseStop(velEntity2));
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

                if (solidColliderWorld.Type == ColliderUnion.ColliderType.Rectangle && 
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

                if (velocityColliderWorld.CollidesWith(solidColliderWorld))
                {
                    Send(new Collided(velEntity, solidEntity));
                }
            }

            Set(velEntity, new Velocity(velocity));
            Set(velEntity, new Position(position));
        }
    }
}
